using System.Collections.Generic;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;

namespace Orchard.Glimpse.AlternateImplementation
{
    public class GlimpseContentPartDriver : IDecorator<IContentPartDriver>, IContentPartDriver
    {
        private readonly IContentPartDriver _decoratedService;

        public GlimpseContentPartDriver(IContentPartDriver decoratedService) {
            _decoratedService = decoratedService;
        }

        public DriverResult BuildDisplay(BuildDisplayContext context) {
            return _decoratedService.BuildDisplay(context);
        }

        public DriverResult BuildEditor(BuildEditorContext context) {
            return _decoratedService.BuildEditor(context);
        }

        public void Exported(ExportContentContext context) {
            _decoratedService.Exported(context);
        }

        public void Exporting(ExportContentContext context) {
            _decoratedService.Exporting(context);
        }

        public void GetContentItemMetadata(GetContentItemMetadataContext context) {
            _decoratedService.GetContentItemMetadata(context);
        }

        public IEnumerable<ContentPartInfo> GetPartInfo() {
            return _decoratedService.GetPartInfo();
        }

        public void Imported(ImportContentContext context) {
            _decoratedService.Imported(context);
        }

        public void Importing(ImportContentContext context) {
            _decoratedService.Importing(context);
        }

        public DriverResult UpdateEditor(UpdateEditorContext context) {
            return _decoratedService.UpdateEditor(context);
        }
    }
}