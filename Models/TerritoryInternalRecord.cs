using Orchard.Data.Conventions;
using Orchard.Environment.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Territories")]
    public class TerritoryInternalRecord {
        public virtual int Id { get; set; } //Primary Key
        [StringLengthMax]
        public virtual string Name { get; set; } //Name given to the territory

        public TerritoryInternalRecord() {
            TerritoryParts = new List<TerritoryPartRecord>();
        }

        public virtual IList<TerritoryPartRecord> TerritoryParts { get; set; }

        /// <summary>
        /// Returns a copy of the TerritoryInternalRecord passed as parameter.
        /// </summary>
        /// <param name="tir">The object to duplicate</param>
        /// <returns>A copy of the TerritoryInternalRecord passed as parameter that can be safely manipulated.</returns>
        public static TerritoryInternalRecord Copy(TerritoryInternalRecord tir) {
            return new TerritoryInternalRecord {
                Id = tir.Id,
                Name = tir.Name,
                TerritoryParts = tir.TerritoryParts
            };
            //this allows us to safely pass stuff along without affecting the data in the db
        }

        /// <summary>
        /// Returns a copy of the TerritoryInternalRecords passed as parameter.
        /// </summary>
        /// <param name="tir">The object to duplicate</param>
        /// <returns>A copy of the TerritoryInternalRecords passed as parameter that can be safely manipulated.</returns>
        public static IEnumerable<TerritoryInternalRecord> Copy(IEnumerable<TerritoryInternalRecord> records) {
            var copy = new List<TerritoryInternalRecord>(records.Count());
            copy.AddRange(records.Select(tir => Copy(tir)));
            return copy;
        }
    }
}
