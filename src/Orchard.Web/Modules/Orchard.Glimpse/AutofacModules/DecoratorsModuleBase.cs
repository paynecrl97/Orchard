using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;

//Credit to LombiqTechnologies for this class- it's taken from their Helpful Libraries module
namespace Orchard.Glimpse.AutofacModules {
    /// <summary>
    /// A base class for an Autofac module that registers decorators for other services. 
    /// </summary>
    public abstract class DecoratorsModuleBase : Module {
        private IEnumerable<DecorationConfiguration> _decorationConfigurations;

        protected abstract IEnumerable<DecorationConfiguration> DescribeDecorators();

        protected override void Load(ContainerBuilder builder) {
            foreach (var configuration in GetDecorationConfigurations()) {
                builder.RegisterType(configuration.DecoratorType).AsSelf().InstancePerDependency();
            }
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration) {
            foreach (var configuration in GetDecorationConfigurations()) {
                if (configuration.DecoratedType.IsAssignableFrom(registration.Activator.LimitType) && registration.Activator.LimitType != configuration.DecoratorType) {
                    var scopedConfiguration = configuration;
                    registration.Activating += (sender, e) =>
                    {
                        // This is needed so e.g. the Localizer and Logger can get registered.
                        registration.RaiseActivated(e.Context, e.Parameters, e.Instance);

                        e.Instance = e.Context.Resolve(scopedConfiguration.DecoratorType, new TypedParameter(scopedConfiguration.DecoratedType, e.Instance));
                    };
                }
            }
        }


        private IEnumerable<DecorationConfiguration> GetDecorationConfigurations() {
            return _decorationConfigurations ?? (_decorationConfigurations = DescribeDecorators());
        }


        protected class DecorationConfiguration {
            public Type DecoratedType { get; private set; }
            public Type DecoratorType { get; private set; }

            public DecorationConfiguration(Type decoratedType, Type decoratorType) {
                DecoratedType = decoratedType;
                DecoratorType = decoratorType;
            }

            public static DecorationConfiguration Create<TDecorated, TDecorator>() {
                return new DecorationConfiguration(typeof(TDecorated), typeof(TDecorator));
            }
        }
    }
}