using System.Collections.Generic;
using Orchard.Environment.Extensions;
using Orchard.Glimpse.AlternateImplementation;
using Orchard.Security;

namespace Orchard.Glimpse.AutofacModules {
    [OrchardFeature(FeatureNames.Authorizer)]
    public class AuthorizerDecoratorModule : DecoratorsModuleBase {

        protected override IEnumerable<DecorationConfiguration> DescribeDecorators() {
            return new[] 
            {
                DecorationConfiguration.Create<IAuthorizer, GlimpseAuthorizerDecorator>()
            };
        }

    }
}