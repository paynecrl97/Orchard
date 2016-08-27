using System.Web.Routing;
using Orchard.ContentManagement.Handlers;

namespace Orchard.Core.Contents.Handlers {
    public class ContentsHandler : ContentHandlerBase {
        public override void GetContentItemMetadata(GetContentItemMetadataContext context) {
            if (context.Metadata.CreateRouteValues == null) {
                context.Metadata.CreateRouteValues = new RouteValueDictionary {
                    {"Area", "Contents"},
                    {"Controller", "Admin"},
                    {"Action", "Create"},
                    {"Id", context.ContentItem.ContentType}
                };
            }
            if (context.Metadata.EditorRouteValues == null) {
                context.Metadata.EditorRouteValues = new RouteValueDictionary {
                    {"Area", "Contents"},
                    {"Controller", "Admin"},
                    {"Action", "Edit"},
                    {"Id", context.ContentItem.Id}
                };
            }
            if (context.Metadata.DisplayRouteValues == null) {
                context.Metadata.DisplayRouteValues = new RouteValueDictionary {
                    {"Area", "Contents"},
                    {"Controller", "Item"},
                    {"Action", "Display"},
                    {"Id", context.ContentItem.Id}
                };
            }
            if (context.Metadata.PreviewRouteValues == null) {
                context.Metadata.PreviewRouteValues = new RouteValueDictionary {
                    {"Area", "Contents"},
                    {"Controller", "Item"},
                    {"Action", "Preview"},
                    {"Id", context.ContentItem.Id},
                    {"Version", context.ContentItem.VersionRecord.Number}
                };

                if (!string.IsNullOrEmpty(context.ContentItem.VersionRecord.AccessToken)) { 
                    context.Metadata.PreviewRouteValues.Add("AccessToken", context.ContentItem.VersionRecord.AccessToken);
                }
            }
            if (context.Metadata.RemoveRouteValues == null) {
                context.Metadata.RemoveRouteValues = new RouteValueDictionary {
                    {"Area", "Contents"},
                    {"Controller", "Admin"},
                    {"Action", "Remove"},
                    {"Id", context.ContentItem.Id}
                };
            }
        }
    }
}