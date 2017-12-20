using Nwazet.Commerce.Models;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Extensions {
    public static class TerritoriesUtilities {

        static Localizer T = NullLocalizer.Instance;

        //Messages that are displayed in different Unauthorized conditions
        public static string Default401HierarchyMessage = T("Not authorized to manage hierarchies.").Text;
        public static LocalizedString Creation401HierarchyMessage = T("Couldn't create hierarchy");
        public static LocalizedString Edit401HierarchyMessage = T("Couldn't edit hierarchy");
        public static LocalizedString Delete401HierarchyMessage = T("Couldn't delete hierarchy");

        public static string Default401TerritoryMessage = T("Not authorized to manage territories.").Text;
        public static LocalizedString Creation401TerritoryMessage = T("Couldn't create territory");
        public static LocalizedString Edit401TerritoryMessage = T("Couldn't edit territory");
        public static LocalizedString Delete401TerritoryMessage = T("Couldn't delete territory");

        public static string SpecificHierarchy401Message(string typeName) {
            return T("Not authorized to manage hierarchies of type \"{0}\"", typeName).Text;
        }

        public static string SpecificTerritory401Message(string typeName) {
            return T("Not authorized to manage territories of type \"{0}\"", typeName).Text;
        }

        /// <summary>
        /// This method verifies that neither the passed territory is valid. In case it's not, it will 
        /// throw the corresponding exception.
        /// </summary>
        /// <param name="territory">The TerritoryPart argument to validate.</param>
        /// <param name="name">The name of the argument being validated.</param>
        /// <exception cref="ArgumentNullException">Throws an ArgumentNullException if the TerritoryPart
        /// argument is null.</exception>
        /// <exception cref="ArgumentException">Throws an ArgumentException if the TerritoryPart
        /// argument has a null underlying record.</exception>
        public static void ValidateArgument(TerritoryPart territory, string name) {
            if (territory == null) {
                throw new ArgumentNullException(name);
            }
            if (territory.Record == null) {
                throw new ArgumentException(T("Part record cannot be null.").Text, name);
            }
        }
        /// <summary>
        /// This method verifies that neither the passed territory is valid. In case it's not, it will 
        /// throw the corresponding exception.
        /// </summary>
        /// <param name="hierarchy">The TerritoryHierarchyPart argument to validate.</param>
        /// <param name="name">The name of the argument being validated.</param>
        /// <exception cref="ArgumentNullException">Throws an ArgumentNullException if the TerritoryHierarchyPart
        /// argument is null.</exception>
        /// <exception cref="ArgumentException">Throws an ArgumentException if the TerritoryHierarchyPart
        /// argument has a null underlying record.</exception>
        public static void ValidateArgument(TerritoryHierarchyPart hierarchy, string name) {
            if (hierarchy == null) {
                throw new ArgumentNullException(name);
            }
            if (hierarchy.Record == null) {
                throw new ArgumentException(T("Part record cannot be null.").Text, name);
            }
        }

        // The following constants are used to define the editor routes for hierarchies and territories
        // (see in the respective handlers)
        public static string Area = "Nwazet.Commerce";
        public static string HierarchyEditController = "TerritoryHierarchiesAdmin";
        public static string TerritoryEditController = "HierarchyTerritoriesAdmin";
        public static string HierarchyEditAction = "EditHierarchy";
        public static string TerritoryEditAction = "EditTerritory";

        /// <summary>
        /// Returns a copy of the TerritoryInternalRecords
        /// </summary>
        /// <param name="records"></param>
        /// <returns>A copy of the Ienumerable whose elements can be safely manipulated without affecting 
        /// records in the database.</returns>
        public static IEnumerable<TerritoryInternalRecord> CreateSafeDuplicate(this IEnumerable<TerritoryInternalRecord> records) {
            var copy = new List<TerritoryInternalRecord>(records.Count());
            copy.AddRange(records.Select(tir => tir.CreateSafeDuplicate()));
            return copy;
        }
    }
}
