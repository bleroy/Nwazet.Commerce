using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Indexing;

namespace Nwazet.Commerce.Tests.Stubs {
    public class ContentManagerStub : IContentManager {
        private readonly IEnumerable<IContent> _contentItems;
 
        public ContentManagerStub(IEnumerable<IContent> contentItems) {
            _contentItems = contentItems;

            foreach (var item in _contentItems) {
                if (item.ContentItem != null) {
                    item.ContentItem.ContentManager = this;
                }
            }
        }

        public IEnumerable<IContent> GetAllItems() {
            return _contentItems;
        } 

        public ContentItem Get(int id) {
            var item = _contentItems
                .FirstOrDefault(i => i.Id == id);
            return item == null ? null : item.ContentItem;
        }

        public IEnumerable<T> GetMany<T>(IEnumerable<int> ids, VersionOptions options, QueryHints hints) where T : class, IContent {
            var idList = ids.ToList();
            return _contentItems
                .Where(i => idList.Contains(i.Id))
                .Select(i => i.As<T>());
        }

        public IContentQuery<ContentItem> Query() {
            return new FakeQuery<ContentItem>(this);
        }

        public IEnumerable<ContentTypeDefinition> GetContentTypeDefinitions() {
            throw new System.NotImplementedException();
        }

        public ContentItem New(string contentType) {
            throw new System.NotImplementedException();
        }

        public void Create(ContentItem contentItem) {
            throw new System.NotImplementedException();
        }

        public void Create(ContentItem contentItem, VersionOptions options) {
            throw new System.NotImplementedException();
        }

        public ContentItem Get(int id, VersionOptions options) {
            throw new System.NotImplementedException();
        }

        public ContentItem Get(int id, VersionOptions options, QueryHints hints) {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ContentItem> GetAllVersions(int id) {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> GetManyByVersionId<T>(IEnumerable<int> versionRecordIds, QueryHints hints) where T : class, IContent {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ContentItem> GetManyByVersionId(IEnumerable<int> versionRecordIds, QueryHints hints) {
            throw new System.NotImplementedException();
        }

        public void Publish(ContentItem contentItem) {
            throw new System.NotImplementedException();
        }

        public void Unpublish(ContentItem contentItem) {
            throw new System.NotImplementedException();
        }

        public void Remove(ContentItem contentItem) {
            throw new System.NotImplementedException();
        }

        public void Index(ContentItem contentItem, IDocumentIndex documentIndex) {
            throw new System.NotImplementedException();
        }

        XElement IContentManager.Export(ContentItem contentItem) {
            return Export(contentItem);
        }

        public XElement Export(ContentItem contentItem) {
            throw new System.NotImplementedException();
        }

        public void Flush() {
            throw new System.NotImplementedException();
        }

        public void Clear() {
            throw new System.NotImplementedException();
        }

        public IHqlQuery HqlQuery() {
            throw new System.NotImplementedException();
        }

        public ContentItemMetadata GetItemMetadata(IContent contentItem) {
            throw new System.NotImplementedException();
        }

        public IEnumerable<GroupInfo> GetEditorGroupInfos(IContent contentItem) {
            throw new System.NotImplementedException();
        }

        public IEnumerable<GroupInfo> GetDisplayGroupInfos(IContent contentItem) {
            throw new System.NotImplementedException();
        }

        public GroupInfo GetEditorGroupInfo(IContent contentItem, string groupInfoId) {
            throw new System.NotImplementedException();
        }

        public GroupInfo GetDisplayGroupInfo(IContent contentItem, string groupInfoId) {
            throw new System.NotImplementedException();
        }

        public ContentItem ResolveIdentity(ContentIdentity contentIdentity) {
            throw new System.NotImplementedException();
        }

        public dynamic BuildDisplay(IContent content, string displayType = "", string groupId = "") {
            throw new System.NotImplementedException();
        }

        public dynamic BuildEditor(IContent content, string groupId = "") {
            throw new System.NotImplementedException();
        }

        public dynamic UpdateEditor(IContent content, IUpdateModel updater, string groupId = "") {
            throw new System.NotImplementedException();
        }

        public void Import(XElement element, ImportContentSession importContentSession) {
            throw new System.NotImplementedException();
        }
    }
}
