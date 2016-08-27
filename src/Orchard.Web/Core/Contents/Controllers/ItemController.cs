using System;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Themes;

namespace Orchard.Core.Contents.Controllers {
    [Themed]
    public class ItemController : Controller {
        private readonly IContentManager _contentManager;
        private readonly IHttpContextAccessor _hca;

        public ItemController(IContentManager contentManager,
            IShapeFactory shapeFactory,
            IOrchardServices services,
            IHttpContextAccessor hca) {
            _contentManager = contentManager;
            _hca = hca;
            Shape = shapeFactory;
            Services = services;
            T = NullLocalizer.Instance;
        }

        dynamic Shape { get; set; }
        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }

        // /Contents/Item/Display/72
        public ActionResult Display(int? id, int? version) {
            if (id == null)
                return HttpNotFound();

            if (version.HasValue)
                return Preview(id, version, null);

            var contentItem = _contentManager.Get(id.Value, VersionOptions.Published);

            if (contentItem == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.ViewContent, contentItem, T("Cannot view content"))) {
                return new HttpUnauthorizedResult();
            }
            
            var model = _contentManager.BuildDisplay(contentItem);
            if (_hca.Current().Request.IsAjaxRequest()) {
                return new ShapePartialResult(this,model);
            }

            return View(model);
        }

        // /Contents/Item/Preview/72
        // /Contents/Item/Preview/72?version=5
        // /Contents/Item/Preview/72?version=5&accessToken=tokenxyz
        public ActionResult Preview(int? id, int? version, string accessToken) {
            if (id == null)
                return HttpNotFound();

            var versionOptions = VersionOptions.Latest;

            if (version != null)
                versionOptions = VersionOptions.Number((int)version);

            var contentItem = _contentManager.Get(id.Value, versionOptions);
            if (contentItem == null)
                return HttpNotFound();

            if (!AuthorizeContentPreview(contentItem, accessToken)) {
                return new HttpUnauthorizedResult();
            }

            var model = _contentManager.BuildDisplay(contentItem);
            if (_hca.Current().Request.IsAjaxRequest()) {
                return new ShapePartialResult(this, model);
            }

            return View(model);
        }

        private bool AuthorizeContentPreview(ContentItem contentItem, string accessToken) {
            if(Services.Authorizer.Authorize(Permissions.PreviewContent, contentItem)) {
                return true;
            }

            return Services.ContentManager.ValidateAccessToken(contentItem, accessToken);
        }
    }
}