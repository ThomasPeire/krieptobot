﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Application.Constants;
using KrieptoBot.Application.Indicators;
using KrieptoBot.Application.Settings;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KrieptoBot.Application.Recommendators;

public class RecommendatorDownTrend(
    IOptions<RecommendatorSettings> recommendatorSettings,
    ILogger<RecommendatorDownTrend> logger,
    IExchangeService exchangeService,
    ITradingContext tradingContext,
    IExponentialMovingAverage ema)
    : RecommendatorBase(recommendatorSettings.Value, logger)
{
    protected override string Name => "Downtrend recommendator";

    protected override async Task<RecommendatorScore> CalculateRecommendation(Market market)
    {
        var trendCandles = await GetTrendCandles(market);
        var intervalCandles = (await GetIntervalCandles(market)).ToList();

        var ema55 = ema.Calculate(trendCandles.ToDictionary(c => c.TimeStamp, c => c.Close.Value), 55);

        var intervalCandlesDictionary = intervalCandles.ToDictionary(c => c.TimeStamp, c => c.Close.Value);
        var ema55ValuesToCheckInterpolated = CalculateEma55ValuesInterpolated(ema55,
            intervalCandlesDictionary.Select(x => x.Key).ToList());

        var allPricesBelowEma = intervalCandlesDictionary.OrderByDescending(x => x.Key)
            .Take(RecommendatorSettings.DownTrendRecommendatorNumberOfConsecutiveCandles)
            .All(x => PriceBelowEmaValue(x, ema55ValuesToCheckInterpolated));

        logger.LogDebug(
            "Market {Market} - {Recommendator} Downtrend detected: {DownTrendDetected}",
            market.Name.Value, Name, allPricesBelowEma);

        var balance = await GetTotalBalanceForAsset(market.Name.BaseSymbol);
        var thereIsOpenBalance = market.MinimumBaseAmount <= balance;

        logger.LogDebug(
            "Market {Market} - {Recommendator} Sufficient open balance: {OpenBalance}, score {IncludeOrNot} included in final score",
            market.Name.Value, Name, thereIsOpenBalance, thereIsOpenBalance ? "won't be" : "will be");

        return allPricesBelowEma
            ? new RecommendatorScore(RecommendationAction.Sell, !thereIsOpenBalance)
            : new RecommendatorScore(RecommendationAction.None, false);
    }

    private IDictionary<DateTime, decimal> CalculateEma55ValuesInterpolated(
        IEnumerable<KeyValuePair<DateTime, decimal>> ema55ValuesToCheck,
        List<DateTime> dateTimesToInterpolate)
    {
        var emaValues = ema55ValuesToCheck.ToList();

        var interpolatedEmaValues = new Dictionary<DateTime, decimal>();

        foreach (var dateTime in dateTimesToInterpolate)
        {
            var firstValue = emaValues.Where(x => x.Key <= dateTime).OrderByDescending(x => x.Key).FirstOrDefault();
            var lastValue = emaValues.Where(x => x.Key >= dateTime).OrderBy(x => x.Key).FirstOrDefault();
            lastValue = lastValue.Equals(default(KeyValuePair<DateTime, decimal>))
                ? firstValue
                : lastValue;

            var difference = lastValue.Value - firstValue.Value;

            var numberOfMinutesBetweenFirstAndLast = (lastValue.Key - firstValue.Key).TotalMinutes;
            var numberOfMinutesSinceFirst = (dateTime - firstValue.Key).TotalMinutes;

            if (numberOfMinutesBetweenFirstAndLast == 0 || numberOfMinutesSinceFirst == 0)
            {
                interpolatedEmaValues.Add(dateTime, firstValue.Value);
                continue;
            }

            var percentageOfMinutesSinceFirst = numberOfMinutesSinceFirst / numberOfMinutesBetweenFirstAndLast;

            interpolatedEmaValues.Add(dateTime,
                firstValue.Value + (decimal)percentageOfMinutesSinceFirst * difference);
        }

        return interpolatedEmaValues;
    }

    private static bool PriceBelowEmaValue(KeyValuePair<DateTime, decimal> price,
        IDictionary<DateTime, decimal> emaValues)
    {
        return price.Value < emaValues[price.Key];
    }

    private async Task<IEnumerable<Candle>> GetIntervalCandles(Market market)
    {
        return await exchangeService.GetCandlesAsync(market.Name,
            tradingContext.Interval,
            end: tradingContext.CurrentTime);
    }

    private async Task<IEnumerable<Candle>> GetTrendCandles(Market market)
    {
        return await exchangeService.GetCandlesAsync(market.Name,
            RecommendatorSettings.DownTrendRecommendatorInterval,
            end: tradingContext.CurrentTime);
    }

    private async Task<Amount> GetTotalBalanceForAsset(Symbol asset)
    {
        var availableBalance = await exchangeService.GetBalanceAsync(asset);

        return availableBalance != null
            ? availableBalance.Available + availableBalance.InOrder
            : Amount.Zero;
    }
}