using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AwesomeAssertions;
using KrieptoBot.Domain.BuildingBlocks;
using NUnit.Framework;

namespace KrieptoBot.Tests.Domain.BuildingBlocks
{
    public class ValueObjectTests
    {
        [Test]
        public void ValueObject_ShouldNot_BeEqualWhenDifferentValues()
        {
            var dummy1 = new DummyValueObject("abc");
            var dummy2 = new DummyValueObject("xyz");

            var result1 = dummy1.Equals(dummy2);
            var result2= dummy1 == dummy2;

            result1.Should().BeFalse();
            result2.Should().BeFalse();
        }

        [Test]
        public void ValueObject_Should_BeEqualWhenSameValuesButDifferentInstances()
        {
            var dummy1 = new DummyValueObject("abc");
            var dummy2 = new DummyValueObject("abc");

            var result1 = dummy1.Equals(dummy2);
            var result2= dummy1 == dummy2;

            result1.Should().BeTrue();
            result2.Should().BeTrue();
        }

        [Test]
        public void ValueObject_Should_BeEqualWhenSameReference()
        {
            var dummy1 = new DummyValueObject("abc");
            var dummy2 = dummy1;

            var result1 = dummy1.Equals(dummy2);
            var result2= dummy1 == dummy2;

            result1.Should().BeTrue();
            result2.Should().BeTrue();
        }

        [Test]
        public void ValueObject_ShouldNot_BeEqualToNull()
        {
            var dummy1 = new DummyValueObject("abc");
            DummyValueObject dummy2 = null;

            var result1 = dummy1.Equals(dummy2);
            var result2= dummy1 == dummy2;

            result1.Should().BeFalse();
            result2.Should().BeFalse();
        }

        [Test]
        public void ValueObject_ShouldNot_BeEqualWhenDifferentTypes()
        {
            var dummy1 = new DummyValueObject("abc");
            var dummy2 = new OtherDummyValueObject("abc");

            var result1 = dummy1.Equals(dummy2);
            var result2= dummy1 == dummy2;

            result1.Should().BeFalse();
            result2.Should().BeFalse();
        }
    }

    [ExcludeFromCodeCoverage]
    internal class DummyValueObject : ValueObject
    {
        public string Prop1 { get; }

        public DummyValueObject(string prop1)
        {
            Prop1 = prop1;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Prop1;
        }
    }

    [ExcludeFromCodeCoverage]
    internal class OtherDummyValueObject : ValueObject
    {
        public string Prop1 { get; }

        public OtherDummyValueObject(string prop1)
        {
            Prop1 = prop1;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Prop1;
        }
    }

}
