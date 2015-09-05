//using System.Collections.Generic;
//using Orchard.ContentManagement.Drivers;
//using Orchard.ContentManagement.Handlers;
//using Orchard.ContentManagement.MetaData;
//using Orchard.Environment.Extensions;
//using Orchard.Glimpse.Services;
//using Orchard.Glimpse.Tabs.PartDrivers;

//namespace Orchard.Glimpse.AlternateImplementation
//{
//    [OrchardDecorator]
//    [OrchardFeature(FeatureNames.Parts)]
//    public class GlimpseContentPartDriverDecorator : IContentPartDriver {
//        private readonly IContentPartDriver _decoratedService;
//        private readonly IGlimpseService _glimpseService;

//        public GlimpseContentPartDriverDecorator(IContentPartDriver decoratedService, IGlimpseService glimpseService) {
//            _decoratedService = decoratedService;
//            _glimpseService = glimpseService;
//        }

//        public DriverResult BuildDisplay(BuildDisplayContext context) {
//            var result = _glimpseService.Time(() => _decoratedService.BuildDisplay(context));

//            if (result.ActionResult != null)
//            {
//                string stereotype;
//                if (!context.ContentItem.TypeDefinition.Settings.TryGetValue("Stereotype", out stereotype))
//                {
//                    stereotype = "Content";
//                }

//                var message = new PartDriverMessage
//                {
//                    Stereotype = stereotype,
//                    Context = context,
//                    Duration = result.TimerResult.Duration
//                };

//                _glimpseService.PublishMessage(message);
//                    //TimelineCategories.Layers, "Build Display");
//            }

//            return result.ActionResult;
//        }

//        public DriverResult BuildEditor(BuildEditorContext context) {
//            return _decoratedService.BuildEditor(context);
//        }

//        public DriverResult UpdateEditor(UpdateEditorContext context) {
//            return _decoratedService.UpdateEditor(context);
//        }

//        public void Importing(ImportContentContext context) {
//            _decoratedService.Importing(context);
//        }

//        public void Imported(ImportContentContext context) {
//            _decoratedService.Imported(context);
//        }

//        public void Exporting(ExportContentContext context) {
//            _decoratedService.Exporting(context);
//        }

//        public void Exported(ExportContentContext context) {
//            _decoratedService.Exported(context);
//        }

//        public IEnumerable<ContentPartInfo> GetPartInfo() {
//            return _decoratedService.GetPartInfo();
//        }

//        public void GetContentItemMetadata(GetContentItemMetadataContext context) {
//            _decoratedService.GetContentItemMetadata(context);
//        }
//    }
//}