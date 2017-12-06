using Nwazet.Commerce.Extensions;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Permissions;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Contents.Settings;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using Orchard.Mvc.Html;
using Orchard.Security;
using Orchard.Security.Permissions;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Orchard.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nwazet.Commerce.Controllers {
    [OrchardFeature("Territories")]
    [Admin]
    [ValidateInput(false)]
    public class HierarchyTerritoriesAdminController : Controller, IUpdateModel {

        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ITerritoriesService _territoriesService;
        private readonly IAuthorizer _authorizer;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly RouteCollection _routeCollection;
        private readonly ITerritoriesHierarchyService _territoriesHierarchyService;
        private readonly ITransactionManager _transactionManager;
        private readonly INotifier _notifier;
        private readonly IEnumerable<IContentHandler> _handlers;

        public HierarchyTerritoriesAdminController(
            IContentManager contentManager,
            IContentDefinitionManager contentDefinitionManager,
            ITerritoriesService territoriesService,
            IAuthorizer authorizer,
            IWorkContextAccessor workContextAccessor,
            RouteCollection routeCollection,
            ITerritoriesHierarchyService territoriesHierarchyService,
            ITransactionManager transactionManager,
            INotifier notifier,
            IEnumerable<IContentHandler> handlers) {

            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
            _territoriesService = territoriesService;
            _authorizer = authorizer;
            _workContextAccessor = workContextAccessor;
            _routeCollection = routeCollection;
            _territoriesHierarchyService = territoriesHierarchyService;
            _transactionManager = transactionManager;
            _notifier = notifier;
            _handlers = handlers;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;

            _allowedTerritoryTypes = new Lazy<IEnumerable<ContentTypeDefinition>>(GetAllowedTerritoryTypes);
            _allowedHierarchyTypes = new Lazy<IEnumerable<ContentTypeDefinition>>(GetAllowedHierarchyTypes);
        }

        public Localizer T;
        public ILogger Logger { get; set; }

        [HttpGet]
        public ActionResult Index(int id) {
            ActionResult redirectTo;
            if (ShouldRedirectForPermissions(id, out redirectTo)) {
                return redirectTo;
            }

            // list the first level of territories for the selected hierarchy
            // The null checks for these objects are done in ShouldRedirectForPermissions
            var hierarchyItem = _contentManager.Get(id, VersionOptions.Latest);
            var hierarchyPart = hierarchyItem.As<TerritoryHierarchyPart>();
            
            var firstLevelOfHierarchy = _territoriesService
                .GetTerritoriesQuery(hierarchyPart, null, VersionOptions.Latest)
                .List().ToList();
                       

            var model = new TerritoryHierarchyTerritoriesViewModel {
                HierarchyPart = hierarchyPart,
                HierarchyItem = hierarchyItem,
                FirstLevelNodes = firstLevelOfHierarchy.Select(MakeANode).ToList(),
                Nodes = _territoriesService.
                    GetTerritoriesQuery(hierarchyPart, VersionOptions.Latest)
                    .List().Select(MakeANode).ToList(),
                CanAddMoreTerritories = _territoriesService
                    .GetAvailableTerritoryInternals(hierarchyPart)
                    .Any()
            };

            return View(model);
        }

        [HttpPost, ActionName("Index")]
        public ActionResult IndexPost(IList<TerritoryHierarchyTreeNode> nodes, int? hierarchyId) {
            ActionResult redirectTo = HttpNotFound();
            if (!hierarchyId.HasValue || ShouldRedirectForPermissions(hierarchyId.Value, out redirectTo)) {
                return redirectTo;
            }
            var hierarchy = _contentManager.Get(hierarchyId.Value, VersionOptions.Latest);
            var hierarchyPart = hierarchy.As<TerritoryHierarchyPart>();

            if (nodes != null) {
                foreach (var node in nodes) {
                    // The only fields we receive as populated for the nodes are:
                    //  - node.Id: the Id of the ContentItem for the TerritoryPart
                    //  - node.ParentId: the Id of the Parent assigned to that node
                    var territoryPart = _contentManager.Get<TerritoryPart>(node.Id, VersionOptions.Latest);
                    try {
                        if (node.ParentId == 0) {
                            if (territoryPart.Parent != null) { // do not update if there was no change
                                // moved from a parent up to the root
                                UpdateTerritoryPosition(territoryPart, hierarchyPart);
                            }
                        } else {
                            if (territoryPart.Parent == null || // the territory was at root
                                territoryPart.Parent.Record.Id != node.ParentId) { // the territory had a different parent
                                var parentPart = _contentManager.Get<TerritoryPart>(node.ParentId, VersionOptions.Latest);
                                UpdateTerritoryPosition(territoryPart, hierarchyPart, parentPart);
                            }
                        }
                    } catch (Exception ex) {
                        AddModelError("Hierarchy", ex.Message);
                    }
                }
            }

            return RedirectToAction("Index", new { id = hierarchyId.Value });
        }

        [HttpPost]
        public ActionResult DeleteTerritory(int id, string returnUrl) {

            var territoryItem = _contentManager.Get(id, VersionOptions.Latest);
            var hierarchyId = territoryItem.As<TerritoryPart>().Hierarchy.Record.Id;
            return ExecuteTerritoryPost(new TerritoryExecutionContext {
                HierarchyItem = territoryItem.As<TerritoryPart>().Hierarchy,
                TerritoryItem = territoryItem,
                Message = TerritoriesUtilities.Delete401TerritoryMessage,
                AdditionalPermissions = new Permission[] { Orchard.Core.Contents.Permissions.DeleteContent },
                ExecutionAction = item => {
                    if (item != null) {
                        _contentManager.Remove(item);
                        _notifier.Information(string.IsNullOrWhiteSpace(item.TypeDefinition.DisplayName)
                            ? T("That content has been removed.")
                            : T("That {0} has been removed.", item.TypeDefinition.DisplayName));
                    }

                    return this.RedirectLocal(returnUrl, () => RedirectToAction("Index", new { id = hierarchyId }));
                }
            });
        }

        #region Create
        [HttpGet]
        public ActionResult CreateTerritory(string id, int hierarchyId) {
            // id is the name of the ContentType for the territory we are trying to create. By calling
            // that argument "id" we can use the standard MVC routing (i.e. controller/action/id?querystring).
            // This is especially nice on POST calls.
            ActionResult redirectTo;
            if (ShouldRedirectForPermissions(hierarchyId, out redirectTo)) {
                return redirectTo;
            }

            // The null checks for these objects are done in ShouldRedirectForPermissions
            var hierarchyItem = _contentManager.Get(hierarchyId, VersionOptions.Latest);
            var hierarchyPart = hierarchyItem.As<TerritoryHierarchyPart>();
            var hierarchyTitle = _contentManager.GetItemMetadata(hierarchyItem).DisplayText;

            if (!id.Equals(hierarchyPart.TerritoryType, StringComparison.OrdinalIgnoreCase)) {
                // The hierarchy expects a TerritoryType different form the one we are trying to create
                var errorText = string.IsNullOrWhiteSpace(hierarchyTitle) ?
                    T("The requested type \"{0}\" does not match the expected TerritoryType for the hierarchy.", id) :
                    T("The requested type \"{0}\" does not match the expected TerritoryType for hierarchy \"{1}\".", id, hierarchyTitle);
                AddModelError("", errorText);
                return RedirectToAction("Index");
            }

            // There must be "unused" TerritoryInternalRecords for this hierarchy.
            if (_territoriesService
                .GetAvailableTerritoryInternals(hierarchyPart)
                .Any()) {

                // Creation
                var territoryItem = _contentManager.New(id);
                // Cannot insert Territory in the Hierarchy here, because its records do not exist yet.
                // We will have to do it in the POST call.
                // Allow user to Edit stuff
                var model = _contentManager.BuildEditor(territoryItem);
                return View(model.Hierarchy(hierarchyItem));
            }

            AddModelError("", T("There are no territories that may be added to hierarchy \"{1}\".", hierarchyTitle));
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("CreateTerritory")]
        [Orchard.Mvc.FormValueRequired("submit.Save")]
        public ActionResult CreateTerritoryPost(string id, int hierarchyId, string returnUrl) {
            return CreateTerritoryPost(id, hierarchyId, returnUrl, contentItem => {
                if (!contentItem.Has<IPublishingControlAspect>() &&
                    !contentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable) {

                    _contentManager.Publish(contentItem);
                }
            });
        }

        [HttpPost, ActionName("CreateTerritory")]
        [Orchard.Mvc.FormValueRequired("submit.Publish")]
        public ActionResult CreateAndPublishTerritoryPost(string id, int hierarchyId, string returnUrl) {
            var dummyContent = _contentManager.New(id);

            if (!_authorizer.Authorize(
                Orchard.Core.Contents.Permissions.PublishContent, dummyContent, TerritoriesUtilities.Creation401TerritoryMessage))
                return new HttpUnauthorizedResult();

            return CreateTerritoryPost(id, hierarchyId, returnUrl, contentItem => _contentManager.Publish(contentItem));
        }

        private ActionResult CreateTerritoryPost(
            string typeName, int hierarchyId, string returnUrl, Action<ContentItem> conditionallyPublish) {

            return ExecuteTerritoryPost(new TerritoryExecutionContext {
                HierarchyItem = _contentManager.Get(hierarchyId, VersionOptions.Latest),
                TerritoryItem = _contentManager.New(typeName),
                Message = TerritoriesUtilities.Creation401TerritoryMessage,
                AdditionalPermissions = new Permission[] { Orchard.Core.Contents.Permissions.EditContent },
                ExecutionAction = item => {
                    _contentManager.Create(item, VersionOptions.Draft);

                    var model = _contentManager.UpdateEditor(item, this);

                    if (!ModelState.IsValid) {
                        _transactionManager.Cancel();
                        return View(model.Hierarchy(_contentManager.Get(hierarchyId, VersionOptions.Latest)));
                    }

                    var territoryPart = item.As<TerritoryPart>();
                    var hierachyPart = _contentManager.Get<TerritoryHierarchyPart>(hierarchyId, VersionOptions.Latest);

                    _territoriesHierarchyService.AddTerritory(territoryPart, hierachyPart);

                    conditionallyPublish(item);

                    _notifier.Information(string.IsNullOrWhiteSpace(item.TypeDefinition.DisplayName)
                        ? T("Your content has been created.")
                        : T("Your {0} has been created.", item.TypeDefinition.DisplayName));

                    return this.RedirectLocal(returnUrl, () => 
                        RedirectToAction("EditTerritory", 
                            new RouteValueDictionary { { "Id", item.Id } }));
                }
            });
        }

        #endregion

        #region Edit
        [HttpGet]
        public ActionResult EditTerritory(int id) {

            var territoryItem = _contentManager.Get(id, VersionOptions.Latest);
            if (territoryItem == null)
                return HttpNotFound();
            var territoryPart = territoryItem.As<TerritoryPart>();
            if (territoryPart == null)
                return HttpNotFound();

            ActionResult redirectTo;
            if (ShouldRedirectForPermissions(territoryPart.Record.Hierarchy.Id, out redirectTo)) {
                return redirectTo;
            }
            
            if (!_authorizer.Authorize(
                Orchard.Core.Contents.Permissions.EditContent, territoryItem, TerritoriesUtilities.Edit401TerritoryMessage))
                return new HttpUnauthorizedResult();

            // We should have filtered out the cases where we cannot or should not be editing the item here
            var model = _contentManager.BuildEditor(territoryItem);
            return View(model.Hierarchy(territoryItem.As<TerritoryPart>().Hierarchy));
        }

        [HttpPost, ActionName("EditTerritory")]
        [Orchard.Mvc.FormValueRequired("submit.Save")]
        public ActionResult EditTerritoryPost(int id, string returnUrl) {
            return EditTerritoryPost(id, returnUrl, contentItem => {
                if (!contentItem.Has<IPublishingControlAspect>() && 
                    !contentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable)
                    _contentManager.Publish(contentItem);
            });
        }

        [HttpPost, ActionName("EditTerritory")]
        [Orchard.Mvc.FormValueRequired("submit.Publish")]
        public ActionResult EditAndPublishTerritoryPost(int id, string returnUrl) {
            var content = _contentManager.Get(id, VersionOptions.Latest);

            if (content == null)
                return HttpNotFound();

            if (!_authorizer.Authorize(
                Orchard.Core.Contents.Permissions.PublishContent, content, TerritoriesUtilities.Edit401TerritoryMessage))
                return new HttpUnauthorizedResult();

            return EditTerritoryPost(id, returnUrl, contentItem => _contentManager.Publish(contentItem));
        }

        private ActionResult EditTerritoryPost(
            int id, string returnUrl, Action<ContentItem> conditionallyPublish) {

            var territoryItem = _contentManager.Get(id, VersionOptions.DraftRequired);
            return ExecuteTerritoryPost(new TerritoryExecutionContext {
                HierarchyItem = territoryItem.As<TerritoryPart>().Hierarchy,
                TerritoryItem = territoryItem,
                Message = TerritoriesUtilities.Edit401TerritoryMessage,
                AdditionalPermissions = new Permission[] { Orchard.Core.Contents.Permissions.EditContent },
                ExecutionAction = item => {
                    var model = _contentManager.UpdateEditor(item, this);

                    if (!ModelState.IsValid) {
                        _transactionManager.Cancel();
                        return View(model);
                    }

                    string previousRoute = null;
                    if (item.Has<IAliasAspect>()
                        && !string.IsNullOrWhiteSpace(returnUrl)
                        && Request.IsLocalUrl(returnUrl)
                        // only if the original returnUrl is the content itself
                        && String.Equals(returnUrl, Url.ItemDisplayUrl(item), StringComparison.OrdinalIgnoreCase)
                        ) {
                        previousRoute = item.As<IAliasAspect>().Path;
                    }

                    conditionallyPublish(item);

                    if (!string.IsNullOrWhiteSpace(returnUrl)
                        && previousRoute != null
                        && !String.Equals(item.As<IAliasAspect>().Path, previousRoute, StringComparison.OrdinalIgnoreCase)) {
                        returnUrl = Url.ItemDisplayUrl(item);
                    }

                    _notifier.Information(string.IsNullOrWhiteSpace(item.TypeDefinition.DisplayName)
                        ? T("Your content has been updated.")
                        : T("Your {0} has been updated.", item.TypeDefinition.DisplayName));

                    return this.RedirectLocal(returnUrl, () =>
                        RedirectToAction("EditTerritory",
                            new RouteValueDictionary { { "Id", item.Id } }));
                }
            });
        }
        #endregion

        private ActionResult ExecuteTerritoryPost(
            TerritoryExecutionContext context) {
            if (context.HierarchyItem == null || context.TerritoryItem == null) {
                return HttpNotFound();
            }
            var hierarchyPart = context.HierarchyItem.As<TerritoryHierarchyPart>();
            var territoryPart = context.TerritoryItem.As<TerritoryPart>();
            if (hierarchyPart == null || territoryPart == null) {
                return HttpNotFound();
            }

            #region Authorize
            ActionResult redirectTo;
            if (ShouldRedirectForPermissions(hierarchyPart.Record.Id, out redirectTo)) {
                return redirectTo;
            }
            foreach (var permission in context.AdditionalPermissions) {
                if (!_authorizer.Authorize(permission, context.TerritoryItem, context.Message))
                    return new HttpUnauthorizedResult();
            }
            #endregion

            return context.ExecutionAction(context.TerritoryItem);
        }

        /// <summary>
        /// This method performs a bunch of default checks to verify that the user is allowed to proceed
        /// with the action it called. This will return false if the user is authorized to proceed.
        /// </summary>
        /// <param name="hierarchyId">The Id of a hierarchy ContentItem.</param>
        /// <returns>Returns false if the caller is authorized to proceed. Otherwise the ou ActionResult
        /// argument is populated with the Action the user should be redirected to.</returns>
        private bool ShouldRedirectForPermissions(int hierarchyId, out ActionResult redirectTo) {
            redirectTo = null;
            if (AllowedHierarchyTypes == null) {
                redirectTo = new HttpUnauthorizedResult(TerritoriesUtilities.Default401HierarchyMessage);
                return true;
            }
            if (AllowedTerritoryTypes == null) {
                redirectTo = new HttpUnauthorizedResult(TerritoriesUtilities.Default401TerritoryMessage);
                return true;
            }

            var hierarchyItem = _contentManager.Get(hierarchyId, VersionOptions.Latest);
            if (hierarchyItem == null) {
                redirectTo = HttpNotFound();
                return true;
            }
            var hierarchyPart = hierarchyItem.As<TerritoryHierarchyPart>();
            if (hierarchyPart == null) {
                redirectTo = HttpNotFound();
                return true;
            }

            if (!AllowedHierarchyTypes.Any(ty => ty.Name == hierarchyItem.ContentType)) {
                var typeName = _contentDefinitionManager.GetTypeDefinition(hierarchyItem.ContentType).DisplayName;
                redirectTo = new HttpUnauthorizedResult(TerritoriesUtilities.SpecificHierarchy401Message(typeName));
                return true;
            }
            if (!AllowedTerritoryTypes.Any(ty => ty.Name == hierarchyPart.TerritoryType)) {
                var typeName = _contentDefinitionManager.GetTypeDefinition(hierarchyPart.TerritoryType).DisplayName;
                redirectTo = new HttpUnauthorizedResult(TerritoriesUtilities.SpecificTerritory401Message(typeName));
                return true;
            }

            return false;
        }

        private Lazy<IEnumerable<ContentTypeDefinition>> _allowedTerritoryTypes;
        private IEnumerable<ContentTypeDefinition> AllowedTerritoryTypes {
            get { return _allowedTerritoryTypes.Value; }
        }

        /// <summary>
        /// This method gets all the territory types the current user is allowed to manage.
        /// </summary>
        /// <returns>Returns the types the user is allowed to manage. Returns null if the user lacks the correct 
        /// permissions to be invoking these actions.</returns>
        private IEnumerable<ContentTypeDefinition> GetAllowedTerritoryTypes() {
            var allowedTypes = _territoriesService.GetTerritoryTypes();
            if (!allowedTypes.Any() && //no dynamic permissions
                !_authorizer.Authorize(TerritoriesPermissions.ManageTerritories)) {

                return null;
            }

            return allowedTypes;
        }

        private Lazy<IEnumerable<ContentTypeDefinition>> _allowedHierarchyTypes;
        private IEnumerable<ContentTypeDefinition> AllowedHierarchyTypes {
            get { return _allowedHierarchyTypes.Value; }
        }

        /// <summary>
        /// This method gets all the hierarchy types the current user is allowed to manage.
        /// </summary>
        /// <returns>Returns the types the user is allwoed to manage. Returns null if the user lacks the correct 
        /// permissions to be invoking these actions.</returns>
        private IEnumerable<ContentTypeDefinition> GetAllowedHierarchyTypes() {
            var allowedTypes = _territoriesService.GetHierarchyTypes();
            if (!allowedTypes.Any() && //no dynamic permissions
                !_authorizer.Authorize(TerritoriesPermissions.ManageTerritoryHierarchies)) {

                return null;
            }

            return allowedTypes;
        }

        private TerritoryHierarchyTreeNode MakeANode(TerritoryPart territoryPart) {
            var metadata = _contentManager.GetItemMetadata(territoryPart.ContentItem);
            var requestContext = _workContextAccessor.GetContext().HttpContext.Request.RequestContext;
            return new TerritoryHierarchyTreeNode {
                Id = territoryPart.ContentItem.Id,
                TerritoryItem = territoryPart.ContentItem,
                ParentId = territoryPart.Record.ParentTerritory == null ? 0 : territoryPart.Record.ParentTerritory.Id,
                EditUrl = _routeCollection.GetVirtualPath(requestContext, metadata.EditorRouteValues).VirtualPath,
                DisplayText = metadata.DisplayText + 
                    (!territoryPart.ContentItem.IsPublished() ? T(" (draft)").Text : string.Empty) +
                    ((territoryPart.Record.TerritoryInternalRecord == null) ? T(" (requires identity)").Text : string.Empty)
            };
        }

        private void UpdateTerritoryPosition(
            TerritoryPart territoryPart, TerritoryHierarchyPart hierarchyPart, TerritoryPart parentPart = null) {

            var context = new UpdateContentContext(territoryPart.ContentItem);
            _handlers.Invoke(handler => handler.Updating(context), Logger);
            if (parentPart == null) {
                _territoriesHierarchyService.AddTerritory(territoryPart, hierarchyPart); // move to root
            } else {
                _territoriesHierarchyService.AssignParent(territoryPart, parentPart);
            }
            _handlers.Invoke(handler => handler.Updated(context), Logger);
        }

        #region IUpdateModel implementation
        public void AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

        public void AddModelError(string key, string errorMessage) {
            ModelState.AddModelError(key, errorMessage);
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }
        #endregion
    }
}
