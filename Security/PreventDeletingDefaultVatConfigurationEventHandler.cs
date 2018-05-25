using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Security;
using Orchard.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Security {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class PreventDeletingDefaultVatConfigurationEventHandler : IAuthorizationServiceEventHandler {
        /*
            Currently, the only class where this event handler would be used is RolesBasedAuthorizationService
            The way its TryCheckAccess method is written, the only way to guarantee we prevent the default
            VatConfigurationPart to be deleted is by changing the context.Granted in the Complete method,
            because changing it anywhere else has a change for it be be superseded by other handling, e.g.
            for a superuser context.Granted would always be true. However:
            - I can't see a 100% sure way to verify, in the complete method, that we are seeking authorization 
             for an attempt to delete the deault VatConfigurationPart. Note that such way should be robust and
             prevent deletion while at the same time not deny other authorizations even in concurrent scenarios,
             or in requests where several permissions are being tested on the content.
            - The superuser arguably should be allowed to delete even the default configuration.
        */

        private readonly ISiteService _siteService;
        
        public PreventDeletingDefaultVatConfigurationEventHandler(
            ISiteService siteService) {

            _siteService = siteService;
        }

        public void Checking(CheckAccessContext context) {
            // This is the first method to be called. At this point nothing should have changed the permission
            // we are testing for (that may happen later in the calls to the Adjust handler methods), so this
            // is a good place to take note of the fact that we are processing an attempt to delete the default
            // VatConfigurationPart. However, denying the permission here may be superseded by later steps: even 
            // if we set Granted = false here, if the call comes fromm the superuser the authorization will be 
            // granted in later steps. Moreover, other event handlers may later adjust the permission and compute
            // that the authorization should be granted
            if (IsDefaultVatConfigurationPart(context)) {
                context.Granted &= context.Permission != Orchard.Core.Contents.Permissions.DeleteContent;
            }
        }

        private bool IsDefaultVatConfigurationPart(CheckAccessContext context) {
            var settings = _siteService.GetSiteSettings().As<VatConfigurationSiteSettingsPart>();
            return (context.Content?.Is<VatConfigurationPart>() == true) &&
                settings.DefaultVatConfigurationId == context.Content.ContentItem.Id;
        }

        #region Not implemented IAuthorizationServiceEventHandler methods
        public void Adjust(CheckAccessContext context) {
        }
        public void Complete(CheckAccessContext context) {
        }
        #endregion
    }
}
