using KrieptoBot.Domain.Recommendation.ValueObjects;
using Microsoft.FSharp.Core;
using Plotly.NET;
using Color = System.Drawing.Color;

namespace KrieptoBot.DataVisualizer.Extensions;

public static class CandleChartExtensions
{
    public static GenericChart AddSupportLevels(this GenericChart chart,
        IEnumerable<SupportResistanceLevel> levels, DateTime from, DateTime to)
    {
        var levelsToPlot = levels.Select(x => new List<Tuple<DateTime, decimal>>
        {
            new(x.From, x.Value),
            new(to, x.Value)
        });

        var charts = levelsToPlot.Select(level =>
                Chart2D.Chart.Line<DateTime, decimal, string>(level,
                    MarkerColor: Plotly.NET.Color.fromString("Gray")))
            .ToList();

        charts.Add(chart);

        return Chart.Combine(charts);
    }

    public static GenericChart AddSubChartLine(this GenericChart chart,
        IDictionary<DateTime, decimal> values)
    {
        var valuesToPlot = values.Select(x => new Tuple<DateTime, decimal>(x.Key, x.Value));

        var charts = new List<GenericChart>
        {
            chart,
            Chart2D.Chart.Line<DateTime, decimal, string>(valuesToPlot)
                .WithYAxisStyle(Title.init(), ZeroLine: false)
        };

        return Chart.Combine(charts);
    }

    public static GenericChart AddLineChart(this GenericChart chart,
        IDictionary<DateTime, decimal> values, Color color = default)
    {
        color = color.IsEmpty ? Color.Red : color;

        var valuesToPlot = values.Select(x => new Tuple<DateTime, decimal>(x.Key, x.Value));

        var charts = new List<GenericChart>
        {
            chart,
            Chart2D.Chart.Line<DateTime, decimal, string>(valuesToPlot,
                    LineColor: new FSharpOption<Plotly.NET.Color>(
                        Plotly.NET.Color.fromARGB(color.A, color.R, color.G, color.B)))
                .WithYAxisStyle(Title.init(), ZeroLine: false).WithConfig(Config.init())
        };

        return Chart.Combine(charts);
    }
}