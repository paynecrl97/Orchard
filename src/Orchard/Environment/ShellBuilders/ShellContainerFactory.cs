using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Autofac;
using Autofac.Builder;
using Autofac.Configuration;
using Autofac.Core;
using Autofac.Features.Indexed;
using Orchard.Environment.AutofacUtil;
using Orchard.Environment.AutofacUtil.DynamicProxy2;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;
using Orchard.Environment.ShellBuilders.Models;
using Orchard.Events;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI;

namespace Orchard.Environment.ShellBuilders {

    public interface IShellContainerFactory {
        ILifetimeScope CreateContainer(ShellSettings settings, ShellBlueprint blueprint);
    }

    public class ShellContainerFactory : IShellContainerFactory {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IShellContainerRegistrations _shellContainerRegistrations;
        private readonly Dictionary<Type, string> _registrationNames;

        public ShellContainerFactory(ILifetimeScope lifetimeScope, IShellContainerRegistrations shellContainerRegistrations) {
            _lifetimeScope = lifetimeScope;
            _shellContainerRegistrations = shellContainerRegistrations;

            _registrationNames = new Dictionary<Type, string>();

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public ILifetimeScope CreateContainer(ShellSettings settings, ShellBlueprint blueprint) {
            var intermediateScope = _lifetimeScope.BeginLifetimeScope(
                builder => {
                    foreach (var item in blueprint.Dependencies.Where(t => typeof(IModule).IsAssignableFrom(t.Type))) {
                        var registration = RegisterType(builder, item)
                            .Keyed<IModule>(item.Type)
                            .InstancePerDependency();

                        foreach (var parameter in item.Parameters) {
                            registration = registration
                                .WithParameter(parameter.Name, parameter.Value)
                                .WithProperty(parameter.Name, parameter.Value);
                        }
                    }
                });

            return intermediateScope.BeginLifetimeScope(
                "shell",
                builder => {
                    var dynamicProxyContext = new DynamicProxyContext();

                    builder.Register(ctx => dynamicProxyContext);
                    builder.Register(ctx => settings);
                    builder.Register(ctx => blueprint.Descriptor);
                    builder.Register(ctx => blueprint);

                    var moduleIndex = intermediateScope.Resolve<IIndex<Type, IModule>>();
                    foreach (var item in blueprint.Dependencies.Where(t => typeof(IModule).IsAssignableFrom(t.Type))) {
                        builder.RegisterModule(moduleIndex[item.Type]);
                    }

                    var decorators = new List<KeyValuePair<DependencyBlueprint, string>>();

                    foreach (var item in blueprint.Dependencies.Where(t => typeof(IDependency).IsAssignableFrom(t.Type))) {
                        var registration = RegisterType(builder, item)
                            .EnableDynamicProxy(dynamicProxyContext)
                            .InstancePerLifetimeScope();

                        var decoratorAttribute = item.Type.GetCustomAttribute<OrchardDecoratorAttribute>();
                        if (decoratorAttribute == null) {
                            foreach (var interfaceType in GetInterfacesFromBlueprint(item)) {
                                registration = ConfigureRegistration(registration, interfaceType);
                            }
                        }
                        else {
                            decorators.Add(new KeyValuePair<DependencyBlueprint, string>(item, decoratorAttribute.Priority));
                        }

                        if (typeof(IEventHandler).IsAssignableFrom(item.Type)) {
                            var interfaces = item.Type.GetInterfaces();
                            foreach (var interfaceType in interfaces) {

                                // register named instance for each interface, for efficient filtering inside event bus
                                // IEventHandler derived classes only
                                if (interfaceType.GetInterface(typeof (IEventHandler).Name) != null) {
                                    registration = registration.Named<IEventHandler>(interfaceType.Name);
                                }
                            }
                        }

                        foreach (var parameter in item.Parameters) {
                            registration = registration
                                .WithParameter(parameter.Name, parameter.Value)
                                .WithProperty(parameter.Name, parameter.Value);
                        }
                    }

                    foreach (var kvp in decorators.OrderBy(kvp=>kvp.Value, new FlatPositionComparer())) {
                        var decorator = RegisterType(builder, kvp.Key);

                        foreach (var interfaceType in GetInterfacesFromBlueprint(kvp.Key)){

                            var scopedInterfaceType = interfaceType;
                            if (!_registrationNames.ContainsKey(scopedInterfaceType)){
                                var exception = new OrchardFatalException(T("The only registered implementations of `{0}` are decorators. In order to avoid circular dependenices, there must be at least one implementation that is not also an implementation of `IDecorator`", interfaceType.FullName));
                                Logger.Fatal(exception, "Could not complete dependency registration as a circular dependency chain has been found.");

                                throw exception;
                            }

                            var scopedName = _registrationNames[scopedInterfaceType];
                            decorator.WithParameter(
                                (p, c) => p.ParameterType == scopedInterfaceType,
                                (p, c) => c.ResolveNamed(scopedName, scopedInterfaceType));

                            decorator = ConfigureRegistration(decorator, scopedInterfaceType);
                        }
                    }

                    foreach (var item in blueprint.Controllers) {
                        var serviceKeyName = (item.AreaName + "/" + item.ControllerName).ToLowerInvariant();
                        var serviceKeyType = item.Type;
                        RegisterType(builder, item)
                            .EnableDynamicProxy(dynamicProxyContext)
                            .Keyed<IController>(serviceKeyName)
                            .Keyed<IController>(serviceKeyType)
                            .WithMetadata("ControllerType", item.Type)
                            .InstancePerDependency();
                    }

                    foreach (var item in blueprint.HttpControllers) {
                        var serviceKeyName = (item.AreaName + "/" + item.ControllerName).ToLowerInvariant();
                        var serviceKeyType = item.Type;
                        RegisterType(builder, item)
                            .EnableDynamicProxy(dynamicProxyContext)
                            .Keyed<IHttpController>(serviceKeyName)
                            .Keyed<IHttpController>(serviceKeyType)
                            .WithMetadata("ControllerType", item.Type)
                            .InstancePerDependency();
                    }

                    // Register code-only registrations specific to a shell
                    _shellContainerRegistrations.Registrations(builder);

                    var optionalShellByNameConfig = HostingEnvironment.MapPath("~/Config/Sites." + settings.Name + ".config");
                    if (File.Exists(optionalShellByNameConfig)) {
                        builder.RegisterModule(new ConfigurationSettingsReader(ConfigurationSettingsReaderConstants.DefaultSectionName, optionalShellByNameConfig));
                    }
                    else {
                        var optionalShellConfig = HostingEnvironment.MapPath("~/Config/Sites.config");
                        if (File.Exists(optionalShellConfig))
                            builder.RegisterModule(new ConfigurationSettingsReader(ConfigurationSettingsReaderConstants.DefaultSectionName, optionalShellConfig));
                    }
                });
        }

        private IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType(ContainerBuilder builder, ShellBlueprintItem item) {
            return builder.RegisterType(item.Type)
                .WithProperty("Feature", item.Feature)
                .WithMetadata("Feature", item.Feature);
        }

        private IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> ConfigureRegistration(IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registration, Type interfaceType)
        {
            var registrationName = Guid.NewGuid().ToString();
            _registrationNames[interfaceType] = registrationName;

            registration = registration.Named(registrationName, interfaceType);
            registration = registration.As(interfaceType);

            if (typeof(ISingletonDependency).IsAssignableFrom(interfaceType)) {
                registration = registration.InstancePerMatchingLifetimeScope("shell");
            }
            else if (typeof(IUnitOfWorkDependency).IsAssignableFrom(interfaceType)) {
                registration = registration.InstancePerMatchingLifetimeScope("work");
            }
            else if (typeof(ITransientDependency).IsAssignableFrom(interfaceType)) {
                registration = registration.InstancePerDependency();
            }

            return registration;
        }

        private IEnumerable<Type> GetInterfacesFromBlueprint(DependencyBlueprint blueprint) {
            return blueprint.Type.GetInterfaces()
                .Where(itf => typeof(IDependency).IsAssignableFrom(itf)
                            && !typeof(IEventHandler).IsAssignableFrom(itf));
        }
    }
}