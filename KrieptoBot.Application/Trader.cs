using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Application.Settings;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.Entity;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KrieptoBot.Application;

public class Trader(
    ILogger<Trader> logger,
    ITradingContext tradingContext,
    IExchangeService exchangeService,
    IRecommendationCalculator recommendationCalculator,
    ISellManager sellManager,
    IBuyManager buyManager,
    IOptions<TradingSettings> tradingSettings)
    : ITrader
{
    private readonly TradingSettings _tradingSettings = tradingSettings.Value;

    public async Task Run(CancellationToken cancellation = default)
    {
        await LogTime();
        await CancelOpenLimitOrMarketOrders();

        var recommendations = await GetRecommendations();

        LogFinalScores(recommendations);

        await SellIfNeeded(recommendations);
        await BuyIfNeeded(recommendations);
        await SetOrUpdateStopLossForMarketsWithSellableAmount();
    }

    private async Task SetOrUpdateStopLossForMarketsWithSellableAmount()
    {
        if (_tradingSettings.IsSimulation)
            return;

        foreach (var market in await GetMarketsToEvaluate())
        {
            //get balances
            var openStopLossOrders = await GetOpenStopLossOrders(market);
            var balance = await exchangeService.GetBalanceAsync(market.Name.BaseSymbol.Value);
            var amountInStopLossOrders = openStopLossOrders.Sum(x => x.Amount.Value);

            if (balance.Available.Value + amountInStopLossOrders < market.MinimumBaseAmount)
            {
                continue;
            }

            var candles = await exchangeService.GetCandlesAsync(market.Name.Value, tradingContext.Interval,
                end: tradingContext.CurrentTime);

            var stopLossPrice = candles.MaxBy(x => x.TimeStamp)!.Close.Value *
                                (1 - _tradingSettings.StopLossPercentage / 100);

            if (balance.Available.Value >= market.MinimumBaseAmount)
            {
                await exchangeService.PostStopLossOrderAsync(market.Name.Value, balance.Available.Value,
                    stopLossPrice);
            }

            foreach (var existingStopLoss in openStopLossOrders.Where(x => x.TriggerPrice.Value < stopLossPrice))
            {
                await exchangeService.UpdateOrderTriggerAmount(market.Name.Value, existingStopLoss.Id,
                    stopLossPrice);
            }
        }
    }

    private async Task<List<Order>> GetOpenStopLossOrders(Market market)
    {
        var openOrders = await exchangeService.GetOpenOrderAsync(market.Name.Value);
        var openStopLossOrders = openOrders.Where(o => o.Type.Value == OrderType.StopLoss).ToList();
        return openStopLossOrders;
    }

    private async Task LogTime()
    {
        var bitvavoTime = await exchangeService.GetTime();
        logger.LogInformation("Exchange time: {BitvavoTime}, Local time: {Local}",
            bitvavoTime.ToLocalTime(), DateTime.Now);
    }

    private async Task BuyIfNeeded(Dictionary<Market, RecommendatorScore> recommendations)
    {
        var marketsToBuy = await DetermineMarketsToBuy(recommendations);
        if (marketsToBuy.Any()) await Buy(marketsToBuy);
    }

    private async Task SellIfNeeded(Dictionary<Market, RecommendatorScore> recommendations)
    {
        var marketsToSell = await DetermineMarketsToSell(recommendations);
        if (marketsToSell.Any())
            await Sell(marketsToSell.Select(x => x.Key));
    }

    private void LogFinalScores(Dictionary<Market, RecommendatorScore> recommendations)
    {
        foreach (var recommendatorScore in recommendations)
        {
            logger.LogInformation("Market {Market}: Final score: {Score}", recommendatorScore.Key.Name.Value,
                recommendatorScore.Value.Value.ToString("0.00"));
        }
    }

    private async Task CancelOpenLimitOrMarketOrders()
    {
        if (!tradingContext.IsSimulation)
        {
            foreach (var market in await GetMarketsToEvaluate())
            {
                var openMarketOrders = await exchangeService.GetOpenOrderAsync(market.Name.Value);
                var openMarketOrdersIdsToCancel = openMarketOrders.Where(x =>
                    x.Type == OrderType.Limit || x.Type == OrderType.Market).ToList();

                openMarketOrdersIdsToCancel.ForEach(x => exchangeService.CancelOrder(x.MarketName.Value, x.Id));
            }
        }
    }

    private async Task<Dictionary<Market, Amount>> DetermineBudgetForMarket(
        IDictionary<Market, RecommendatorScore> marketsToBuy)
    {
        var totalRecommendationScore = marketsToBuy.Sum(x => x.Value);

        var dictionary = new Dictionary<Market, Amount>();

        var availableBudget = new Amount((await GetAvailableBudgetToInvest()) / 100 * 95);

        foreach (var (market, recommendation) in marketsToBuy)
        {
            var amount = Math.Floor(recommendation.Value * availableBudget.Value /
                                    totalRecommendationScore);

            if (amount < _tradingSettings.MinBuyBudgetPerCoin)
            {
                continue;
            }

            dictionary.Add(
                market,
                new Amount(Math.Min(amount, _tradingSettings.MaxBuyBudgetPerCoin)));
        }

        return dictionary;
    }

    private async Task<Amount> GetAvailableBudgetToInvest()
    {
        var balance = await GetAvailableBalanceForAsset(new Symbol("EUR"));

        logger.LogInformation("Available budget: {Budget}", balance.Value);

        return balance;
    }

    private async Task<Amount> GetAvailableBalanceForAsset(Symbol asset)
    {
        var availableBalance = await exchangeService.GetBalanceAsync(asset);

        return availableBalance != null
            ? availableBalance.Available
            : Amount.Zero;
    }

    private async Task<Amount> GetTotalBalanceForAsset(Symbol asset)
    {
        var availableBalance = await exchangeService.GetBalanceAsync(asset);

        return availableBalance != null
            ? availableBalance.Available + availableBalance.InOrder
            : Amount.Zero;
    }

    private async Task Buy(Dictionary<Market, Amount> marketsWithBudget)
    {
        foreach (var (market, budget) in marketsWithBudget)
        {
            if (await NoTradesInCoolDownPeriod(market))
            {
                await buyManager.Buy(market, budget);
            }
        }
    }

    private async Task<bool> NoTradesInCoolDownPeriod(Market market)
    {
        var trades = await exchangeService.GetTradesAsync(market.Name, 5, end: tradingContext.CurrentTime);

        var buyTradesInCoolDown = trades.Any(x =>
            x.Side == OrderSide.Buy &&
            x.Timestamp.AddMinutes(tradingContext.BuyCoolDownPeriodInMinutes) >= tradingContext.CurrentTime);

        return buyTradesInCoolDown == false;
    }

    private async Task Sell(IEnumerable<Market> marketsToSell)
    {
        foreach (var market in marketsToSell)
            await sellManager.Sell(market);
    }

    private async Task<Dictionary<Market, Amount>> DetermineMarketsToBuy(
        Dictionary<Market, RecommendatorScore> marketRecommendations)
    {
        var marketsToBuy = GetMarketsWhereBuyRecommendationExceedsMargin(marketRecommendations);

        var budgetForMarkets = await DetermineBudgetForMarket(marketsToBuy);
        budgetForMarkets = GetMarketsWhereAvailableBalanceExceedsMinimumQuoteAmount(budgetForMarkets);

        foreach (var (market, _) in budgetForMarkets)
            logger.LogInformation("Buy recommendation for {Market}, with a score of {Score}", market.Name.Value,
                marketsToBuy.First(x => x.Key == market).Value.Value.ToString("0.00"));

        return budgetForMarkets;
    }

    private Dictionary<Market, RecommendatorScore> GetMarketsWhereBuyRecommendationExceedsMargin(
        Dictionary<Market, RecommendatorScore> recommendations)
    {
        return recommendations
            .Where(x => x.Value >= tradingContext.BuyMargin)
            .ToDictionary(x => x.Key, x => x.Value);
    }

    private async Task<IDictionary<Market, RecommendatorScore>> DetermineMarketsToSell(
        Dictionary<Market, RecommendatorScore> recommendations)
    {
        var marketsToSell = GetMarketsWhereSellRecommendationExceedsMargin(recommendations);

        marketsToSell = await GetMarketsWhereBalanceExceedsMinimumBaseAmount(marketsToSell);

        foreach (var (market, score) in marketsToSell)
            logger.LogInformation("Sell recommendation for {Market}, with a score of {Score}", market.Name.Value,
                score.Value.ToString("0.00"));

        return marketsToSell;
    }

    private async Task<Dictionary<Market, RecommendatorScore>>
        GetMarketsWhereBalanceExceedsMinimumBaseAmount(
            Dictionary<Market, RecommendatorScore> marketsToSell)
    {
        var marketsToSellWithEnoughBalance = new Dictionary<Market, RecommendatorScore>();

        foreach (var (market, recommendatorScore) in marketsToSell)
        {
            var balance = await GetTotalBalanceForAsset(market.Name.BaseSymbol);
            if (market.MinimumBaseAmount <= balance) marketsToSellWithEnoughBalance.Add(market, recommendatorScore);
        }

        return marketsToSellWithEnoughBalance;
    }

    private static Dictionary<Market, Amount> GetMarketsWhereAvailableBalanceExceedsMinimumQuoteAmount(
        Dictionary<Market, Amount> marketsWithBudget)
    {
        return marketsWithBudget
            .Where(marketWithBudget => marketWithBudget.Key.MinimumQuoteAmount <= marketWithBudget.Value)
            .ToDictionary(x => x.Key, x => x.Value);
    }

    private Dictionary<Market, RecommendatorScore> GetMarketsWhereSellRecommendationExceedsMargin(
        Dictionary<Market, RecommendatorScore> recommendations)
    {
        return recommendations
            .Where(x => x.Value <= tradingContext.SellMargin)
            .ToDictionary(x => x.Key, x => x.Value);
    }

    private async Task<Dictionary<Market, RecommendatorScore>> GetRecommendations()
    {
        var marketsToEvaluate = (await GetMarketsToEvaluate()).ToList();

        LogMarketsToEvaluate(marketsToEvaluate);

        var marketRecommendations =
            await Task.WhenAll(
                marketsToEvaluate.Select(async market => (market,
                    recommendation: await recommendationCalculator.CalculateRecommendation(market))));

        return marketRecommendations.ToDictionary(x => x.market, x => x.recommendation);
    }

    private async Task<IEnumerable<Market>> GetMarketsToEvaluate()
    {
        var marketsToWatch = new List<Market>();

        foreach (var market in tradingContext.MarketsToWatch)
            marketsToWatch.Add(await exchangeService.GetMarketAsync(market));

        return marketsToWatch;
    }

    private void LogMarketsToEvaluate(List<Market> marketsToWatch)
    {
        logger.LogInformation("Markets to evaluate:");

        foreach (var market in marketsToWatch)
            logger.LogInformation("{Market}", market.Name.Value);
    }
}