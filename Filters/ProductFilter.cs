using System;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc.Filters;
using Orchard.UI.Resources;

namespace Nwazet.Commerce.Filters {

    public class ProductFilter : IFilterProvider {
        private readonly IContentManager _contentManager;
        public Localizer T { get; set; }

        public ProductFilter(IContentManager contentManager) {
            _contentManager = contentManager;

            T = NullLocalizer.Instance;
        }

        public void Describe(dynamic describe) {
            describe.For("ProductPart", T("ProductPart"), T("ProductPart"))
                .Element("Sku", T("Sku"), T("Search for Sku."),
                    (Action<dynamic>)ApplyFilter,
                    (Func<dynamic, LocalizedString>)DisplayFilter,
                    "OwnerStringForm"
                );
        }

        public void ApplyFilter(dynamic context) {
            var query = (IHqlQuery)context.Query;
            if (context.State != null && context.State.Value != null && context.State.Value != "") {
                string op = context.State.Operator;
                string val = context.State.Value;
                IEnumerable<UserPart> owners = getOwners(op, val);
                List<int> ownersId = new List<int>();
                foreach (UserPart owner in owners) {
                    ownersId.Add(owner.Id);
                }
                context.Query = query.Where(x => x.ContentPartRecord<CommonPartRecord>(),
                    x => x.In("OwnerId", ownersId.ToArray()));

            }
            return;
        }

        public LocalizedString DisplayFilter(dynamic context) {
            if (context.State.Value != null && context.State.Value != "")
                return T("Content Items with owner: {0}.", context.State.Value);
            else
                return T("No owner name found");
        }

        private IEnumerable<UserPart> getOwners(string op, string stateValue) {
            List<UserPart> owners = new List<UserPart>();
            switch (op) {
                case "Equals":
                    return _contentManager.Query<UserPart, UserPartRecord>().Where(u => u.UserName.Equals(stateValue)).List();
                case "NotEquals":
                    return _contentManager.Query<UserPart, UserPartRecord>().Where(u => !u.UserName.Equals(stateValue)).List();
                case "Contains":
                    return _contentManager.Query<UserPart, UserPartRecord>().Where(u => u.UserName.Contains(stateValue)).List();
                case "ContainsAny": //rifare il form e mettere la , come separatore tra i nomi...
                    string[] ownList = stateValue.Split(',');
                    foreach (string owner in ownList) {
                        owners.AddRange(_contentManager.Query<UserPart, UserPartRecord>().Where(u => u.UserName.Equals(owner)).List());
                    }
                    return owners;
                case "ContainsAll": //da rifare il form e toglierlo
                    return _contentManager.Query<UserPart, UserPartRecord>().Where(u => u.UserName.Equals(stateValue)).List();
                case "Starts":
                    return _contentManager.Query<UserPart, UserPartRecord>().Where(u => u.UserName.StartsWith(stateValue, StringComparison.OrdinalIgnoreCase)).List();
                case "NotStarts":
                    return _contentManager.Query<UserPart, UserPartRecord>().Where(u => !u.UserName.StartsWith(stateValue, StringComparison.OrdinalIgnoreCase)).List();
                case "Ends":
                    return _contentManager.Query<UserPart, UserPartRecord>().Where(u => u.UserName.EndsWith(stateValue, StringComparison.OrdinalIgnoreCase)).List();
                case "NotEnds":
                    return _contentManager.Query<UserPart, UserPartRecord>().Where(u => !u.UserName.EndsWith(stateValue, StringComparison.OrdinalIgnoreCase)).List();
                case "NotContains":
                    return _contentManager.Query<UserPart, UserPartRecord>().Where(u => !u.UserName.Contains(stateValue)).List();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}