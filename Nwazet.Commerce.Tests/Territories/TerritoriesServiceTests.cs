using Autofac;
using Moq;
using NUnit.Framework;
using Nwazet.Commerce.Handlers;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Permissions;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Stubs;
using Nwazet.Commerce.Tests.Territories.Handlers;
using Orchard;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.Records;
using Orchard.Data;
using Orchard.Security;
using Orchard.Tests.Modules;
using Orchard.Tests.Stubs;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Nwazet.Commerce.Tests.Territories {
    [TestFixture]
    public class TerritoriesServiceTests : DatabaseEnabledTestsBase {

        private ITerritoriesService _territoriesService;
        private ITerritoriesRepositoryService _territoryRepositoryService;
        private ITerritoriesPermissionProvider _permissionProvider;
        private ITerritoriesHierarchyService _territoriesHierarchyService;
        private IContentManager _contentManager;
        private ITransactionManager _transactionManager;

        public override void Init() {
            base.Init();

            _territoriesService = _container.Resolve<ITerritoriesService>();
            _territoryRepositoryService = _container.Resolve<ITerritoriesRepositoryService>();
            _permissionProvider = _container.Resolve<ITerritoriesPermissionProvider>();
            _contentManager = _container.Resolve<IContentManager>();
            _territoriesHierarchyService = _container.Resolve<ITerritoriesHierarchyService>();
            _transactionManager = _container.Resolve<ITransactionManager>();
        }

        public override void Register(ContainerBuilder builder) {

            builder.RegisterType<TerritoriesService>().As<ITerritoriesService>();
            builder.RegisterType<TerritoryRepositoryService>().As<ITerritoriesRepositoryService>();
            builder.RegisterType<TerritoriesPermissions>().As<ITerritoriesPermissionProvider>();
            builder.RegisterType<TerritoriesHierarchyService>().As<ITerritoriesHierarchyService>();

            //for TerritoriesService
            var mockDefinitionManager = new Mock<IContentDefinitionManager>();
            mockDefinitionManager
                .Setup<IEnumerable<ContentTypeDefinition>>(mdm => mdm.ListTypeDefinitions())
                .Returns(MockTypeDefinitions);
            mockDefinitionManager // this is required to create the test items
                .Setup(mdm => mdm.GetTypeDefinition(It.IsAny<string>()))
                .Returns<string>(name => MockTypeDefinitions().FirstOrDefault(ctd => ctd.Name == name));
            builder.RegisterInstance(mockDefinitionManager.Object);

            builder.RegisterType<Authorizer>().As<IAuthorizer>();
            builder.RegisterType<DefaultContentManager>().As<IContentManager>().SingleInstance();

            //for TerritoryRepositoryService
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));

            //for Authorizer
            builder.RegisterInstance(new Mock<INotifier>().Object);
            builder.RegisterType<AuthorizationServiceStub>().As<IAuthorizationService>();

            //For DefaultContentManager
            builder.RegisterType<StubCacheManager>().As<ICacheManager>();
            builder.RegisterType<DefaultContentManagerSession>().As<IContentManagerSession>();
            builder.RegisterInstance(new Mock<IContentDisplay>().Object);
            builder.RegisterType<Signals>().As<ISignals>();
            builder.RegisterType<DefaultContentQuery>().As<IContentQuery>().InstancePerDependency();

            var _workContext = new Mock<WorkContext>();
            _workContext.Setup(w =>
                w.GetState<IUser>(It.Is<string>(s => s == "CurrentUser"))).Returns(() => { return _currentUser; });

            var _workContextAccessor = new Mock<IWorkContextAccessor>();
            _workContextAccessor.Setup(w => w.GetContext()).Returns(_workContext.Object);
            builder.RegisterInstance(_workContextAccessor.Object).As<IWorkContextAccessor>();

            //Handlers
            builder.RegisterType<TerritoryHierarchyPartHandler>().As<IContentHandler>();
            builder.RegisterType<TerritoryHierachyMockHandler>().As<IContentHandler>();
            builder.RegisterType<TerritoryPartHandler>().As<IContentHandler>();
            builder.RegisterType<TerritoryMockHandler>().As<IContentHandler>();
        }

        private List<ContentTypeDefinition> MockTypeDefinitions() {
            var typeDefinitions = new List<ContentTypeDefinition>();
            //generate some dummy definitions for the tests of the Territories feature
            for (int i = 0; i < 3; i++) {
                var settingsDictionary = new Dictionary<string, string>();
                settingsDictionary.Add("TerritoryHierarchyPartSettings.TerritoryType",
                    "TerritoryType" + i.ToString());
                settingsDictionary.Add("TerritoryHierarchyPartSettings.MayChangeTerritoryTypeOnItem",
                    false.ToString(CultureInfo.InvariantCulture));
                var typeDefinition = new ContentTypeDefinition(
                    name: "HierarchyType" + i.ToString(),
                    displayName: "HierarchyType" + i.ToString(),
                    parts: new ContentTypePartDefinition[] {
                        new ContentTypePartDefinition(
                            contentPartDefinition: new ContentPartDefinition(TerritoryHierarchyPart.PartName, Enumerable.Empty<ContentPartFieldDefinition>(), null),
                            settings: new SettingsDictionary(settingsDictionary)
                            )
                    },
                    settings: null
                    );
                typeDefinitions.Add(typeDefinition);
            }
            for (int i = 0; i < 3; i++) {
                var typeDefinition = new ContentTypeDefinition(
                    name: "TerritoryType" + i.ToString(),
                    displayName: "TerritoryType" + i.ToString(),
                    parts: new ContentTypePartDefinition[] {
                        new ContentTypePartDefinition(
                            contentPartDefinition: new ContentPartDefinition(TerritoryPart.PartName, Enumerable.Empty<ContentPartFieldDefinition>(), null),
                            settings: null
                            )
                    },
                    settings: null
                    );
                typeDefinitions.Add(typeDefinition);
            }

            return typeDefinitions;
        }

        protected override IEnumerable<Type> DatabaseTypes {
            get {
                return new[] {
                    typeof(TerritoryInternalRecord),
                    typeof(ContentItemVersionRecord),
                    typeof(ContentItemRecord),
                    typeof(ContentTypeRecord),
                    typeof(TerritoryHierarchyPartRecord),
                    typeof(TerritoryPartRecord)
                };
            }
        }

        private IUser _currentUser;

        [Test]
        public void HierarchyManagePermissionsAreSameNumberAsHierarchyTypesForAdmin() {

            _currentUser = new FakeUser() { UserName = "admin" };

            Assert.That(_territoriesService.GetHierarchyTypes().Count(), Is.EqualTo(3));
            Assert.That(_permissionProvider.ListHierarchyTypePermissions().Count(), Is.EqualTo(3));
        }

        [Test]
        public void TerritoryManagePermissionsAreSameNumberAsTerritoryTypesForAdmin() {

            _currentUser = new FakeUser() { UserName = "admin" };

            Assert.That(_territoriesService.GetTerritoryTypes().Count(), Is.EqualTo(3));
            Assert.That(_permissionProvider.ListTerritoryTypePermissions().Count(), Is.EqualTo(3));
        }

        [Test]
        public void HierarchyManagePermissionsAreNotSameNumberAsHierarchyTypes() {

            _currentUser = new FakeUser() { UserName = "user1" };

            Assert.That(_territoriesService.GetHierarchyTypes().Count(), Is.EqualTo(1));
            Assert.That(_permissionProvider.ListHierarchyTypePermissions().Count(), Is.EqualTo(3));
        }

        [Test]
        public void TerritoryManagePermissionsAreNotSameNumberAsTerritoryTypes() {

            _currentUser = new FakeUser() { UserName = "user1" };

            Assert.That(_territoriesService.GetTerritoryTypes().Count(), Is.EqualTo(1));
            Assert.That(_permissionProvider.ListTerritoryTypePermissions().Count(), Is.EqualTo(3));
        }

        private List<IContent> AddSampleHierarchiesData() {
            var items = new List<IContent> {
                _contentManager.Create<TerritoryHierarchyPart>("HierarchyType1", VersionOptions.Published),
                _contentManager.Create<TerritoryHierarchyPart>("HierarchyType2", VersionOptions.Draft),
                _contentManager.Create<TerritoryHierarchyPart>("HierarchyType0", VersionOptions.Published),
                _contentManager.Create<TerritoryHierarchyPart>("HierarchyType1", VersionOptions.Draft)
            };
            _transactionManager.RequireNew();
            return items;
        }

        [Test]
        public void ParameterlessGetHierarchiesQueryReturnsAllLatestVersions() {
            var created = AddSampleHierarchiesData();
            //verify draft vs published

            var gotten = _territoriesService.GetHierarchiesQuery().List();
            Assert.That(gotten.Count() == created.Count());
            Assert.That(gotten
                .Select(pa => pa.ContentItem)
                .Where(ci => ci.IsPublished())
                .Count(), Is.EqualTo(2));
            Assert.That(gotten
                .Select(pa => pa.ContentItem)
                .Where(ci => !ci.IsPublished())
                .Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetHierarchiesQueryReturnsSpecificVersions() {
            var created = AddSampleHierarchiesData();
            //try published separately from draft

            var gottenDraft = _territoriesService.GetHierarchiesQuery(VersionOptions.Draft).List();
            Assert.That(gottenDraft.Count(), Is.EqualTo(2));

            var gottenPub = _territoriesService.GetHierarchiesQuery(VersionOptions.Published).List();
            Assert.That(gottenPub.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetHierarchiesQueryDiscriminatesOnContentType() {
            var created = AddSampleHierarchiesData();

            var gotten0 = _territoriesService.GetHierarchiesQuery("HierarchyType0").List();
            Assert.That(gotten0.Count(), Is.EqualTo(1));

            var gotten1 = _territoriesService.GetHierarchiesQuery("HierarchyType1").List();
            Assert.That(gotten1.Count(), Is.EqualTo(2));

            var gotten02 = _territoriesService.GetHierarchiesQuery("HierarchyType0", "HierarchyType2").List();
            Assert.That(gotten02.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetHierarchiesQueryFiltersBothVersionAndContentTypes() {
            var created = AddSampleHierarchiesData();

            var gotten0 = _territoriesService.GetHierarchiesQuery(VersionOptions.Draft, "HierarchyType0").List();
            Assert.That(gotten0.Count(), Is.EqualTo(0));

            var gotten1 = _territoriesService.GetHierarchiesQuery(VersionOptions.Published, "HierarchyType0").List();
            Assert.That(gotten1.Count(), Is.EqualTo(1));

            var gotten2 = _territoriesService.GetHierarchiesQuery(VersionOptions.Draft, "HierarchyType0", "HierarchyType2").List();
            Assert.That(gotten2.Count(), Is.EqualTo(1));

            var gotten3 = _territoriesService.GetHierarchiesQuery(VersionOptions.Published, "HierarchyType1").List();
            Assert.That(gotten3.Count(), Is.EqualTo(1));
        }
        
        private void AddSampleTerritoriesData(out List<IContent> hierarchies, out List<IContent> territories) {
            hierarchies = AddSampleHierarchiesData();
            territories = new List<IContent>();
            var hierarchiesArray = hierarchies.ToArray();
            for (int i = 0; i < hierarchiesArray.Length; i++) {
                var currentHierarchy = hierarchiesArray[i].As<TerritoryHierarchyPart>();
                var territoryType = currentHierarchy.TerritoryType;
                // add i+1 territories
                for (int j = 0; j < i + 1; j++) {
                    var territory = _contentManager
                        .Create<TerritoryPart>(territoryType, i % 2 == 0 ? VersionOptions.Published : VersionOptions.Draft);
                    _territoriesHierarchyService.AddTerritory(territory, currentHierarchy);
                    territories.Add(territory);
                }
            }
            _transactionManager.RequireNew();
        }

        [Test]
        public void GetTerritoryQueriesThrowExceptionForNullHierarchy() {
            Assert.Throws<ArgumentNullException>(() => _territoriesService.GetTerritoriesQuery(null));

            var hierarchy = _contentManager.Create<TerritoryHierarchyPart>("HierarchyType0");
            hierarchy.Record = null;
            Assert.Throws<ArgumentNullException>(() => _territoriesService.GetTerritoriesQuery(hierarchy));
        }

        [Test]
        public void GetTerritoriesQueryDoesNotBreakIfThereAreNoTerritories() {

            var hierarchy = _contentManager.Create<TerritoryHierarchyPart>("HierarchyType0");
            Assert.That(_territoriesService.GetTerritoriesQuery(hierarchy).List().Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetTerritoriesQueryForLatestReturnsAllTerritoriesForHierarchy() {
            List<IContent> hierarchies, territories;
            AddSampleTerritoriesData(out hierarchies, out territories);

            int expectedCount = 1;
            foreach (var hierarchy in hierarchies) {
                var gotten = _territoriesService
                    .GetTerritoriesQuery((TerritoryHierarchyPart)hierarchy, VersionOptions.Latest)
                    .List();
                Assert.That(gotten.Count(), Is.EqualTo(expectedCount));
                expectedCount++;
            }
        }

        [Test]
        public void GetTerritoriesQueryReturnsSameVersionAsHierarchy() {
            List<IContent> hierarchies, territories;
            AddSampleTerritoriesData(out hierarchies, out territories);
            
            foreach (var hierarchy in hierarchies) {
                var gotten = _territoriesService
                    .GetTerritoriesQuery(hierarchy.As<TerritoryHierarchyPart>())
                    .List();
                var all = _territoriesService
                    .GetTerritoriesQuery(hierarchy.As<TerritoryHierarchyPart>(), VersionOptions.Latest)
                    .List();
                var publishedTerritories = all.Where(t => t.ContentItem.IsPublished());

                var expectedCount = all.Count();
                if (hierarchy.ContentItem.IsPublished()) {
                    expectedCount = publishedTerritories.Count();
                }

                Assert.That(gotten.Count(), Is.EqualTo(expectedCount));
            }

        }

        [Test]
        public void GetAvailableTerritoryInternalsThrowsTheExpectedArgumentNullExceptions() {
            Assert.Throws<ArgumentNullException>(() => _territoriesService.GetAvailableTerritoryInternals(null));

            var hierarchy = _contentManager.Create<TerritoryHierarchyPart>("HierarchyType0");
            hierarchy.Record = null;
            Assert.Throws<ArgumentNullException>(() => _territoriesService.GetAvailableTerritoryInternals(hierarchy));
        }

        #region These tests would require the 1-to-many relationships to work in the test db
        //[Test]
        //public void GetAvailableTerritoryInternalsDoesNotReturnUsedInternals() { }

        #endregion
    }
}
