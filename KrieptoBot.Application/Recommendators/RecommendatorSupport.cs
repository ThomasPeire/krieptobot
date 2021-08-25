﻿using System.Threading.Tasks;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorSupport : RecommendatorBase
    {
        public override float Weight => 0.7F;

        protected override Task<RecommendatorScore> CalculateRecommendation(string market)
        {
            //To be implemented
            return Task.FromResult(new RecommendatorScore { Score = .0F });
        }
    }
}