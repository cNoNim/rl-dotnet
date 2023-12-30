using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace RL.MonteCarlo;

public static class Plot
{
    public static void PlotToSvg(
        this IEnumerable<double> readOnlyList,
        string title,
        string xAxisName,
        string yAxisName,
        string filePath
    )
    {
        var model = new PlotModel
        {
            Title = title,
            Axes =
            {
                new LinearAxis { Position = AxisPosition.Left, Title = xAxisName },
                new LinearAxis { Position = AxisPosition.Bottom, Title = yAxisName }
            },
            Series =
            {
                new LineSeries
                {
                    ItemsSource = readOnlyList.Select((d, i) => new DataPoint(i, d))
                }
            }
        };


        using var stream = File.Create(filePath);

        var exporter = new SvgExporter { Width = 1920, Height = 1080 };
        exporter.Export(model, stream);
    }
}
