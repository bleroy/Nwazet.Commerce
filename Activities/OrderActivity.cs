using System.Collections.Generic;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;

namespace Nwazet.Commerce.Activities {
    [OrchardFeature("Nwazet.Orders")]
    public abstract class OrderActivity : Event {

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

    [OrchardFeature("Nwazet.Orders")]
    public class NewOrder : OrderActivity {
        public override string Name {
            get { return "NewOrder"; }
        }

        public override LocalizedString Description {
            get { return T("A new order has been passed."); }
        }
    }

    [OrchardFeature("Nwazet.Orders")]
    public class OrderStatusChanged : OrderActivity {
        public override string Name {
            get { return "OrderStatusChanged"; }
        }

        public override LocalizedString Description {
            get { return T("The status of an order has changed."); }
        }
    }

    [OrchardFeature("Nwazet.Orders")]
    public class OrderTrackingUrlChanged : OrderActivity {
        public override string Name {
            get { return "OrderTrackingUrlChanged"; }
        }

        public override LocalizedString Description {
            get { return T("The tracking URL of an order has changed."); }
        }
    }
}