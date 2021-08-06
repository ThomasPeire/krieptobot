using KrieptoBod.Exchange.Bitvavo.Model;
using KrieptoBod.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KrieptoBod.Exchange.Bitvavo.Helpers
{
    public static class Extensions
    {
        public static Asset ConvertToKrieptoBodModel(this AssetDto dto)
        {
            return new Asset()
            {
                Symbol = dto.Symbol,
                Name = dto.Name,
                Decimals = dto.Decimals,
                DepositFee = decimal.Parse(dto.DepositFee),
                DepositConfirmations = dto.DepositConfirmations,
                DepositStatus = dto.DepositStatus,
                WithdrawalFee = decimal.Parse(dto.WithdrawalFee),
                WithdrawalMinAmount = decimal.Parse(dto.WithdrawalMinAmount),
                WithdrawalStatus = dto.WithdrawalStatus,
                Networks = dto.Networks,
                Message = dto.Message,
            };
        }

        public static Balance ConvertToKrieptoBodModel(this BalanceDto dto)
        {
            return new Balance()
            {
                Symbol = dto.Symbol,
                Available = decimal.Parse(dto.Available),
                InOrder = decimal.Parse(dto.InOrder),
            };
        }

        public static Candle ConvertToKrieptoBodModel(this CandleDto dto)
        {
            return new Candle()
            {
                //TODO: correct mapping
                TimeStamp = DateTime.Now,
                //High = decimal.Parse(dto.something),
                //Low = decimal.Parse(dto.something),
                //Open = decimal.Parse(dto.something),
                //Close = decimal.Parse(dto.something),
                //Volume = decimal.Parse(dto.something),
            };
        }

        public static Market ConvertToKrieptoBodModel(this MarketDto dto)
        {
            return new Market()
            {
                MarketName = dto.MarketName,
                Status = dto.Status,
                Base = dto.Base,
                Quote = dto.Quote,
                PricePrecision = int.Parse(dto.PricePrecision),
                MinOrderInQuoteAsset = decimal.Parse(dto.MinOrderInQuoteAsset),
                MinOrderInBaseAsset = decimal.Parse(dto.MinOrderInBaseAsset),
                OrderTypes = dto.OrderTypes,
            };
        }

        public static Order ConvertToKrieptoBodModel(this OrderDto dto)
        {
            return new Order()
            {
                OrderId = dto.OrderId,
                Market = dto.Market,
                Created = new DateTime(dto.Created),
                Updated = new DateTime(dto.Updated),
                Status = dto.Status,
                Side = dto.Side,
                OrderType = dto.OrderType,
                Amount = decimal.Parse(dto.Amount),
                AmountRemaining = decimal.Parse(dto.AmountRemaining),
                Price = decimal.Parse(dto.Price),
                AmountQuote = decimal.Parse(dto.AmountQuote),
                AmountQuoteRemaining = decimal.Parse(dto.AmountQuoteRemaining),
                OnHold = decimal.Parse(dto.OnHold),
                OnHoldCurrency = dto.OnHoldCurrency,
                TriggerPrice = decimal.Parse(dto.TriggerPrice),
                TriggerAmount = decimal.Parse(dto.TriggerAmount),
                TriggerType = dto.TriggerType,
                TriggerReference = dto.TriggerReference,
                FilledAmount = decimal.Parse(dto.FilledAmount),
                FilledAmountQuote = decimal.Parse(dto.FilledAmountQuote),
                FeePaid = decimal.Parse(dto.FeePaid),
                FeeCurrency = dto.FeeCurrency,
                Fills = dto.Fills.ConvertToKrieptoBodModel(),
                SelfTradePrevention = dto.SelfTradePrevention,
                Visible = dto.Visible,
                TimeInForce = dto.TimeInForce,
                PostOnly = dto.PostOnly,
                DisableMarketProtection = dto.DisableMarketProtection,
            };
        }

        public static Fill ConvertToKrieptoBodModel(this FillDto dto)
        {
            return new Fill()
            {
                Id = dto.Id,
                Timestamp = new DateTime(dto.Timestamp),
                Amount = decimal.Parse(dto.Amount),
                Price = decimal.Parse(dto.Price),
                Taker = dto.Taker,
                Fee = decimal.Parse(dto.Fee),
                FeeCurrency = dto.FeeCurrency,
                Settled = dto.Settled,
            };
        }

        public static Trade ConvertToKrieptoBodModel(this TradeDto dto)
        {
            return new Trade()
            {
                Timestamp = new DateTime(dto.Timestamp),
                Id = dto.Id,
                Amount = decimal.Parse(dto.Amount),
                Price = decimal.Parse(dto.Price),
                Side = dto.Side,
            };
        }

        public static IEnumerable<Asset> ConvertToKrieptoBodModel(this IEnumerable<AssetDto> dtoList)
        {
            return dtoList.Select(dto => dto.ConvertToKrieptoBodModel());
        }

        public static IEnumerable<Balance> ConvertToKrieptoBodModel(this IEnumerable<BalanceDto> dtoList)
        {
            return dtoList.Select(dto => dto.ConvertToKrieptoBodModel());
        }

        public static IEnumerable<Candle> ConvertToKrieptoBodModel(this IEnumerable<CandleDto> dtoList)
        {
            return dtoList.Select(dto => dto.ConvertToKrieptoBodModel());
        }

        public static IEnumerable<Market> ConvertToKrieptoBodModel(this IEnumerable<MarketDto> dtoList)
        {
            return dtoList.Select(dto => dto.ConvertToKrieptoBodModel());
        }

        public static IEnumerable<Order> ConvertToKrieptoBodModel(this IEnumerable<OrderDto> dtoList)
        {
            return dtoList.Select(dto => dto.ConvertToKrieptoBodModel());
        }

        public static IEnumerable<Fill> ConvertToKrieptoBodModel(this IEnumerable<FillDto> dtoList)
        {
            return dtoList.Select(dto => dto.ConvertToKrieptoBodModel());
        }

        public static IEnumerable<Trade> ConvertToKrieptoBodModel(this IEnumerable<TradeDto> dtoList)
        {
            return dtoList.Select(dto => dto.ConvertToKrieptoBodModel());
        }
    }
}