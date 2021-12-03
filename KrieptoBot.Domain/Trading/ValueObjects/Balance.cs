using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Trading.ValueObjects
{
    public class Balance : ValueObject
    {
        public Balance(Symbol symbol, Amount available, Amount inOrder)
        {
            Symbol = symbol;
            Available = available;
            InOrder = inOrder;
        }

        public Symbol Symbol { get; }
        public Amount Available { get; }
        public Amount InOrder { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Symbol;
            yield return Available;
            yield return InOrder;
        }
    }
}
