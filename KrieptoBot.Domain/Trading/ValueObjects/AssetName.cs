using System;
using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Trading.ValueObjects
{
    public class AssetName : ValueObject
    {
        private AssetName()
        {
        }

        public AssetName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Asset name can not be empty");

            Value = value;
        }

        public string Value { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
