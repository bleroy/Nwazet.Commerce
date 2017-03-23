using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface ISelectedCurrencyProvider : IDependency {
        /// <summary>
        /// The name of the provider is used to identify uniquely the provider.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Describe the provider. Use localized strings, because this will be used in UI.
        /// </summary>
        string Description { get; }
        string CurrencyCode { get; }
        /// <summary>
        /// 3-character code representing the selected currency
        /// </summary>
        /// <summary>
        /// Gets a description of the currency selected for this provider
        /// </summary>
        /// <returns>A string containing a description of a currency.</returns>
        /// <example>If the provider is set to US dollars, the method will return "United States Dollar"</example>
        string GetCurrencyDescription();
        /// <summary>
        /// Gets a string representation of the currency selected.
        /// </summary>
        /// <returns>A string contianing a string representation of the selected currency. If no symbol is available,
        /// this will return the currency code.</returns>
        /// <example>If the provider is set to US dollars, the method will return "$"</example>
        string GetCurrencySymbol();
        /// <summary>
        /// Tells whether the provider is active and should be used.
        /// </summary>
        bool Active { get; set; }
    }
}
