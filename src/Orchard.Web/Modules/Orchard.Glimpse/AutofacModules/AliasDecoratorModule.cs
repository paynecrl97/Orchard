using System.Collections.Generic;
using Orchard.Environment.Extensions;
using Orchard.Glimpse.AlternateImplementation;
using Orchard.Mvc.Routes;

namespace Orchard.Glimpse.AutofacModules {
    [OrchardFeature(FeatureNames.Routes)]
    public class AliasDecoratorModule : DecoratorsModuleBase {

        protected override IEnumerable<DecorationConfiguration> DescribeDecorators() {
            return new[] 
            {
                DecorationConfiguration.Create<IRoutePublisher, GlimpseRoutePublisherDecorator>()
            };
        }

    }
}