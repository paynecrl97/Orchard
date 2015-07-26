using System.Collections.Generic;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.Environment.Extensions;
using Orchard.Glimpse.Services;
using Orchard.Glimpse.Tabs.FieldDrivers;

namespace Orchard.Glimpse.AlternateImplementation
{
    [OrchardDecorator]
    [OrchardFeature(FeatureNames.Fields)]
    public class GlimpseContentFieldDriverDecorator : IContentFieldDriver {
        private readonly IContentFieldDriver _decoratedService;
        private readonly IGlimpseService _glimpseService;

        public GlimpseContentFieldDriverDecorator(IContentFieldDriver decoratedService, IGlimpseService glimpseService) {
            _decoratedService = decoratedService;
            _glimpseService = glimpseService;
        }

        public DriverResult BuildDisplayShape(BuildDisplayContext context) {
            var result = _glimpseService.PublishTimedAction(() => _decoratedService.BuildDisplayShape(context), (r, t) => {
                    string stereotype;
                    if (!context.ContentItem.TypeDefinition.Settings.TryGetValue("Stereotype", out stereotype)) {
                        stereotype = "Content";
                    }

                    return new FieldDriverMessage {
                        Stereotype = stereotype,
                        Context = context,
                        Duration = t.Duration
                    };
                },
                TimelineCategories.Layers, "Build Display");

            return result.ActionResult;
        }

        public DriverResult BuildEditorShape(BuildEditorContext context) {
            return _decoratedService.BuildEditorShape(context);
        }

        public DriverResult UpdateEditorShape(UpdateEditorContext context) {
            return _decoratedService.UpdateEditorShape(context);
        }

        public void Importing(ImportContentContext context) {
            _decoratedService.Importing(context);
        }

        public void Imported(ImportContentContext context) {
            _decoratedService.Imported(context);
        }

        public void Exporting(ExportContentContext context) {
            _decoratedService.Exporting(context);
        }

        public void Exported(ExportContentContext context) {
            _decoratedService.Exported(context);
        }

        public void Describe(DescribeMembersContext context) {
            _decoratedService.Describe(context);
        }

        public IEnumerable<ContentFieldInfo> GetFieldInfo() {
            return _decoratedService.GetFieldInfo();
        }

        public void GetContentItemMetadata(GetContentItemMetadataContext context) {
            _decoratedService.GetContentItemMetadata(context);
        }
    }
}