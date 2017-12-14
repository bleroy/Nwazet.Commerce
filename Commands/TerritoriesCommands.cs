using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard.Commands;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Commands {
    [OrchardFeature("Territories")]
    public class TerritoriesCommands : DefaultOrchardCommandHandler {

        private readonly ITerritoriesRepositoryService _territoriesRepositoryService;

        public TerritoriesCommands(
            ITerritoriesRepositoryService territoriesRepositoryService) {
            
            _territoriesRepositoryService = territoriesRepositoryService;
        }

        [CommandName("territories import")]
        [CommandHelp("territories import <name>\r\n\t" + "Imports the territory with the given name to be used as a reference.")]
        public void Import(string territoryName) {
            if (_territoriesRepositoryService.GetTerritoryInternal(territoryName) == null) {
                _territoriesRepositoryService.AddTerritory(new TerritoryInternalRecord { Name = territoryName });
            }
        }
    }
}
