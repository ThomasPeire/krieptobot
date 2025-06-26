using System;
using System.Collections.Generic;
using System.Linq;

namespace KrieptoBot.Application.Recommendators;

public class RecommendatorSorter(IEnumerable<IRecommendator> recommendators) : IRecommendatorSorter
{
    public IEnumerable<IRecommendator> GetSortRecommendators()
    {
        var sortedRecommendators = new List<IRecommendator>();

        AddRecommendatorsWithoutDependency(sortedRecommendators);

        var numberOfCurrentlySortedRecommendators = sortedRecommendators.Count;

        while (ThereAreUnsortedRecommendators(sortedRecommendators))
        {
            AddRecommendatorsWithOnlyDependenciesThatAreAlreadyAdded(sortedRecommendators);

            if (numberOfCurrentlySortedRecommendators == sortedRecommendators.Count)
            {
                break;
            }

            numberOfCurrentlySortedRecommendators = sortedRecommendators.Count;
        }

        if (ThereAreUnsortedRecommendators(sortedRecommendators))
        {
            throw new Exception(
                $"Some recommendators have a circular reference {string.Join(", ", recommendators.Select(x => x.GetType()).Except(sortedRecommendators.Select(y => y.GetType())))}");
        }

        return sortedRecommendators;
    }

    private bool ThereAreUnsortedRecommendators(List<IRecommendator> sortedRecommendators)
    {
        return sortedRecommendators.Count != recommendators.Count();
    }

    private void AddRecommendatorsWithOnlyDependenciesThatAreAlreadyAdded(List<IRecommendator> sortedRecommendators)
    {
        var recommendatorsWhereAllDependenciesAreAlreadySorted =
            recommendators
                .Where(recommendatorToAdd =>
                {
                    var alreadyAdded = sortedRecommendators.Select(x => x.GetType())
                        .Contains(recommendatorToAdd.GetType());

                    var hasRemainingUnaddedDependencies = recommendatorToAdd.DependencyRecommendators
                        .Except(sortedRecommendators.Select(alreadyAddedRecommendator =>
                            alreadyAddedRecommendator.GetType())).Any();

                    return !alreadyAdded && !hasRemainingUnaddedDependencies;
                });

        sortedRecommendators.AddRange(recommendatorsWhereAllDependenciesAreAlreadySorted);
    }

    private void AddRecommendatorsWithoutDependency(List<IRecommendator> sortedRecommendators)
    {
        sortedRecommendators.AddRange(recommendators.Where(x => !x.DependencyRecommendators.Any()));
    }
}