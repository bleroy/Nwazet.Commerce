using Orchard.Data.Conventions;
using Orchard.Environment.Extensions;
using System.Collections.Generic;

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
        /// Returns a copy of this TerritoryInternalRecord.
        /// </summary>
        /// <returns>>A copy of this TerritoryInternalRecord that can be safely manipulated
        /// without affecting records in the database.</returns>
        public TerritoryInternalRecord CreateSafeDuplicate() {
            return new TerritoryInternalRecord {
                Id = this.Id,
                Name = this.Name,
                TerritoryParts = this.TerritoryParts
            };
        }
    }
}
