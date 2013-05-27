using System;
using System.Xml.Linq;
using Orchard;

namespace Nwazet.Commerce.Services
{
    public interface IUspsWebService : IDependency
    {
        XElement QueryWebService(Uri url);
    }
}
