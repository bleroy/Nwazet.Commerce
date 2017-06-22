using System.Collections.Generic;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;

namespace Nwazet.Commerce.Activities {
    [OrchardFeature("Nwazet.Commerce")]
    public abstract class CommerceActivity : Event {

        public Localizer T { get; set; }

        public override bool CanStartWorkflow {
            get { return true; }
        }

        public override bool CanExecute(WorkflowContext workflowContext, ActivityContext activityContext) {
            return true;
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext,
            ActivityContext activityContext) {
            return new[] {T("Done")};
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext,
            ActivityContext activityContext) {
            yield return T("Done");
        }

        public override LocalizedString Category {
            get { return T("Commerce"); }
        }
    }

    [OrchardFeature("Nwazet.Commerce")]
    public class CartUpdated : CommerceActivity {
        public override string Name {
            get { return "CartUpdated"; }
        }

        public override LocalizedString Description {
            get { return T("The shopping cart has been updated."); }
        }
    }

    [OrchardFeature("Nwazet.Commerce")]
    public class CartItemAdded : CommerceActivity {
        public override string Name {
            get { return "CartItemAdded"; }
        }

        public override LocalizedString Description {
            get { return T("An item has been added to the cart."); }
        }
    }

    [OrchardFeature("Nwazet.Commerce")]
    public class CartItemRemoved : CommerceActivity {
        public override string Name {
            get { return "CartItemRemoved"; }
        }

        public override LocalizedString Description {
            get { return T("An item has been removed from the cart."); }
        }
    }

    [OrchardFeature("Nwazet.Commerce")]
    public class CartFinalized : CommerceActivity {
        public override string Name {
            get { return "CartFinalized"; }
        }

        public override LocalizedString Description {
            get { return T("The shopping cart has been finalized (e.g. by completing an order)."); }
        }
    }
}