using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Roles.Models;
using Orchard.Security;

namespace Nwazet.Commerce.Tests.Stubs {
    public class UserStub : IUser {
        public UserStub(string userName, string email, IEnumerable<string> roles) {
            Id = -1;
            UserName = userName;
            Email = email;
            ContentItem = new ContentItem {
                VersionRecord = new ContentItemVersionRecord {
                    ContentItemRecord = new ContentItemRecord()
                },
                ContentType = "User"
            };
            var rolesPart = new UserRolesStub(roles.ToList());
            ContentItem.Weld(rolesPart);
        }

        public ContentItem ContentItem { get; private set; }
        public int Id { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
    }

    public class UserRolesStub : ContentPart, IUserRoles {
        public UserRolesStub(IList<string> roles) {
            Roles = roles;
        }
        public IList<string> Roles { get; private set; }
    }
}
