using KrieptoBot.Infrastructure.Bitvavo.Dtos;
using KrieptoBot.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace KrieptoBot.Infrastructure.Bitvavo.Extensions
{
    public static class Mappings
    {
        public static Asset ConvertToKrieptoBotModel(this AssetDto dto)
        {
            return new Asset
            {
                Symbol = dto.Symbol,
                Name = dto.Name,
                Decimals = dto.Decimals,
                DepositFee = decimal.Parse(dto.DepositFee ?? "0", CultureInfo.InvariantCulture),
                DepositConfirmations = dto.DepositConfirmations,
                DepositStatus = dto.DepositStatus,
                WithdrawalFee = decimal.Parse(dto.WithdrawalFee ?? "0", CultureInfo.InvariantCulture),
                WithdrawalMinAmount = decimal.Parse(dto.WithdrawalMinAmount ?? "0", CultureInfo.InvariantCulture),
                WithdrawalStatus = dto.WithdrawalStatus,
                Networks = dto.Networks,
                Message = dto.Message,
            };
        }

        public static Balance ConvertToKrieptoBotModel(this BalanceDto dto)
        {
            return new Balance
            {
                Symbol = dto.Symbol,
                Available = decimal.Parse(dto.Available ?? "0", CultureInfo.InvariantCulture),
                InOrder = decimal.Parse(dto.InOrder ?? "0", CultureInfo.InvariantCulture),
            };
        }

        public static Candle ConvertToKrieptoBotModel(this CandleDto dto)
        {
            return new Candle
            {
                TimeStamp = dto.TimeStamp,
                High = dto.High,
                Low = dto.Low,
                Open = dto.Open,
                Close = dto.Close,
                Volume = dto.Volume,
            };
        }

        public static Market ConvertToKrieptoBotModel(this MarketDto dto)
        {
            return new Market
            {
                MarketName = dto.MarketName,
                Status = dto.Status,
                Base = dto.Base,
                Quote = dto.Quote,
                PricePrecision = int.Parse(dto.PricePrecision ?? "0", CultureInfo.InvariantCulture),
                MinOrderInQuoteAsset = decimal.Parse(dto.MinOrderInQuoteAsset ?? "0", CultureInfo.InvariantCulture),
                MinOrderInBaseAsset = decimal.Parse(dto.MinOrderInBaseAsset ?? "0", CultureInfo.InvariantCulture),
                OrderTypes = dto.OrderTypes,
            };
        }

        public static Order ConvertToKrieptoBotModel(this OrderDto dto)
        {
            return new Order
            {
                OrderId = dto.OrderId,
                Market = dto.Market,
                Created = new DateTime(dto.Created),
                Updated = new DateTime(dto.Updated),
                Status = dto.Status,
                Side = dto.Side,
                OrderType = dto.OrderType,
                Amount = decimal.Parse(dto.Amount ?? "0", CultureInfo.InvariantCulture),
                AmountRemaining = decimal.Parse(dto.AmountRemaining ?? "0", CultureInfo.InvariantCulture),
                Price = decimal.Parse(dto.Price ?? "0", CultureInfo.InvariantCulture),
                AmountQuote = decimal.Parse(dto.AmountQuote ?? "0", CultureInfo.InvariantCulture),
                AmountQuoteRemaining = decimal.Parse(dto.AmountQuoteRemaining ?? "0", CultureInfo.InvariantCulture),
                OnHold = decimal.Parse(dto.OnHold ?? "0", CultureInfo.InvariantCulture),
                OnHoldCurrency = dto.OnHoldCurrency,
                TriggerPrice = decimal.Parse(dto.TriggerPrice ?? "0", CultureInfo.InvariantCulture),
                TriggerAmount = decimal.Parse(dto.TriggerAmount ?? "0", CultureInfo.InvariantCulture),
                TriggerType = dto.TriggerType,
                TriggerReference = dto.TriggerReference,
                FilledAmount = decimal.Parse(dto.FilledAmount ?? "0", CultureInfo.InvariantCulture),
                FilledAmountQuote = decimal.Parse(dto.FilledAmountQuote ?? "0", CultureInfo.InvariantCulture),
                FeePaid = decimal.Parse(dto.FeePaid ?? "0", CultureInfo.InvariantCulture),
                FeeCurrency = dto.FeeCurrency,
                Fills = dto.Fills?.ConvertToKrieptoBotModel(),
                SelfTradePrevention = dto.SelfTradePrevention,
                Visible = dto.Visible,
                TimeInForce = dto.TimeInForce,
                PostOnly = dto.PostOnly,
                DisableMarketProtection = dto.DisableMarketProtection,
            };
        }

        public static Fill ConvertToKrieptoBotModel(this FillDto dto)
        {
            return new Fill
            {
                Id = dto.Id,
                Timestamp = new DateTime(dto.Timestamp),
                Amount = decimal.Parse(dto.Amount ?? "0", CultureInfo.InvariantCulture),
                Price = decimal.Parse(dto.Price ?? "0", CultureInfo.InvariantCulture),
                Taker = dto.Taker,
                Fee = decimal.Parse(dto.Fee ?? "0", CultureInfo.InvariantCulture),
                FeeCurrency = dto.FeeCurrency,
                Settled = dto.Settled,
            };
        }

        public static Trade ConvertToKrieptoBotModel(this TradeDto dto)
        {
            return new Trade
            {
                Timestamp = new DateTime(dto.Timestamp),
                Id = dto.Id,
                Amount = decimal.Parse(dto.Amount ?? "0", CultureInfo.InvariantCulture),
                Price = decimal.Parse(dto.Price ?? "0", CultureInfo.InvariantCulture),
                Side = dto.Side,
            };
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

        public static IEnumerable<Fill> ConvertToKrieptoBotModel(this IEnumerable<FillDto> dtoList)
        {
            return dtoList.Select(dto => dto.ConvertToKrieptoBotModel());
        }

        public static IEnumerable<Trade> ConvertToKrieptoBotModel(this IEnumerable<TradeDto> dtoList)
        {
            return dtoList.Select(dto => dto.ConvertToKrieptoBotModel());
        }
    }
}