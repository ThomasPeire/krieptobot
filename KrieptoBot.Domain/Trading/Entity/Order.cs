﻿using System;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Domain.Trading.Entity;

public class Order : BuildingBlocks.Entity
{
    public Order(Guid id, MarketName marketName, DateTime created, DateTime updated, OrderStatus status,
        OrderSide side,
        OrderType type, Amount amount, Price price, Price triggerPrice) : base(id)
    {
        ArgumentNullException.ThrowIfNull(marketName);
        ArgumentNullException.ThrowIfNull(status);
        ArgumentNullException.ThrowIfNull(side);
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(amount);
        ArgumentNullException.ThrowIfNull(price);
        ArgumentNullException.ThrowIfNull(triggerPrice);

        MarketName = marketName;
        Created = created;
        Updated = updated;
        Status = status;
        Side = side;
        Type = type;
        Amount = amount;
        Price = price;
        TriggerPrice = triggerPrice;
    }

    public MarketName MarketName { get; }
    public DateTime Created { get; }
    public DateTime Updated { get; }
    public OrderStatus Status { get; }
    public OrderSide Side { get; }
    public OrderType Type { get; }
    public Amount Amount { get; }
    public Price Price { get; }
    public Price TriggerPrice { get; }
}