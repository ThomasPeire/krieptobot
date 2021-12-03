using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KrieptoBot.Domain.Trading.Entity;
using KrieptoBot.Domain.Trading.ValueObjects;
using KrieptoBot.Infrastructure.Bitvavo.Dtos;

namespace KrieptoBot.Infrastructure.Bitvavo.Extensions
{
    public static class Mappings
    {
        public static Asset ConvertToKrieptoBotModel(this AssetDto dto)
        {
            return
                new Asset
                (
                    new Symbol(dto.Symbol), new AssetName(dto.Name)
                );
        }

        public static Balance ConvertToKrieptoBotModel(this BalanceDto dto)
        {
            return
                new Balance
                (
                    new Symbol(dto.Symbol),
                    new Amount(decimal.Parse(dto.Available ?? "0", CultureInfo.InvariantCulture)),
                    new Amount(decimal.Parse(dto.InOrder ?? "0", CultureInfo.InvariantCulture))
                )
                ;
        }

        public static Candle ConvertToKrieptoBotModel(this CandleDto dto)
        {
            return
                new Candle
                (
                    DateTime.UnixEpoch.AddMilliseconds(dto.TimeStamp),
                    new Price(dto.High),
                    new Price(dto.Low),
                    new Price(dto.Open),
                    new Price(dto.Close),
                    dto.Volume
                );
        }

        public static Market ConvertToKrieptoBotModel(this MarketDto dto)
        {
            return
                new Market
                (
                    new MarketName(dto.Base, dto.Quote),
                    new Amount(decimal.Parse(dto.MinOrderInBaseAsset ?? "0", CultureInfo.InvariantCulture)),
                    new Amount(decimal.Parse(dto.MinOrderInQuoteAsset ?? "0", CultureInfo.InvariantCulture))
                );
        }

        public static Order ConvertToKrieptoBotModel(this OrderDto dto)
        {
            return
                new Order
                (
                    Guid.Parse(dto.OrderId),
                    new MarketName(dto.Market),
                    DateTime.UnixEpoch.AddMilliseconds(dto.Created),
                    DateTime.UnixEpoch.AddMilliseconds(dto.Updated),
                    OrderStatus.FromString(dto.Status),
                    OrderSide.FromString(dto.Side),
                    OrderType.FromString(dto.OrderType),
                    new Amount(decimal.Parse(dto.Amount ?? "0", CultureInfo.InvariantCulture)),
                    new Price(decimal.Parse(dto.Price ?? "0", CultureInfo.InvariantCulture))
                );
        }

        public static Trade ConvertToKrieptoBotModel(this TradeDto dto)
        {
            return
                new Trade
                (
                    Guid.Parse(dto.Id),
                    DateTime.UnixEpoch.AddMilliseconds(dto.Timestamp),
                    new MarketName(dto.Market),
                    new Amount(decimal.Parse(dto.Amount ?? "0", CultureInfo.InvariantCulture)),
                    new Price(decimal.Parse(dto.Price ?? "0", CultureInfo.InvariantCulture)),
                    OrderSide.FromString(dto.Side)
                );
        }

        public static TickerPrice ConvertToKrieptoBotModel(this TickerPriceDto dto)
        {
            return
                new TickerPrice
                (
                    new MarketName(dto.Market),
                    new Price(decimal.Parse(dto.Price ?? "0", CultureInfo.InvariantCulture))
                );
        }

        public static IEnumerable<Asset> ConvertToKrieptoBotModel(this IEnumerable<AssetDto> dtoList)
        {
            return dtoList.Select(dto => dto.ConvertToKrieptoBotModel());
        }

        public static IEnumerable<Balance> ConvertToKrieptoBotModel(this IEnumerable<BalanceDto> dtoList)
        {
            return dtoList.Select(dto => dto.ConvertToKrieptoBotModel());
        }

        public static IEnumerable<Candle> ConvertToKrieptoBotModel(this IEnumerable<CandleDto> dtoList)
        {
            return dtoList.Select(dto => dto.ConvertToKrieptoBotModel());
        }

        public static IEnumerable<Market> ConvertToKrieptoBotModel(this IEnumerable<MarketDto> dtoList)
        {
            return dtoList.Select(dto => dto.ConvertToKrieptoBotModel());
        }

        public static IEnumerable<Order> ConvertToKrieptoBotModel(this IEnumerable<OrderDto> dtoList)
        {
            return dtoList.Select(dto => dto.ConvertToKrieptoBotModel());
        }

        public static IEnumerable<Trade> ConvertToKrieptoBotModel(this IEnumerable<TradeDto> dtoList)
        {
            return dtoList.Select(dto => dto.ConvertToKrieptoBotModel());
        }
    }
}
