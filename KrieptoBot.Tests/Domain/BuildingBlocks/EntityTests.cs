using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using KrieptoBot.Domain.BuildingBlocks;
using NUnit.Framework;

namespace KrieptoBot.Tests.Domain.BuildingBlocks
{
    public class EntityTests
    {
        private readonly Guid Id = Guid.NewGuid();


        [Test]
        public void Entity_ShouldNot_BeEqualWhenOtherTypeIsNoEntity()
        {
            var dummy1 = new DummyEntityObject(Id, "abc");
            var dummy2 = new string("abc");

            var result1 = dummy1.Equals(dummy2);

            result1.Should().BeFalse();
        }

        [Test]
        public void Entity_Should_BeEqualWhenSameReference()
        {
            var dummy1 = new DummyEntityObject(Id, "abc");
            var dummy2 = dummy1;

            var result1 = dummy1.Equals(dummy2);
            var result2 = dummy1 == dummy2;

            result1.Should().BeTrue();
            result2.Should().BeTrue();
        }

        [Test]
        public void Entity_ShouldNot_BeEqualWhenDifferentTypes()
        {
            var dummy1 = new DummyEntityObject(Id, "abc");
            var dummy2 = new OtherDummyEntityObject(Id, "abc");

            var result1 = dummy1.Equals(dummy2);
            var result2 = dummy1 == dummy2;

            result1.Should().BeFalse();
            result2.Should().BeFalse();
        }

        [Test]
        public void Entity_ShouldNot_BeEqualWhenIdHasDefaultValue()
        {
            var dummy1 = new DummyEntityObject(default, "abc");
            var dummy2 = new DummyEntityObject(Id, "abc");

            var result1 = dummy1.Equals(dummy2);
            var result2 = dummy1 == dummy2;
            var result3 = dummy2.Equals(dummy1);
            var result4 = dummy2 == dummy1;

            result1.Should().BeFalse();
            result2.Should().BeFalse();
            result3.Should().BeFalse();
            result4.Should().BeFalse();
        }

        [Test]
        public void Entity_ShouldNot_BeEqualToNull()
        {
            var dummy1 = new DummyEntityObject(Id, "abc");
            DummyEntityObject dummy2 = null;

            var result = dummy1 == dummy2;

            result.Should().BeFalse();
        }

        [Test]
        public void Entity_Should_BeEqualWhenBothAreNull()
        {
            DummyEntityObject dummy1 = null;
            DummyEntityObject dummy2 = null;

            var result = dummy1 == dummy2;

            result.Should().BeTrue();
        }

        [Test]
        public void Entity_Should_BeEqualWhenSameId()
        {
            var dummy1 = new DummyEntityObject(Id, "abc");
            var dummy2 = new DummyEntityObject(Id, "xyz");

            var result1 = dummy1.Equals(dummy2);
            var result2 = dummy1 == dummy2;
            var result3 = dummy1 != dummy2;

            result1.Should().BeTrue();
            result2.Should().BeTrue();
            result3.Should().BeFalse();
        }

        [Test]
        public void GetHashCode_Should_ReturnSameHashCodeWhenObjectHasSameId()
        {
            var dummy1 = new DummyEntityObject(Id, "abc");
            var dummy2 = new DummyEntityObject(Id, "xyz");

            var result = dummy1.GetHashCode() == dummy2.GetHashCode();

            result.Should().BeTrue();
        }
    }

    [ExcludeFromCodeCoverage]
    internal class DummyEntityObject : Entity
    {
        public Guid Id { get; }
        public string OtherProp { get; }

        public DummyEntityObject(Guid id, string otherProp) : base(id)
        {
            Id = id;
            OtherProp = otherProp;
        }
    }

    [ExcludeFromCodeCoverage]
    internal class OtherDummyEntityObject : Entity
    {
        public Guid Id { get; }
        public string OtherProp { get; }

        public OtherDummyEntityObject(Guid id, string otherProp) : base(id)
        {
            Id = id;
            OtherProp = otherProp;
        }
    }
}
