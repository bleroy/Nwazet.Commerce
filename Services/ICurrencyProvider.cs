using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface ICurrencyProvider : IDependency {
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
        /// Gets the format descriptor to be used when writing out currency amounts.
        /// </summary>
        /// <returns>The string to be used as format descriptor.</returns>
        string GetCurrencyFormat();
        /// <summary>
        /// Gets a string represenation of the price that includes the selected currency.
        /// </summary>
        /// <param name="price">The number representing the amount.</param>
        /// <returns>A string representing the price.</returns>
        /// <example>With an inpu of price=42.5, the the selected currency being USD, the method returns
        /// the string "42.5 $".</example>
        string GetPriceString(double price);
        /// <summary>
        /// Gets a string represenation of the price that includes the selected currency.
        /// </summary>
        /// <param name="price">The number representing the amount.</param>
        /// <returns>A string representing the price.</returns>
        /// <example>With an inpu of price=42.5, the the selected currency being USD, the method returns
        /// the string "42.5 $".</example>
        string GetPriceString(decimal price);
        /// <summary>
        /// Gets a string represenation of the price that includes the selected currency.
        /// </summary>
        /// <param name="price">The number representing the amount.</param>
        /// <returns>A string representing the price.</returns>
        /// <example>With an inpu of price=42.5, the the selected currency being USD, the method returns
        /// the string "42.5 $".</example>
        string GetPriceString(double? price);
        /// <summary>
        /// Gets a string represenation of the price that includes the selected currency.
        /// </summary>
        /// <param name="price">The number representing the amount.</param>
        /// <returns>A string representing the price.</returns>
        /// <example>With an inpu of price=42.5, the the selected currency being USD, the method returns
        /// the string "42.5 $".</example>
        string GetPriceString(decimal? price);
    }
}
