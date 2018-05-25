using Nwazet.Commerce.Extensions;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Permissions;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Contents.Settings;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.Security.Permissions;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nwazet.Commerce.Controllers {
    [OrchardFeature("Territories")]
    [Admin]
    [ValidateInput(false)]
    public class TerritoryHierarchiesAdminController : Controller, IUpdateModel {

        private readonly ISiteService _siteService;
        private readonly ITerritoriesService _territoriesService;
        private readonly IContentManager _contentManager;
        private readonly IAuthorizer _authorizer;
        private readonly ITransactionManager _transactionManager;
        private readonly INotifier _notifier;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public TerritoryHierarchiesAdminController(
            ISiteService siteService,
            ITerritoriesService territoriesService,
            IShapeFactory shapeFactory,
            IContentManager contentManager,
            IAuthorizer authorizer,
            ITransactionManager transactionManager,
            INotifier notifier,
            IContentDefinitionManager contentDefinitionManager) {

            _siteService = siteService;
            _territoriesService = territoriesService;
            _contentManager = contentManager;
            _authorizer = authorizer;
            _transactionManager = transactionManager;
            _notifier = notifier;
            _contentDefinitionManager = contentDefinitionManager;

            _shapeFactory = shapeFactory;

            T = NullLocalizer.Instance;

            _allowedHierarchyTypes = new Lazy<IEnumerable<ContentTypeDefinition>>(GetAllowedHierarchyTypes);
        }

        public Localizer T;
        dynamic _shapeFactory;

        #region Manage the contents for hierarches
        /// <summary>
        /// This is the entry Action to the section to manage hierarchies of territories. 
        /// From here, users will not directly go and handle the records with the unique territory definitions.
        /// </summary>
        [HttpGet]
        public ActionResult Index(PagerParameters pagerParameters) {
            if (AllowedHierarchyTypes == null) {
                return new HttpUnauthorizedResult(TerritoriesUtilities.Default401HierarchyMessage);
            }

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            HierarchyAdminIndexViewModel model;
            if (AllowedHierarchyTypes.Any()) {
                var typeNames = AllowedHierarchyTypes.Select(ctd => ctd.Name).ToArray();

                var hierarchies = _territoriesService
                    .GetHierarchiesQuery(typeNames)
                    .Slice(pager.GetStartIndex(), pager.PageSize);

                var pagerShape = _shapeFactory
                    .Pager(pager)
                    .TotalItemCount(_territoriesService.GetHierarchiesQuery(typeNames).Count());

                var entries = hierarchies
                    .Select(CreateEntry)
                    .ToList();

                model = new HierarchyAdminIndexViewModel {
                    HierarchyEntries = entries,
                    AllowedHierarchyTypes = AllowedHierarchyTypes.ToList(),
                    Pager = pagerShape
                };
            } else {
                //No ContentType has been defined that contains the TerritoryHierarchyPart
                var pagerShape = _shapeFactory
                    .Pager(pager)
                    .TotalItemCount(0);

                model = new HierarchyAdminIndexViewModel {
                    HierarchyEntries = new List<HierarchyIndexEntry>(),
                    AllowedHierarchyTypes = AllowedHierarchyTypes.ToList(),
                    Pager = pagerShape
                };
                //For now we handle this by simply pointing out that the user should create types
                AddModelError("", T("There are no Hierarchy types that the user is allowed to manage."));
            }

            return View(model);
        }

        #region Creation
        [HttpGet]
        public ActionResult CreateHierarchy(string id) {
            //id is the Name of the ContentType we are trying to create. Calling that id allows us to use standard 
            //MVC routing (i.e. controller/action/id?querystring). This is especially nice for the post calls.
            if (AllowedHierarchyTypes == null) {
                return new HttpUnauthorizedResult(TerritoriesUtilities.Default401HierarchyMessage);
            }

            if (!AllowedHierarchyTypes.Any()) { //nothing to do
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrWhiteSpace(id)) { //specific type requested
                var typeDefinition = AllowedHierarchyTypes.FirstOrDefault(ctd => ctd.Name == id);
                if (typeDefinition != null) {
                    return CreateHierarchy(typeDefinition);
                }
            }
            if (AllowedHierarchyTypes.Count() == 1) {
                return CreateHierarchy(AllowedHierarchyTypes.FirstOrDefault());
            } else {
                return CreatableHierarchiesList();
            }
        }

        [HttpPost, ActionName("CreateHierarchy")]
        [Orchard.Mvc.FormValueRequired("submit.Save")]
        public ActionResult CreateHierarchyPost(string id, string returnUrl) {
            return CreateHierarchyPost(id, returnUrl, contentItem => {
                if (!contentItem.Has<IPublishingControlAspect>() && !contentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable) {
                    _contentManager.Publish(contentItem);
                }
            });
        }

        [HttpPost, ActionName("CreateHierarchy")]
        [Orchard.Mvc.FormValueRequired("submit.Publish")]
        public ActionResult CreateAndPublishHierarchyPost(string id, string returnUrl) {
            var dummyContent = _contentManager.New(id);

            if (!_authorizer.Authorize(Orchard.Core.Contents.Permissions.PublishContent, dummyContent, TerritoriesUtilities.Creation401HierarchyMessage))
                return new HttpUnauthorizedResult();

            return CreateHierarchyPost(id, returnUrl, contentItem => _contentManager.Publish(contentItem));
        }

        private ActionResult CreateHierarchy(ContentTypeDefinition typeDefinition) {
            if (AllowedHierarchyTypes == null) {
                return new HttpUnauthorizedResult(TerritoriesUtilities.Default401HierarchyMessage);
            }
            if (!AllowedHierarchyTypes.Any(ty => ty.Name == typeDefinition.Name)) {
                return new HttpUnauthorizedResult(TerritoriesUtilities.SpecificHierarchy401Message(typeDefinition.DisplayName));
            }
            if (!typeDefinition.Parts.Any(pa => pa.PartDefinition.Name == TerritoryHierarchyPart.PartName)) {
                AddModelError("", T("The requested type \"{0}\" is not a Hierarchy type.", typeDefinition.DisplayName));
                return RedirectToAction("Index");
            }
            //We should have filtered out the cases where we cannot or should not be creating the new item here
            var hierarchyItem = _contentManager.New(typeDefinition.Name);
            var model = _contentManager.BuildEditor(hierarchyItem);
            return View(model);
        }

        private ActionResult CreatableHierarchiesList() {
            if (AllowedHierarchyTypes == null) {
                return new HttpUnauthorizedResult(TerritoriesUtilities.Default401HierarchyMessage);
            }
            //This will be like the AdminController from Orchard.Core.Contents
            var viewModel = _shapeFactory.ViewModel(HierarchyTypes: AllowedHierarchyTypes);

            return View("CreatableTypeList", viewModel);
        }

        private ActionResult CreateHierarchyPost(string typeName, string returnUrl, Action<ContentItem> conditionallyPublish) {
            return ExecuteHierarchyPost(new TerritoriesAdminHierarchyExecutionContext {
                HierarchyItem = _contentManager.New(typeName),
                Message = TerritoriesUtilities.Creation401HierarchyMessage,
                AdditionalPermissions = new Permission[] { Orchard.Core.Contents.Permissions.EditContent },
                ExecutionAction = item => {
                    _contentManager.Create(item, VersionOptions.Draft);

                    var model = _contentManager.UpdateEditor(item, this);

                    if (!ModelState.IsValid) {
                        _transactionManager.Cancel();
                        return View(model);
                    }

                    conditionallyPublish(item);

                    _notifier.Information(string.IsNullOrWhiteSpace(item.TypeDefinition.DisplayName)
                        ? T("Your content has been created.")
                        : T("Your {0} has been created.", item.TypeDefinition.DisplayName));

                    return this.RedirectLocal(returnUrl, () => 
                        RedirectToAction("EditHierarchy", new RouteValueDictionary { { "Id", item.Id } }));
                }
            });
        }

        #endregion

        #region Edit
        [HttpGet]
        public ActionResult EditHierarchy(int id) {
            if (AllowedHierarchyTypes == null) {
                return new HttpUnauthorizedResult(TerritoriesUtilities.Default401HierarchyMessage);
            }

            var hierarchyItem = _contentManager.Get(id, VersionOptions.Latest);

            if (hierarchyItem == null)
                return HttpNotFound();

            var typeName = hierarchyItem.ContentType;
            var typeDefinition = _contentDefinitionManager.GetTypeDefinition(typeName);
            if (!typeDefinition.Parts.Any(pa => pa.PartDefinition.Name == TerritoryHierarchyPart.PartName)) {
                AddModelError("", T("The requested type \"{0}\" is not a Hierarchy type.", typeDefinition.DisplayName));
                return RedirectToAction("Index");
            }
            typeDefinition = AllowedHierarchyTypes.FirstOrDefault(ctd => ctd.Name == typeName);

            if (typeDefinition == null) {
                return new HttpUnauthorizedResult(TerritoriesUtilities.SpecificHierarchy401Message(typeName));
            }

            if (!_authorizer.Authorize(Orchard.Core.Contents.Permissions.EditContent, hierarchyItem, TerritoriesUtilities.Edit401HierarchyMessage))
                return new HttpUnauthorizedResult();

            //We should have filtered out the cases where we cannot or should not be editing the item here
            var model = _contentManager.BuildEditor(hierarchyItem);
            return View(model);
        }

        [HttpPost, ActionName("EditHierarchy")]
        [Orchard.Mvc.FormValueRequired("submit.Save")]
        public ActionResult EditHierarchyPost(int id, string returnUrl) {
            return EditHierarchyPost(id, returnUrl, contentItem => {
                if (!contentItem.Has<IPublishingControlAspect>() && !contentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable)
                    _contentManager.Publish(contentItem);
            });
        }

        [HttpPost, ActionName("EditHierarchy")]
        [Orchard.Mvc.FormValueRequired("submit.Publish")]
        public ActionResult EditAndPublishHierarchyPost(int id, string returnUrl) {
            var content = _contentManager.Get(id, VersionOptions.Latest);

            if (content == null)
                return HttpNotFound();

            if (!_authorizer.Authorize(Orchard.Core.Contents.Permissions.PublishContent, content, TerritoriesUtilities.Edit401HierarchyMessage))
                return new HttpUnauthorizedResult();

            return EditHierarchyPost(id, returnUrl, contentItem => _contentManager.Publish(contentItem));
        }

        private ActionResult EditHierarchyPost(int id, string returnUrl, Action<ContentItem> conditionallyPublish) {
            return ExecuteHierarchyPost(new TerritoriesAdminHierarchyExecutionContext {
                HierarchyItem = _contentManager.Get(id, VersionOptions.DraftRequired),
                Message = TerritoriesUtilities.Edit401HierarchyMessage,
                AdditionalPermissions = new Permission[] { Orchard.Core.Contents.Permissions.EditContent },
                ExecutionAction = item => {
                    var model = _contentManager.UpdateEditor(item, this);

                    if (!ModelState.IsValid) {
                        _transactionManager.Cancel();
                        return View(model);
                    }

                    conditionallyPublish(item);

                    _notifier.Information(string.IsNullOrWhiteSpace(item.TypeDefinition.DisplayName)
                        ? T("Your content has been saved.")
                        : T("Your {0} has been saved.", item.TypeDefinition.DisplayName));

                    return this.RedirectLocal(returnUrl, () => RedirectToAction("EditHierarchy", new RouteValueDictionary { { "Id", item.Id } }));
                }
            });
        }

        #endregion

        [HttpPost]
        public ActionResult DeleteHierarchy(int id, string returnUrl) {
            return ExecuteHierarchyPost(new TerritoriesAdminHierarchyExecutionContext {
                HierarchyItem = _contentManager.Get(id, VersionOptions.Latest),
                Message = TerritoriesUtilities.Delete401HierarchyMessage,
                AdditionalPermissions = new Permission[] { Orchard.Core.Contents.Permissions.DeleteContent },
                ExecutionAction = item => {
                    if (item != null) {
                        _contentManager.Remove(item);
                        _notifier.Information(string.IsNullOrWhiteSpace(item.TypeDefinition.DisplayName)
                            ? T("That content has been removed.")
                            : T("That {0} has been removed.", item.TypeDefinition.DisplayName));
                    }

                    return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
                }
            });
        }

        private ActionResult ExecuteHierarchyPost(
            TerritoriesAdminHierarchyExecutionContext context) {
            var hierarchyItem = context.HierarchyItem;
            if (hierarchyItem == null)
                return HttpNotFound();

            #region Authorize
            if (AllowedHierarchyTypes == null) {
                return new HttpUnauthorizedResult(TerritoriesUtilities.Default401HierarchyMessage);
            }
            var typeName = hierarchyItem.ContentType;
            var typeDefinition = _contentDefinitionManager.GetTypeDefinition(typeName);
            if (!typeDefinition.Parts.Any(pa => pa.PartDefinition.Name == TerritoryHierarchyPart.PartName)) {
                AddModelError("", T("The requested type \"{0}\" is not a Hierarchy type.", typeDefinition.DisplayName));
                return RedirectToAction("Index");
            }
            typeDefinition = AllowedHierarchyTypes.FirstOrDefault(ctd => ctd.Name == typeName);
            if (typeDefinition == null) {
                return new HttpUnauthorizedResult(TerritoriesUtilities.SpecificHierarchy401Message(typeName));
            }

            if (!_authorizer.Authorize(TerritoriesPermissions.ManageTerritoryHierarchies, hierarchyItem, context.Message))
                return new HttpUnauthorizedResult();

            foreach (var permission in context.AdditionalPermissions) {
                if (!_authorizer.Authorize(permission, hierarchyItem, context.Message))
                    return new HttpUnauthorizedResult();
            }
            #endregion

            return context.ExecutionAction(hierarchyItem);
        }
        #endregion


        private HierarchyIndexEntry CreateEntry(TerritoryHierarchyPart part) {
            var hierarchyType = _contentDefinitionManager.GetTypeDefinition(part.ContentItem.ContentType);
            var typeDisplayName = hierarchyType != null ?
                hierarchyType.DisplayName : T("ERROR: impossible to find hierarchy type.").Text;

            var territoryType = _contentDefinitionManager.GetTypeDefinition(part.TerritoryType);
            var territoryDisplayName = territoryType != null ?
                territoryType.DisplayName : T("ERROR: impossible to find territory type.").Text;

            return new HierarchyIndexEntry {
                Id = part.ContentItem.Id,
                DisplayText = _contentManager.GetItemMetadata(part.ContentItem).DisplayText,
                ContentItem = part.ContentItem,
                TypeDisplayName = typeDisplayName,
                IsDraft = !part.ContentItem.IsPublished(),
                TerritoriesCount = part.Territories.Count(),
                TerritoryTypeDisplayName = territoryDisplayName
            };
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
