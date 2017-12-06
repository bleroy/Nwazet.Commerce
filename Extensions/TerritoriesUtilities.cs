using Orchard.Localization;

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
    }
}
