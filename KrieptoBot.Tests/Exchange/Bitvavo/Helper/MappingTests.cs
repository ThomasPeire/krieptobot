using System;
using System.Globalization;
using AutoBogus;
using AwesomeAssertions;
using KrieptoBot.Domain.Trading.ValueObjects;
using KrieptoBot.Infrastructure.Bitvavo.Dtos;
using KrieptoBot.Infrastructure.Bitvavo.Extensions;
using NUnit.Framework;

namespace KrieptoBot.Tests.Exchange.Bitvavo.Helper
{
    public class MappingTests
    {
        [Test]
        public void ConvertToKrieptoBotModel_ShouldConvert_BitvavoAssetToKrieptoBotAsset()
        {
            var asset = new AutoFaker<AssetDto>().Generate();

            var result = asset.ConvertToKrieptoBotModel();

            result.Name.Should().BeEquivalentTo(new AssetName(asset.Name));
            result.Symbol.Should().BeEquivalentTo(new Symbol(asset.Symbol));
        }

        [Test]
        public void ConvertToKrieptoBotModel_ShouldConvert_BitvavoBalanceToKrieptoBotBalance()
        {
            var balance = new AutoFaker<BalanceDto>()
                .RuleFor(x => x.InOrder, f => f.Finance.Amount().ToString(CultureInfo.InvariantCulture))
                .RuleFor(x => x.Available, f => f.Finance.Amount().ToString(CultureInfo.InvariantCulture))
                .Generate();
            var result = balance.ConvertToKrieptoBotModel();

            result.Available.Should()
                .BeEquivalentTo(new Amount(decimal.Parse(balance.Available, CultureInfo.InvariantCulture)));
            result.InOrder.Should()
                .BeEquivalentTo(new Amount(decimal.Parse(balance.InOrder, CultureInfo.InvariantCulture)));
            result.Symbol.Should().BeEquivalentTo(new Symbol(balance.Symbol));
        }

        [Test]
        public void ConvertToKrieptoBotModel_ShouldConvert_BitvavoCandleToKrieptoBotCandle()
        {
            var candle = new AutoFaker<CandleDto>()
                    .RuleFor(x => x.TimeStamp, f => ((DateTimeOffset)f.Date.Recent()).ToUnixTimeMilliseconds())
                    .RuleFor(x => x.Close, f => f.Finance.Amount())
                    .RuleFor(x => x.Open, f => f.Finance.Amount())
                    .RuleFor(x => x.High, f => f.Finance.Amount())
                    .RuleFor(x => x.Low, f => f.Finance.Amount())
                    .RuleFor(x => x.Volume, f => f.Finance.Amount())
                    .Generate()
                ;
            var result = candle.ConvertToKrieptoBotModel();

            result.Close.Should()
                .BeEquivalentTo(new Price(candle.Close));
            result.Open.Should()
                .BeEquivalentTo(new Price(candle.Open));
            result.High.Should()
                .BeEquivalentTo(new Price(candle.High));
            result.Low.Should()
                .BeEquivalentTo(new Price(candle.Low));
            result.Volume.Should().Be(candle.Volume);
            result.TimeStamp.Should().Be(DateTime.UnixEpoch.AddMilliseconds(candle.TimeStamp));
        }

        [Test]
        public void ConvertToKrieptoBotModel_ShouldConvert_BitvavoMarketToKrieptoBotMarket()
        {
            var market = new AutoFaker<MarketDto>()
                .RuleFor(x => x.Base, f => f.Finance.Currency().Code)
                .RuleFor(x => x.Quote, f => f.Finance.Currency().Code)
                .RuleFor(x => x.MinOrderInBaseAsset, f => f.Finance.Amount().ToString(CultureInfo.InvariantCulture))
                .RuleFor(x => x.MinOrderInQuoteAsset, f => f.Finance.Amount().ToString(CultureInfo.InvariantCulture))
                .Generate();

            var result = market.ConvertToKrieptoBotModel();

            result.Name.Should().BeEquivalentTo(new MarketName(market.Base, market.Quote));
            result.MinimumBaseAmount.Should()
                .BeEquivalentTo(new Amount(decimal.Parse(market.MinOrderInBaseAsset, CultureInfo.InvariantCulture)));
            result.MinimumQuoteAmount.Should()
                .BeEquivalentTo(new Amount(decimal.Parse(market.MinOrderInQuoteAsset, CultureInfo.InvariantCulture)));
        }

        [Test]
        public void ConvertToKrieptoBotModel_ShouldConvert_BitvavoOrderToKrieptoBotOrder()
        {
            var order = new AutoFaker<OrderDto>()
                .RuleFor(x => x.OrderId, f => f.Random.Guid().ToString())
                .RuleFor(x => x.Created,
                    f => ((DateTimeOffset)f.Date.Recent(1, DateTime.Today.AddDays(-1))).ToUnixTimeMilliseconds())
                .RuleFor(x => x.Updated, f => ((DateTimeOffset)f.Date.Recent(1)).ToUnixTimeMilliseconds())
                .RuleFor(x => x.Market, f => $"{f.Finance.Currency().Code}-{f.Finance.Currency().Code}")
                .RuleFor(x => x.Amount, f => f.Finance.Amount().ToString(CultureInfo.InvariantCulture))
                .RuleFor(x => x.Price, f => f.Finance.Amount().ToString(CultureInfo.InvariantCulture))
                .RuleFor(x => x.Side, f => f.PickRandom("sell", "buy"))
                .RuleFor(x => x.Status,
                    f => f.PickRandom("canceled", "expired", "filled", "new", "rejected", "partiallyfilled",
                        "awaitingtrigger"))
                .RuleFor(x => x.OrderType, f => f.PickRandom("takeProfitLimit", "market", "limit"))
                .RuleFor(x => x.TriggerPrice, f => f.Finance.Amount().ToString(CultureInfo.InvariantCulture))
                .Generate();
            var result = order.ConvertToKrieptoBotModel();

            result.Amount.Should()
                .BeEquivalentTo(new Amount(decimal.Parse(order.Amount, CultureInfo.InvariantCulture)));
            result.Created.Should().Be(DateTime.UnixEpoch.AddMilliseconds(order.Created));
            result.Price.Should().BeEquivalentTo(new Price(decimal.Parse(order.Price, CultureInfo.InvariantCulture)));
            result.Side.Should().BeEquivalentTo(OrderSide.FromString(order.Side));
            result.Status.Should().BeEquivalentTo(OrderStatus.FromString(order.Status));
            result.Type.Should().BeEquivalentTo(OrderType.FromString(order.OrderType));
            result.Updated.Should().Be(DateTime.UnixEpoch.AddMilliseconds(order.Updated));
            result.MarketName.Should().BeEquivalentTo(new MarketName(order.Market));
            result.Id.Should().Be(order.OrderId);
        }

        [Test]
        public void ConvertToKrieptoBotModel_ShouldConvert_BitvavoTradeToKrieptoBotTrade()
        {
            var trade = new AutoFaker<TradeDto>()
                .RuleFor(x => x.Id, f => f.Random.Guid().ToString())
                .RuleFor(x => x.Timestamp, f => ((DateTimeOffset)f.Date.Recent()).ToUnixTimeMilliseconds())
                .RuleFor(x => x.Market, f => $"{f.Finance.Currency().Code}-{f.Finance.Currency().Code}")
                .RuleFor(x => x.Amount, f => f.Finance.Amount().ToString(CultureInfo.InvariantCulture))
                .RuleFor(x => x.Price, f => f.Finance.Amount().ToString(CultureInfo.InvariantCulture))
                .RuleFor(x => x.Side, f => f.PickRandom("sell", "buy"))
                .Generate();

            var result = trade.ConvertToKrieptoBotModel();

            result.Amount.Should()
                .BeEquivalentTo(new Amount(decimal.Parse(trade.Amount, CultureInfo.InvariantCulture)));
            result.Price.Should().BeEquivalentTo(new Price(decimal.Parse(trade.Price, CultureInfo.InvariantCulture)));
            result.Side.Should().BeEquivalentTo(OrderSide.FromString(trade.Side));
            result.Timestamp.Should().Be(DateTime.UnixEpoch.AddMilliseconds(trade.Timestamp));
            result.MarketName.Should().BeEquivalentTo(new MarketName(trade.Market));
            result.Id.Should().Be(trade.Id);
        }

        [Test]
        public void ConvertToKrieptoBotModel_ShouldConvert_BitvavoTickerPriceToKrieptoBotTickerPrice()
        {
            var tickerPriceDto = new AutoFaker<TickerPriceDto>()
                .RuleFor(x => x.Market, f => $"{f.Finance.Currency().Code}-{f.Finance.Currency().Code}")
                .RuleFor(x => x.Price, f => f.Finance.Amount().ToString(CultureInfo.InvariantCulture))
                .Generate();

            var result = tickerPriceDto.ConvertToKrieptoBotModel();

            result.Price.Should()
                .BeEquivalentTo(new Price(decimal.Parse(tickerPriceDto.Price, CultureInfo.InvariantCulture)));
            result.MarketName.Should().BeEquivalentTo(new MarketName(tickerPriceDto.Market));
        }
    }
}
