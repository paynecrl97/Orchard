using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Orchard.Environment.ShellBuilders.Models;
using Orchard.Events;
using Orchard.Localization;
using Orchard.Logging;

namespace Orchard.Environment.ShellBuilders {

    public interface IShellContainerFactory {
        ILifetimeScope CreateContainer(ShellSettings settings, ShellBlueprint blueprint);
    }

    public class ShellContainerFactory : IShellContainerFactory {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IShellContainerRegistrations _shellContainerRegistrations;
        private ConcurrentDictionary<Type, ConcurrentBag<string>> _registrationNames;

        public ShellContainerFactory(ILifetimeScope lifetimeScope, IShellContainerRegistrations shellContainerRegistrations) {
            _lifetimeScope = lifetimeScope;
            _shellContainerRegistrations = shellContainerRegistrations;

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

                    _registrationNames = new ConcurrentDictionary<Type, ConcurrentBag<string>>();

                    var moduleIndex = intermediateScope.Resolve<IIndex<Type, IModule>>();
                    foreach (var item in blueprint.Dependencies.Where(t => typeof(IModule).IsAssignableFrom(t.Type))) {
                        builder.RegisterModule(moduleIndex[item.Type]);
                    }

                    var decorators = new ConcurrentQueue<KeyValuePair<DependencyBlueprint, string>>();
                    var itemsToBeRegistered = new ConcurrentQueue<ItemToBeRegistered>();

                    foreach (var item in blueprint.Dependencies.Where(t => typeof(IDependency).IsAssignableFrom(t.Type))) {
                        var isEventHandler = typeof (IEventHandler).IsAssignableFrom(item.Type);

                        var decoratingTypes = item.Type.GetInterfaces()
                            .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDecorator<>))
                            .Select(t => t.GetGenericArguments().First());

                        var isDecorator = decoratingTypes != null && decoratingTypes.Any();

                        if (isDecorator && isEventHandler) {
                            Logger.Error(string.Format("Type `{0}` is annotated with the OrchardDependency attribute, but is also an IEventHandler. Decorating IEventHandlers is not currently supported. This decorator will not be registered.", item.Type.FullName));

                            continue;
                        }
                        
                        if (isDecorator) {
                            // If this service is a decorator, we need to determine which types it decorates
                            foreach (var itemToBeRegistered in itemsToBeRegistered) {
                                foreach (var interfaceType in decoratingTypes) {
                                    if (itemToBeRegistered.InterfaceTypes.Contains(interfaceType)) {
                                        if (itemToBeRegistered.DecoratedTypes == null) {
                                            itemToBeRegistered.DecoratedTypes = new List<Type>();
                                        }

                                        // Add to the collection of interfaces that are decorated only if this interface type has not previously been added
                                        if (!itemToBeRegistered.DecoratedTypes.Contains(interfaceType)) {
                                            itemToBeRegistered.DecoratedTypes.Add(interfaceType);
                                        }
                                    } 
                                }
                            }
                        }
                        
                        itemsToBeRegistered.Enqueue(new ItemToBeRegistered {Item = item, InterfaceTypes = GetInterfacesFromBlueprint(item), DecoratingTypes = decoratingTypes, IsEventHandler = isEventHandler});
                    }

                    foreach (var itemToBeRegistered in itemsToBeRegistered) {
                        var registration = RegisterType(builder, itemToBeRegistered.Item)
                            .EnableDynamicProxy(dynamicProxyContext)
                            .InstancePerLifetimeScope();

                        foreach (var interfaceType in itemToBeRegistered.InterfaceTypes) {
                            var registrationName = registration.ActivatorData.ImplementationType.FullName;

                            var interfaceName = interfaceType.FullName; //todo: remove this on tidyup

                            registration = registration.Named(registrationName, interfaceType);
                            registration = SetRegistrationScope(interfaceType, registration);

                            if (!itemToBeRegistered.IsDecorator(interfaceType) && !itemToBeRegistered.IsDecorated(interfaceType)) {
                                // This service is neither a decorator nor is decorated.
                                // It should be registered as an implementation if this interface type.
                                registration = registration.As(interfaceType);
                            }

                            if (!itemToBeRegistered.IsDecorator(interfaceType)) {
                                // This service is not a decorator for this interface type,
                                // so should be added to the list of services that implement this interface type
                                AddRegistrationName(registrationName, interfaceType);
                            }
                            else {
                                // This service is a decorator for this interface type

                                // We need to ensure that there is an implentation of this service that can be decorated
                                if (!_registrationNames.ContainsKey(interfaceType) || _registrationNames[interfaceType] == null || !_registrationNames[interfaceType].Any()) {
                                    var exception = new OrchardFatalException(T("The only registered implementations of `{0}` are decorators. In order to avoid circular dependenices, there must be at least one implementation that is not marked with the `OrchardDecorator` attribute.", interfaceType.FullName));
                                    Logger.Fatal(exception, "Could not complete dependency registration as a circular dependency chain has been found.");

                                    throw exception;
                                }

                                var decoratorNames = new ConcurrentBag<string>();

                                // For every implementation that can be decorated
                                foreach (var namedRegistration in _registrationNames[interfaceType]) {

                                    // Create a new registration
                                    var decoratorRegistration = RegisterType(builder, itemToBeRegistered.Item)
                                        .EnableDynamicProxy(dynamicProxyContext)
                                        .InstancePerLifetimeScope();

                                    // Create a unique name for the decorator
                                    var decoratorName = string.Format("{0}-{1}", namedRegistration, decoratorRegistration.ActivatorData.ImplementationType.FullName);
                                    decoratorRegistration = decoratorRegistration.Named(decoratorName, interfaceType);

                                    // Specify the named implementation to be injected as the decorated service
                                    decoratorRegistration = decoratorRegistration.WithParameter(
                                        (p, c) => p.ParameterType == interfaceType,
                                        (p, c) => c.ResolveNamed(namedRegistration, interfaceType));
                                    
                                    // Register the decorator as an implementation of this interface type
                                    decoratorRegistration = decoratorRegistration.As(interfaceType);

                                    // Set the scope of this implementation
                                    decoratorRegistration = SetRegistrationScope(interfaceType, decoratorRegistration);

                                    // Assign any custom parameters
                                    foreach (var parameter in itemToBeRegistered.Item.Parameters) {
                                        decoratorRegistration = decoratorRegistration
                                            .WithParameter(parameter.Name, parameter.Value)
                                            .WithProperty(parameter.Name, parameter.Value);
                                    }

                                    decoratorNames.Add(decoratorName);
                                }

                                // update the collection of implmentation names that can be decorated to contain only the decorators (this allows us to stack decorators)
                                _registrationNames[interfaceType] = decoratorNames;
                            }
                        }

                        if (itemToBeRegistered.IsEventHandler) {
                            var interfaces = itemToBeRegistered.Item.Type.GetInterfaces();
                            foreach (var interfaceType in interfaces) {

                                // register named instance for each interface, for efficient filtering inside event bus
                                // IEventHandler derived classes only
                                if (interfaceType.GetInterface(typeof(IEventHandler).Name) != null) {
                                    registration = registration.Named<IEventHandler>(interfaceType.Name);
                                }
                            }
                        }

                        foreach (var parameter in itemToBeRegistered.Item.Parameters) {
                            registration = registration
                                .WithParameter(parameter.Name, parameter.Value)
                                .WithProperty(parameter.Name, parameter.Value);
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

                    var optionalComponentsConfig = HostingEnvironment.MapPath("~/Config/HostComponents.config");
                    if (File.Exists(optionalComponentsConfig))
                        builder.RegisterModule(new HostComponentsConfigModule(optionalComponentsConfig));
                });
        }

        private IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType(ContainerBuilder builder, ShellBlueprintItem item) {
            return builder.RegisterType(item.Type)
                .WithProperty("Feature", item.Feature)
                .WithMetadata("Feature", item.Feature);
        }

        private IEnumerable<Type> GetInterfacesFromBlueprint(DependencyBlueprint blueprint) {
            return blueprint.Type.GetInterfaces()
                .Where(itf => typeof(IDependency).IsAssignableFrom(itf)
                            && !typeof(IEventHandler).IsAssignableFrom(itf));
        }

        private IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> SetRegistrationScope(Type interfaceType, IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> decoratorRegistration) {
            if (typeof (ISingletonDependency).IsAssignableFrom(interfaceType)) {
                decoratorRegistration = decoratorRegistration.InstancePerMatchingLifetimeScope("shell");
            }
            else if (typeof (IUnitOfWorkDependency).IsAssignableFrom(interfaceType)) {
                decoratorRegistration = decoratorRegistration.InstancePerMatchingLifetimeScope("work");
            }
            else if (typeof (ITransientDependency).IsAssignableFrom(interfaceType)) {
                decoratorRegistration = decoratorRegistration.InstancePerDependency();
            }
            return decoratorRegistration;
        }

        private void AddRegistrationName(string registrationName, Type interfaceType) {
            if (_registrationNames.ContainsKey(interfaceType)
                && _registrationNames[interfaceType] != null
                && !_registrationNames[interfaceType].Contains(registrationName)) {
                _registrationNames[interfaceType].Add(registrationName);
            }
            else {
                _registrationNames[interfaceType] = new ConcurrentBag<string> {registrationName};
            }
        }
        
        private class ItemToBeRegistered {
            public DependencyBlueprint Item { get; set; }
            public IEnumerable<Type> InterfaceTypes { get; set; }
            /// <summary>
            /// The types that this item decorates
            /// </summary>
            public IEnumerable<Type> DecoratingTypes { get; set; }
            /// <summary>
            /// The types that this item implements that are decorated by another item
            /// </summary>
            public IList<Type> DecoratedTypes { get; set; }
            public bool IsEventHandler { get; set; }

            public bool IsDecorated(Type interfaceType) {
                return DecoratedTypes != null && DecoratedTypes.Contains(interfaceType);
            }
            public bool IsDecorator(Type interfaceType) {
                return DecoratingTypes != null && DecoratingTypes.Contains(interfaceType);
            }
        }
    }
}