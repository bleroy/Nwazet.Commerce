using System;
using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Tests.Stubs;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;
using Orchard.ContentManagement.Records;

namespace Nwazet.Commerce.Tests {
    public class Helpers {
        public static WeightBasedShippingMethodPart BuildWeightBasedShippingMethod(
            double price,
            double minimumWeight = 0,
            double maximumWeight = double.PositiveInfinity) {

            var result = new WeightBasedShippingMethodPart();
            PreparePart<WeightBasedShippingMethodPart, WeightBasedShippingMethodPartRecord>(result,
                "WeightBasedShippingMethod", 0);
            result.Price = price;
            result.MinimumWeight = minimumWeight;
            result.MaximumWeight = maximumWeight;
            return result;
        }

        public static SizeBasedShippingMethodPart BuildSizeBasedShippingMethod(
            double price,
            string size = null,
            int priority = 0) {

            var result = new SizeBasedShippingMethodPart();
            PreparePart<SizeBasedShippingMethodPart, SizeBasedShippingMethodPartRecord>(result,
                "SizeBasedShippingMethod", 0);
            result.Price = price;
            result.Size = size;
            result.Priority = priority;
            return result;
        }

        public static UspsShippingMethodPart BuildUspsShippingMethod(
            string size = null,
            int priority = 0) {

            var result = new UspsShippingMethodPart();
            PreparePart<UspsShippingMethodPart, UspsShippingMethodPartRecord>(result,
                "UspsShippingMethod", 0);
            result.Size = size;
            result.Priority = priority;
            return result;
        }

        public static ContentItem PreparePart<TPart, TRecord>(TPart part, string contentType, int id = -1)
            where TPart : ContentPart<TRecord>
            where TRecord : ContentPartRecord, new() {

            part.Record = new TRecord();
            return PreparePart(part, contentType, id);
        }

        public static ContentItem PreparePart<TPart>(TPart part, string contentType, int id = -1)
            where TPart : ContentPart {

            var contentItem = part.ContentItem = new ContentItem {
                VersionRecord = new ContentItemVersionRecord {
                    ContentItemRecord = new ContentItemRecord()
                },
                ContentType = contentType
            };
            contentItem.Record.Id = id;
            contentItem.Weld(part);
            contentItem.Weld(new InfosetPart());
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
