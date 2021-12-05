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
        private readonly RecommendatorSettings _recommendatorSettings;

        protected RecommendatorBase(RecommendatorSettings recommendatorSettings, ILogger<RecommendatorBase> logger)
        {
            _recommendatorSettings = recommendatorSettings;
            _logger = logger;
        }

        private decimal SellRecommendationWeight =>
            _recommendatorSettings.SellRecommendationWeights.GetValueOrDefault(GetType().Name);

        private decimal BuyRecommendationWeight =>
            _recommendatorSettings.BuyRecommendationWeights.GetValueOrDefault(GetType().Name);

        protected virtual string Name => GetType().Name;
        public virtual IEnumerable<Type> DependencyRecommendators => new List<Type>();

        public async Task<RecommendatorScore> GetRecommendation(Market market)
        {
            var recommendation = await CalculateRecommendation(market);

            var weightedRecommendation = recommendation > 0
                ? recommendation * BuyRecommendationWeight
                : recommendation * SellRecommendationWeight;

            LogRecommendatorScore(market, weightedRecommendation);

            return weightedRecommendation;
        }

        protected abstract Task<RecommendatorScore> CalculateRecommendation(Market market);


        private void LogRecommendatorScore(Market market, decimal recommendatorScore)
        {
            _logger.LogInformation(
                "Market {Market} - {Recommendator} Recommendation score: {Score}",
                market.Name.Value, Name, recommendatorScore.ToString("0.00"));
        }
    }
}
