using System.Collections.Generic;
using Orchard.Caching.Services;
using Orchard.Environment.Extensions;
using Orchard.Glimpse.AlternateImplementation;

namespace Orchard.Glimpse.AutofacModules {
    [OrchardFeature(FeatureNames.Cache)]
    public class CacheServiceDecoratorModule : DecoratorsModuleBase {

        protected override IEnumerable<DecorationConfiguration> DescribeDecorators() {
            return new[] 
            {
                DecorationConfiguration.Create<ICacheService, GlimpseCacheServiceDecorator>()
            };
        }

    }
}