using System.Numerics;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.ImageSharp;
using OxyPlot.Series;
using RL.Core;

namespace RL.Plot;

public class PlotBuilder(string title)
{
    private AxisConfigurator? _xAxisConfigurator;
    private AxisConfigurator? _yAxisConfigurator;

    private readonly List<(IEnumerable<DataPoint> series, SeriesConfigurator? configurator)> _signals = [];

    public PlotBuilder ConfigureXAxis(Func<AxisConfigurator, AxisConfigurator> configure)
    {
        _xAxisConfigurator = configure(new AxisConfigurator());
        return this;
    }

    public PlotBuilder ConfigureYAxis(Func<AxisConfigurator, AxisConfigurator> configure)
    {
        _yAxisConfigurator = configure(new AxisConfigurator());
        return this;
    }


    public PlotBuilder Signal<TList, T>(
        TList data,
        Func<SeriesConfigurator, SeriesConfigurator>? configure = null
    ) where TList : IReadOnlyList<T> where T : INumberBase<T>
    {
        _signals.Add((
            data.Select<TList, T, DataPoint>((v, i) =>
                new DataPoint(i, double.CreateChecked(v))),
            configure?.Invoke(new SeriesConfigurator())
        ));
        return this;
    }

    public void ToSvg(string path, int width = 1280, int height = 720)
    {
        var model = Build();

        using var stream = File.Create(path);

        var exporter = new SvgExporter { Width = width, Height = height };
        exporter.Export(model, stream);
    }

    public void ToPng(string path, int width = 1280, int height = 720)
    {
        var model = Build();
        model.Background = OxyColors.White;

        using var stream = File.Create(path);

        var exporter = new PngExporter(width, height);
        exporter.Export(model, stream);
    }

    private PlotModel Build()
    {
        var model = new PlotModel
        {
            Title = title
        };
        if (_xAxisConfigurator != null)
            model.Axes.Add(CreateAxis(_xAxisConfigurator, AxisPosition.Left));
        if (_yAxisConfigurator != null)
            model.Axes.Add(CreateAxis(_yAxisConfigurator, AxisPosition.Bottom));

        foreach (var (signal, configurator) in _signals)
        {
            var lineSeries = new LineSeries
            {
                ItemsSource = signal
            };
            if (configurator?.Color != null)
                lineSeries.Color = OxyColor.Parse(configurator.Color);
            model.Series.Add(lineSeries);
        }

        return model;
    }

    private static Axis CreateAxis(AxisConfigurator configurator, AxisPosition position)
    {
        var axis = new LinearAxis
        {
            Position = position
        };

        if (configurator.Title != null)
            axis.Title = configurator.Title;

        if (configurator.Min != null)
            axis.Minimum = configurator.Min.Value;

        if (configurator.Max != null)
            axis.Maximum = configurator.Max.Value;

        return axis;
    }
}
