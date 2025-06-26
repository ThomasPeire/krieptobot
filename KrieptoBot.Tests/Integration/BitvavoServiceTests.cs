using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AwesomeAssertions;
using KrieptoBot.Infrastructure.Bitvavo;
using KrieptoBot.Infrastructure.Bitvavo.Dtos;
using KrieptoBot.Infrastructure.Bitvavo.Extensions;
using KrieptoBot.Infrastructure.Bitvavo.Extensions.Helper;
using KrieptoBot.Infrastructure.Bitvavo.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace KrieptoBot.Tests.Integration;

[TestFixture]
public class ExchangeServiceTests : IntegrationTestBase
{
    private IBitvavoApi _bitvavoApi;
    private BitvavoService _bitvavoService;
    private WireMockServer _wireMockServer;
    private IMemoryCache _memoryCache;

    private readonly string _assetsJson = File.ReadAllText(@"./MockData/Bitvavo/assets.json");
    private readonly string _balancesJson = File.ReadAllText(@"./MockData/Bitvavo/balances.json");
    private readonly string _candlesJson = File.ReadAllText(@"./MockData/Bitvavo/candles_btc-eur.json");
    private readonly string _marketsJson = File.ReadAllText(@"./MockData/Bitvavo/markets.json");
    private readonly string _ordersJson = File.ReadAllText(@"./MockData/Bitvavo/orders_btc-eur.json");
    private readonly string _tradesJson = File.ReadAllText(@"./MockData/Bitvavo/trades_btc-eur.json");

    [OneTimeSetUp]
    public void Setup()
    {
        _bitvavoApi = TestServer.Services.GetRequiredService<IBitvavoApi>();
        _memoryCache = TestServer.Services.GetRequiredService<IMemoryCache>();
        _bitvavoService = new BitvavoService(_bitvavoApi, _memoryCache, new Mock<ILogger<BitvavoService>>().Object);
        _wireMockServer = TestServer.Services.GetRequiredService<WireMockServer>();
    }

    [Test]
    public async Task GetBalances_ShouldReturn_Balances()
    {
        var balances = JsonConvert.DeserializeObject<IEnumerable<BalanceDto>>(_balancesJson);

        _wireMockServer
            .Given(Request.Create().WithPath("/v2/balance"))
            .RespondWith(Response.Create().WithBodyAsJson(balances).WithStatusCode(HttpStatusCode.OK));

        var result = await _bitvavoService.GetBalanceAsync();

        result.Should().BeEquivalentTo(balances.ConvertToKrieptoBotModel());
    }

    [Test]
    public async Task GetBalance_ShouldReturn_Balance()
    {
        var balance = JsonConvert.DeserializeObject<IEnumerable<BalanceDto>>(_balancesJson).First();
        var balanceAsList = new List<BalanceDto> { balance };

        _wireMockServer
            .Given(Request.Create().WithPath("/v2/balance").WithParam("symbol", balance.Symbol))
            .RespondWith(Response.Create().WithBodyAsJson(balanceAsList).WithStatusCode(HttpStatusCode.OK));

        var result = await _bitvavoService.GetBalanceAsync(balance.Symbol);

        result.Should().BeEquivalentTo(balance.ConvertToKrieptoBotModel());
    }

    [Test]
    public async Task GetCandles_ShouldReturn_Candles()
    {
        var candleJArrayDtos = JsonConvert.DeserializeObject<IEnumerable<JArray>>(_candlesJson);
        var candlesArray = new JArray(candleJArrayDtos);
        var candleDtos = candleJArrayDtos.Select(x =>
            new CandleDto
            {
                TimeStamp = x.Value<long>(0),
                Open = x.Value<decimal>(1),
                High = x.Value<decimal>(2),
                Low = x.Value<decimal>(3),
                Close = x.Value<decimal>(4),
                Volume = x.Value<decimal>(5)
            });

        const string market = "BTC-EUR";

        _wireMockServer
            .Given(Request.Create().WithPath($"/v2/{market}/candles"))
            .RespondWith(Response.Create().WithBodyAsJson(candlesArray)
                .WithStatusCode(HttpStatusCode.OK));

        var result = await _bitvavoService.GetCandlesAsync(market);

        result.Should().BeEquivalentTo(candleDtos.ConvertToKrieptoBotModel());
    }


    [Test]
    public async Task GetMarkets_ShouldReturn_Markets()
    {
        var markets = JsonConvert.DeserializeObject<IEnumerable<MarketDto>>(_marketsJson);

        _wireMockServer
            .Given(Request.Create().WithPath("/v2/markets"))
            .RespondWith(Response.Create().WithBodyAsJson(markets).WithStatusCode(HttpStatusCode.OK));

        var result = await _bitvavoService.GetMarketsAsync();

        result.Should().BeEquivalentTo(markets.ConvertToKrieptoBotModel());
    }

    [Test]
    public async Task GetMarket_ShouldReturn_Market()
    {
        var market = JsonConvert.DeserializeObject<IEnumerable<MarketDto>>(_marketsJson).First();

        _wireMockServer
            .Given(Request.Create().WithPath("/v2/markets").WithParam("market", market.MarketName))
            .RespondWith(Response.Create().WithBodyAsJson(market).WithStatusCode(HttpStatusCode.OK));

        var result = await _bitvavoService.GetMarketAsync(market.MarketName);

        result.Should().BeEquivalentTo(market.ConvertToKrieptoBotModel());
    }


    [Test]
    public async Task GetAssets_ShouldReturn_Assets()
    {
        var assets = JsonConvert.DeserializeObject<IEnumerable<AssetDto>>(_assetsJson);

        _wireMockServer
            .Given(Request.Create().WithPath("/v2/assets"))
            .RespondWith(Response.Create().WithBodyAsJson(assets).WithStatusCode(HttpStatusCode.OK));

        var result = await _bitvavoService.GetAssetsAsync();

        result.Should().BeEquivalentTo(assets.ConvertToKrieptoBotModel());
    }

    [Test]
    public async Task GetAsset_ShouldReturn_Asset()
    {
        var asset = JsonConvert.DeserializeObject<IEnumerable<AssetDto>>(_assetsJson).First();

        _wireMockServer
            .Given(Request.Create().WithPath("/v2/assets").WithParam("symbol", asset.Symbol))
            .RespondWith(Response.Create().WithBodyAsJson(asset).WithStatusCode(HttpStatusCode.OK));

        var result = await _bitvavoService.GetAssetAsync(asset.Symbol);

        result.Should().BeEquivalentTo(asset.ConvertToKrieptoBotModel());
    }

    [Test]
    public async Task GetTrades_ShouldReturn_Trades()
    {
        var trades = JsonConvert.DeserializeObject<IEnumerable<TradeDto>>(_tradesJson);

        _wireMockServer
            .Given(
                Request.Create().WithPath("/v2/trades")
                    .WithParam("market", "btc-eur"))
            .RespondWith(Response.Create().WithBodyAsJson(trades).WithStatusCode(HttpStatusCode.OK));

        var result = await _bitvavoService.GetTradesAsync("btc-eur");

        result.Should().BeEquivalentTo(trades.ConvertToKrieptoBotModel());
    }

    [Test]
    public async Task GetOrders_ShouldReturn_Orders()
    {
        var orders = JsonConvert.DeserializeObject<IEnumerable<OrderDto>>(_ordersJson);

        _wireMockServer
            .Given(
                Request.Create().WithPath("/v2/orders")
                    .WithParam("market", "btc-eur"))
            .RespondWith(Response.Create().WithBodyAsJson(orders).WithStatusCode(HttpStatusCode.OK));

        var result = await _bitvavoService.GetOrdersAsync("btc-eur");

        result.Should().BeEquivalentTo(orders.ConvertToKrieptoBotModel());
    }

    [Test]
    public async Task GetOrder_ShouldReturn_Order()
    {
        var guid = new Guid("4a7bd126-2d21-4918-96dc-0c8f51760a0b");
        var orderDto = JsonConvert.DeserializeObject<IEnumerable<OrderDto>>(_ordersJson)
            .First(x => x.OrderId == guid.ToString());

        _wireMockServer
            .Given(
                Request.Create().WithPath("/v2/order")
                    .WithParam("market", "btc-eur")
                    .WithParam("orderId", guid.ToString()))
            .RespondWith(Response.Create().WithBodyAsJson(orderDto).WithStatusCode(HttpStatusCode.OK));

        var result = await _bitvavoService.GetOrderAsync("btc-eur", guid);

        result.Should().BeEquivalentTo(orderDto.ConvertToKrieptoBotModel());
    }

    [Test]
    public async Task GetOpenOrderForMarket_ShouldReturn_OpenOrder()
    {
        var ordersDto = JsonConvert.DeserializeObject<IEnumerable<OrderDto>>(_ordersJson);

        _wireMockServer
            .Given(
                Request.Create().WithPath("/v2/ordersOpen")
                    .WithParam("market", "btc-eur"))
            .RespondWith(Response.Create().WithBodyAsJson(ordersDto).WithStatusCode(HttpStatusCode.OK));

        var result = await _bitvavoService.GetOpenOrderAsync("btc-eur");

        result.Should().BeEquivalentTo(ordersDto.ConvertToKrieptoBotModel());
    }

    [Test]
    public async Task GetOpenOrder_ShouldReturn_OpenOrder()
    {
        var ordersDto = JsonConvert.DeserializeObject<IEnumerable<OrderDto>>(_ordersJson);

        _wireMockServer
            .Given(
                Request.Create().WithPath("/v2/ordersOpen"))
            .RespondWith(Response.Create().WithBodyAsJson(ordersDto).WithStatusCode(HttpStatusCode.OK));

        var result = await _bitvavoService.GetOpenOrderAsync();

        result.Should().BeEquivalentTo(ordersDto.ConvertToKrieptoBotModel());
    }

    [Test]
    public async Task PostBuyOrder_ShouldReturn_AddedOrder()
    {
        var orderDto = JsonConvert.DeserializeObject<IEnumerable<OrderDto>>(_ordersJson).First();

        var orderDtoAmount = decimal.Parse(orderDto.Amount, CultureInfo.InvariantCulture).RoundToSignificantDigits(5).ToString(CultureInfo.InvariantCulture);
        var orderDtoPrice = decimal.Parse(orderDto.Price, CultureInfo.InvariantCulture).RoundToSignificantDigits(5).ToString(CultureInfo.InvariantCulture);

        _wireMockServer
            .Given(
                Request.Create()
                    .WithPath("/v2/order")
                    .WithBody(
                        $"{{\"market\":\"{orderDto.Market}\",\"orderType\":\"{orderDto.OrderType}\",\"side\":\"buy\",\"amount\":\"{orderDtoAmount}\",\"price\":\"{orderDtoPrice}\"}}")
                    .UsingPost())
            .RespondWith(Response.Create().WithBodyAsJson(orderDto).WithStatusCode(HttpStatusCode.OK));

        var result = await _bitvavoService.PostBuyOrderAsync(orderDto.Market, orderDto.OrderType,
            decimal.Parse(orderDto.Amount, CultureInfo.InvariantCulture),
            decimal.Parse(orderDto.Price, CultureInfo.InvariantCulture));

        result.Should().BeEquivalentTo(orderDto.ConvertToKrieptoBotModel());
    }

    [Test]
    public async Task PostSellOrder_ShouldReturn_AddedOrder()
    {
        var orderDto = JsonConvert.DeserializeObject<IEnumerable<OrderDto>>(_ordersJson).First();

        var orderDtoAmount = decimal.Parse(orderDto.Amount, CultureInfo.InvariantCulture).RoundToSignificantDigits(5, MidpointRounding.ToZero).ToString(CultureInfo.InvariantCulture);
        var orderDtoPrice = decimal.Parse(orderDto.Price, CultureInfo.InvariantCulture).RoundToSignificantDigits(5, MidpointRounding.ToZero).ToString(CultureInfo.InvariantCulture);

        _wireMockServer
            .Given(
                Request.Create()
                    .WithPath("/v2/order")
                    .WithBody(
                        $"{{\"market\":\"{orderDto.Market}\",\"orderType\":\"{orderDto.OrderType}\",\"side\":\"sell\",\"amount\":\"{orderDtoAmount}\",\"price\":\"{orderDtoPrice}\"}}")
                    .UsingPost())
            .RespondWith(Response.Create().WithBodyAsJson(orderDto).WithStatusCode(HttpStatusCode.OK));

        var result = await _bitvavoService.PostSellOrderAsync(orderDto.Market, orderDto.OrderType,
            decimal.Parse(orderDto.Amount, CultureInfo.InvariantCulture),
            decimal.Parse(orderDto.Price, CultureInfo.InvariantCulture));

        result.Should().BeEquivalentTo(orderDto.ConvertToKrieptoBotModel());
    }

    [Test]
    public async Task GetTickerPrice_ShouldReturn_TickerPrice()
    {
        var tickerDto = new TickerPriceDto { Market = "btc-eur", Price = "0.002" };

        _wireMockServer
            .Given(
                Request.Create().WithPath("/v2/ticker/price")
                    .WithParam("market", "btc-eur"))
            .RespondWith(Response.Create().WithBodyAsJson(tickerDto).WithStatusCode(HttpStatusCode.OK));

        var result = await _bitvavoService.GetTickerPrice("btc-eur");

        result.Should().BeEquivalentTo(tickerDto.ConvertToKrieptoBotModel());
    }
}