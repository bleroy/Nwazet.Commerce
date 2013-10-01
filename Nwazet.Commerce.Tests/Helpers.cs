using System;
using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Stubs;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Nwazet.Commerce.Tests {
    public class Helpers {
        public static WeightBasedShippingMethodPart BuildWeightBasedShippingMethod(
            double price,
            double minimumWeight = 0,
            double maximumWeight = double.PositiveInfinity
            ) {

            var result = new WeightBasedShippingMethodPart {
                Record = new WeightBasedShippingMethodPartRecord(),
                Price = price,
                MinimumWeight = minimumWeight,
                MaximumWeight = maximumWeight
            };
            return result;
        }

        public static SizeBasedShippingMethodPart BuildSizeBasedShippingMethod(
            double price,
            string size = null,
            int priority = 0) {
            return new SizeBasedShippingMethodPart {
                Record = new SizeBasedShippingMethodPartRecord(),
                Price = price,
                Size = size,
                Priority = priority
            };
        }

        public static UspsShippingMethodPart BuildUspsShippingMethod(
            string size = null,
            int priority = 0) {
            return new UspsShippingMethodPart {
                Record = new UspsShippingMethodPartRecord(),
                Size = size,
                Priority = priority,
            };
        }

        public static ContentItem PreparePart<TPart, TRecord>(TPart part, string contentType, int id = -1)
            where TPart : ContentPart<TRecord>
            where TRecord : ContentPartRecord, new() {

            part.Record = new TRecord();
            var contentItem = part.ContentItem = new ContentItem {
                VersionRecord = new ContentItemVersionRecord {
                    ContentItemRecord = new ContentItemRecord()
                },
                ContentType = contentType
            };
            contentItem.Record.Id = id;
            contentItem.Weld(part);
            return contentItem;
        }

        public static IWorkContextAccessor GetUspsWorkContextAccessor(string originZip,
                                                                      bool commercialPrices,
                                                                      bool commercialPlusPrices, double price = 10) {
            return new WorkContextAccessorStub(new Dictionary<Type, object> {
                {
                    typeof (IUspsService),
                    new UspsServiceStub("", originZip, commercialPrices, commercialPlusPrices, price)
                }
            });
        }
    }
}
