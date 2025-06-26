using System.Collections.Generic;

namespace KrieptoBot.Application.Recommendators;

public interface IRecommendatorSorter
{
    IEnumerable<IRecommendator> GetSortRecommendators();
}