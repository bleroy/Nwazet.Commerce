using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement;
using Orchard.Security.Permissions;

namespace Nwazet.Commerce.Tests.Stubs {
    /// <summary>
    /// this implemerntation used in tests for Territories
    /// </summary>
    public class AuthorizationServiceStub : IAuthorizationService {
        public void CheckAccess(Permission permission, IUser user, IContent content) {
            throw new NotImplementedException();
        }

        public bool TryCheckAccess(Permission permission, IUser user, IContent content) {
            if (user == null || user.UserName == "admin") {
                return true;
            }
            //get the number from the permission name (it's the last character)
            var number = int.Parse(permission.Name.Last().ToString());
            if (user.UserName.Contains(number.ToString())) {
                return true;
            }

            return false;
        }
    }
}
