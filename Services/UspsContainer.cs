using System;
using System.Collections.Generic;

namespace Nwazet.Commerce.Services {
    public class UspsContainer {
        internal UspsContainer(string name, string uspsName, ContainerSize size, ContainerDestination destination) {
            UspsName = uspsName;
            Name = name;
            Size = size;
            Destination = destination;
        }

        public static IDictionary<string, UspsContainer> List {
            get { return Containers; }
        }

        public ContainerSize Size { get; private set; }
        public ContainerDestination Destination { get; private set; }
        public string Name { get; private set; }
        public string UspsName { get; private set; }

        public enum ContainerSize {
            Regular,
            Large
        }

        [Flags]
        public enum ContainerDestination {
            Domestic = 1,
            International = 2,
            Anywhere = 3
        }

        private static readonly Dictionary<string, UspsContainer> Containers =
            new Dictionary<string, UspsContainer>()
                .Add("Variable", ContainerSize.Regular, ContainerDestination.Domestic)
                .Add("Flat Rate Box", ContainerSize.Regular, ContainerDestination.Domestic)
                .Add("Lg Flat Rate Box", ContainerSize.Regular, ContainerDestination.Domestic)
                .Add("Md Flat Rate Box", ContainerSize.Regular, ContainerDestination.Domestic)
                .Add("Sm Flat Rate Box", ContainerSize.Regular, ContainerDestination.Domestic)
                .Add("Regular Flat Rate Box", ContainerSize.Regular, ContainerDestination.Domestic)
                .Add("Regional Rate Box A", ContainerSize.Regular, ContainerDestination.Domestic)
                .Add("Regional Rate Box B", ContainerSize.Regular, ContainerDestination.Domestic)
                .Add("Regional Rate Box C", ContainerSize.Regular, ContainerDestination.Domestic)
                .Add("Flat Rate Env.", ContainerSize.Regular, ContainerDestination.Domestic)
                .Add("Legal Flat Rate Env.", ContainerSize.Regular, ContainerDestination.Domestic)
                .Add("Padded Flat Rate Env.", ContainerSize.Regular, ContainerDestination.Domestic)
                .Add("Gift Card Flat Rate Env.", ContainerSize.Regular, ContainerDestination.Domestic)
                .Add("Window Flat Rate Env.", ContainerSize.Regular, ContainerDestination.Domestic)
                .Add("Sm Flat Rate Env.", ContainerSize.Regular, ContainerDestination.Domestic)
                .Add("Variable Large", "Variable", ContainerSize.Large, ContainerDestination.Domestic)
                .Add("Rectangular", ContainerSize.Large, ContainerDestination.Anywhere)
                .Add("Nonrectangular", ContainerSize.Large, ContainerDestination.Anywhere);
    }

    internal static class Helper {
        public static Dictionary<string, UspsContainer> Add(this Dictionary<string, UspsContainer> dictionary,
                                                            string name, string uspsName,
                                                            UspsContainer.ContainerSize size,
                                                            UspsContainer.ContainerDestination destination) {
            dictionary.Add(name, new UspsContainer(name, uspsName, size, destination));
            return dictionary;
        }

        public static Dictionary<string, UspsContainer> Add(this Dictionary<string, UspsContainer> dictionary,
                                                            string name, UspsContainer.ContainerSize size,
                                                            UspsContainer.ContainerDestination destination) {
            dictionary.Add(name, new UspsContainer(name, name, size, destination));
            return dictionary;
        }
    }
}