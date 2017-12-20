using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Tests.Stubs {
    public class FakeUser : IUser {
        public virtual ContentItem ContentItem {
            get {
                throw new NotImplementedException();
            }
        }

        public virtual string Email {
            get {
                throw new NotImplementedException();
            }
        }

        public virtual int Id {
            get {
                throw new NotImplementedException();
            }
        }

        public virtual string UserName {
            get; set;
        }
    }
}
