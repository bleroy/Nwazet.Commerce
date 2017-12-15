using Nwazet.Commerce.Exceptions;
using Nwazet.Commerce.Extensions;
using Nwazet.Commerce.Models;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Territories")]
    public class TerritoryRepositoryService : ITerritoriesRepositoryService {

        private readonly IRepository<TerritoryInternalRecord> _territoryInternalRecord;

        public TerritoryRepositoryService(
            IRepository<TerritoryInternalRecord> territoryInternalRecord) {

            _territoryInternalRecord = territoryInternalRecord;

            T = NullLocalizer.Instance;
        }

        public Localizer T;

        public TerritoryInternalRecord GetTerritoryInternal(int id) {
            var tir = _territoryInternalRecord.Get(id);
            return tir == null ? null :
                tir.CreateSafeDuplicate();
        }

        public TerritoryInternalRecord GetTerritoryInternal(string name) {
            name = name.Trim();
            try {
                var tir = _territoryInternalRecord.Get(x => x.Name == name);
                return tir == null ? null :
                    tir.CreateSafeDuplicate();
            } catch (Exception) {
                //sqlCE doe not support using strings properly when their length is such that the column
                //in the record is of type ntext.
                var tirs = _territoryInternalRecord.Fetch(x =>
                    x.Name.StartsWith(name) && x.Name.EndsWith(name));
                var tir = tirs.ToList().Where(rr => rr.Name == name).FirstOrDefault();
                return tir == null ? null :
                    tir.CreateSafeDuplicate();
            }
        }

        public IEnumerable<TerritoryInternalRecord> GetTerritories(int startIndex = 0, int pageSize = 0) {
            var result = _territoryInternalRecord.Table
                .Skip(startIndex >= 0 ? startIndex : 0);

            if (pageSize > 0) {
                return result.Take(pageSize).CreateSafeDuplicate();
            }
            return result.ToList().CreateSafeDuplicate();
        }

        public IEnumerable<TerritoryInternalRecord> GetTerritories(int[] itemIds) {
            return _territoryInternalRecord
                .Fetch(x => itemIds.Contains(x.Id))
                .CreateSafeDuplicate(); 
        }

        public int GetTerritoriesCount() {
            return _territoryInternalRecord.Table.Count();
        }

        public TerritoryInternalRecord AddTerritory(TerritoryInternalRecord tir) {
            ValidateTir(tir);
            tir.Name = tir.Name.Trim();
            if (GetSameNameIds(tir).Any()) {
                throw new TerritoryInternalDuplicateException(T("Cannot create duplicate names. A territory with the same name already exists."));
            }
            _territoryInternalRecord.Create(tir);
            return tir.CreateSafeDuplicate();
        }

        public TerritoryInternalRecord Update(TerritoryInternalRecord tir) {
            ValidateTir(tir);
            tir.Name = tir.Name.Trim();
            if (GetSameNameIds(tir).Any()) {
                throw new TerritoryInternalDuplicateException(T("A territory with the same name already exists."));
            }
            _territoryInternalRecord.Update(tir);
            return tir.CreateSafeDuplicate();
        }

        public void Delete(int id) {
            var tir = _territoryInternalRecord.Get(id);
            if (tir != null) {
                // Handle connected TerritoryParts
                foreach (var tpr in tir.TerritoryParts) {
                    tpr.TerritoryInternalRecord = null;
                }
                // Delete record
                _territoryInternalRecord.Delete(tir);
            }
        }

        public void Delete(TerritoryInternalRecord tir) {
            if (tir != null) {
                Delete(tir.Id);
            }
        }

        private IEnumerable<int> GetSameNameIds(TerritoryInternalRecord tir) {
            var name = tir.Name.Trim();
            try {
                return _territoryInternalRecord.Table
                    .Where(x => x.Name == name && (tir.Id == 0 || tir.Id != x.Id)) //can have same name as its own self
                    .ToList() //force execution of the query so it can fail in sqlCE
                    .Select(x => x.Id);
            } catch (Exception) {
                //sqlCE doe not support using strings properly when their length is such that the column
                //in the record is of type ntext.
                var tirs = _territoryInternalRecord.Fetch(x =>
                    x.Name.StartsWith(name) && x.Name.EndsWith(name));
                return tirs
                    .ToList() //force execution so that Linq happens on in-memory objects
                    .Where(x => x.Name == name && (tir.Id == 0 || tir.Id != x.Id))
                    .Select(x => x.Id);
            }
        }

        /// <summary>
        /// Validates a TerritoryInternalRecord parameter, throwing ArgumentExceptions if it fails.
        /// </summary>
        /// <param name="tir">A TerritoryInternalRecord to validate.</param>
        private void ValidateTir(TerritoryInternalRecord tir) {
            if (tir == null) {
                throw new ArgumentNullException("TerritoryInternalRecord");
            }
            if (string.IsNullOrWhiteSpace(tir.Name)) {
                throw new ArgumentNullException("Name");
            }
        }
    }
}
