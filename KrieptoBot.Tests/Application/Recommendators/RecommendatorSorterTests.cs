using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using KrieptoBot.Application.Recommendators;
using Moq;
using NUnit.Framework;

namespace KrieptoBot.Tests.Application.Recommendators;

public class RecommendatorSorterTests
{
    private readonly Mock<IRecommendator1> _recommendatorA_NotDependent = new();
    private readonly Mock<IRecommendator2> _recommendatorB_Dependency_A = new();
    private readonly Mock<IRecommendator3> _recommendatorC_Dependency_B = new();
    private readonly Mock<IRecommendator4> _recommendatorD_Dependency_C = new();
    private readonly Mock<IRecommendator5> _recommendatorE_Dependency_D_B = new();
    private readonly Mock<IRecommendator6> _recommendatorH_Circular_I = new();
    private readonly Mock<IRecommendator7> _recommendatorI_Circular_H = new();

    [SetUp]
    public void Setup()
    {
        _recommendatorA_NotDependent.Setup(x => x.DependencyRecommendators).Returns(new List<Type> { });

        _recommendatorB_Dependency_A.Setup(x => x.DependencyRecommendators)
            .Returns(new List<Type> { _recommendatorA_NotDependent.Object.GetType() });

        _recommendatorC_Dependency_B.Setup(x => x.DependencyRecommendators).Returns(new List<Type>
            { _recommendatorB_Dependency_A.Object.GetType() });

        _recommendatorD_Dependency_C.Setup(x => x.DependencyRecommendators).Returns(new List<Type>
            { _recommendatorC_Dependency_B.Object.GetType(), });

        _recommendatorE_Dependency_D_B.Setup(x => x.DependencyRecommendators).Returns(new List<Type>
            { _recommendatorB_Dependency_A.Object.GetType(), _recommendatorD_Dependency_C.Object.GetType() });


        _recommendatorH_Circular_I.Setup(x => x.DependencyRecommendators).Returns(new List<Type>
        {
            _recommendatorI_Circular_H.Object.GetType()
        });

        _recommendatorI_Circular_H.Setup(x => x.DependencyRecommendators).Returns(new List<Type>
        {
            _recommendatorH_Circular_I.Object.GetType()
        });
    }

    [Test]
    public void Sorter_Should_Sort()
    {
        var recommendators = new List<IRecommendator>
        {
            _recommendatorD_Dependency_C.Object,
            _recommendatorC_Dependency_B.Object,
            _recommendatorB_Dependency_A.Object,
            _recommendatorE_Dependency_D_B.Object,
            _recommendatorA_NotDependent.Object,
        };

        var recommendatorSorter = new RecommendatorSorter(recommendators);

        var result =
            recommendatorSorter.GetSortRecommendators().Select(x => x.GetType()).ToArray();

        result[0].Should().Be(_recommendatorA_NotDependent.Object.GetType());
        result[1].Should().Be(_recommendatorB_Dependency_A.Object.GetType());
        result[2].Should().Be(_recommendatorC_Dependency_B.Object.GetType());
        result[3].Should().Be(_recommendatorD_Dependency_C.Object.GetType());
        result[4].Should().Be(_recommendatorE_Dependency_D_B.Object.GetType());
    }

    [Test]
    public void Sorter_Should_DetectCircularReferences()
    {
        var recommendators = new List<IRecommendator>
        {
            _recommendatorD_Dependency_C.Object,
            _recommendatorC_Dependency_B.Object,
            _recommendatorB_Dependency_A.Object,
            _recommendatorE_Dependency_D_B.Object,
            _recommendatorA_NotDependent.Object,
            _recommendatorH_Circular_I.Object,
            _recommendatorI_Circular_H.Object,
        };

        var recommendatorSorter = new RecommendatorSorter(recommendators);

        Action act = () => recommendatorSorter.GetSortRecommendators();

        act.Should().Throw<Exception>()
            .WithMessage(
                $"Some recommendators have a circular reference {string.Join(", ", new List<Type> { _recommendatorH_Circular_I.Object.GetType(), _recommendatorI_Circular_H.Object.GetType() })}");
    }
}

public interface IRecommendator1 : IRecommendator
{
};

public interface IRecommendator2 : IRecommendator
{
};

public interface IRecommendator3 : IRecommendator
{
};

public interface IRecommendator4 : IRecommendator
{
};

public interface IRecommendator5 : IRecommendator
{
};

public interface IRecommendator6 : IRecommendator
{
};

public interface IRecommendator7 : IRecommendator
{
};

public interface IRecommendator8 : IRecommendator
{
};

public interface IRecommendator9 : IRecommendator
{
};

public interface IRecommendator10 : IRecommendator
{
};