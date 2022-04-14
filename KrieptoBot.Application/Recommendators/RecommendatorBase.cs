using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KrieptoBot.Application.Settings;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application.Recommendators
{
    public abstract class RecommendatorBase : IRecommendator
    {
        private readonly ILogger<RecommendatorBase> _logger;
        protected readonly RecommendatorSettings RecommendatorSettings;

        protected RecommendatorBase(RecommendatorSettings recommendatorSettings, ILogger<RecommendatorBase> logger)
        {
            RecommendatorSettings = recommendatorSettings;
            _logger = logger;
        }

        private decimal SellRecommendationWeight =>
            RecommendatorSettings.SellRecommendationWeights.GetValueOrDefault(GetType().Name);

        private decimal BuyRecommendationWeight =>
            RecommendatorSettings.BuyRecommendationWeights.GetValueOrDefault(GetType().Name);

        protected virtual string Name => GetType().Name;
        public virtual IEnumerable<Type> DependencyRecommendators => new List<Type>();

        public async Task<RecommendatorScore> GetRecommendation(Market market)
        {
            if (SellRecommendationWeight == 0m && BuyRecommendationWeight == 0m)
            {
                return new RecommendatorScore(0m, false);
            }

            var recommendation = await CalculateRecommendation(market);

            var weightedRecommendation = recommendation > 0
                ? recommendation * BuyRecommendationWeight
                : recommendation * SellRecommendationWeight;

            LogRecommendatorScore(market, weightedRecommendation);

            return weightedRecommendation;
        }

        protected abstract Task<RecommendatorScore> CalculateRecommendation(Market market);


        private void LogRecommendatorScore(Market market, RecommendatorScore recommendatorScore)
        {
            _logger.LogInformation(
                "Market {Market} - {Recommendator} Recommendation score: {Score} - Included in final score {Included}",
                market.Name.Value, Name, recommendatorScore.Value.ToString("0.00"), recommendatorScore.IncludeInAverageScore);
        }
    }
}
