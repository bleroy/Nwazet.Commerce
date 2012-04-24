using Orchard.Events;

namespace Nwazet.Commerce.Tokens {
    public interface ITokenProvider : IEventHandler {
        void Describe(dynamic context);
        void Evaluate(dynamic context);
    }
}