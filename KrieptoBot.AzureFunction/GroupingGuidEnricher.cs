using System;
using Serilog.Core;
using Serilog.Events;

namespace KrieptoBot.AzureFunction;

public class GroupingGuidEnricher : ILogEventEnricher
{
    public static Guid CurrentGroupingGuid { get; set; } = Guid.NewGuid();

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("CorrelationId", CurrentGroupingGuid));
    }
}