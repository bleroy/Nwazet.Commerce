using System.Collections.Generic;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;

namespace Nwazet.Commerce.Activities {
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
    public class NewOrder : CommerceActivity {
        public override string Name {
            get { return "NewOrder"; }
        }

        public override LocalizedString Description {
            get { return T("A new order has been passed."); }
        }
    }

    [OrchardFeature("Nwazet.Commerce")]
    public class OrderStatusChanged : CommerceActivity {
        public override string Name {
            get { return "OrderStatusChanged"; }
        }

        public override LocalizedString Description {
            get { return T("The status of an order has changed."); }
        }
    }

    [OrchardFeature("Nwazet.Commerce")]
    public class OrderTrackingUrlChanged : CommerceActivity {
        public override string Name {
            get { return "OrderTrackingUrlChanged"; }
        }

        public override LocalizedString Description {
            get { return T("The tracking URL of an order has changed."); }
        }
    }
}