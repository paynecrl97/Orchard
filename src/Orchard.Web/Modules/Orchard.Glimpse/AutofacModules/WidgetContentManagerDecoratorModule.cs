using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Glimpse.AlternateImplementation;

namespace Orchard.Glimpse.AutofacModules {
    [OrchardFeature(FeatureNames.Widgets)]
    public class WidgetContentManagerDecoratorModule : DecoratorsModuleBase {

        protected override IEnumerable<DecorationConfiguration> DescribeDecorators() {
            return new[] 
            {
                DecorationConfiguration.Create<IContentManager, GlimpseWidgetContentManagerDecorator>()
            };
        }

    }
}