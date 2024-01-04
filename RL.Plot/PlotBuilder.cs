using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.ImageSharp;
using OxyPlot.Legends;
using OxyPlot.Series;
using RL.Core;
using RL.Generators;

namespace RL.Plot;

public ref struct PlotBuilder(string title)
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
    ) where TList : IGenerator<T> where T : INumberBase<T>
    {
        _signals.Add((
            data.Select<TList, T, DataPoint>((v, i) =>
                new DataPoint(i, double.CreateChecked(v))).AsEnumerable(),
            configure?.Invoke(new SeriesConfigurator())
        ));
        return this;
    }

    public PlotBuilder Signal<T>(
        T[] array,
        Func<SeriesConfigurator, SeriesConfigurator>? configure = null
    ) where T : INumberBase<T>
    {
        _signals.Add((
            array.Select((v, i) =>
                new DataPoint(i, double.CreateChecked(v))).AsEnumerable(),
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

        model.Legends.Add(new Legend
        {
            LegendPlacement = LegendPlacement.Inside,
            LegendPosition = LegendPosition.RightTop
        });

        if (_xAxisConfigurator != null)
            model.Axes.Add(CreateAxis(_xAxisConfigurator.Value, AxisPosition.Left));
        if (_yAxisConfigurator != null)
            model.Axes.Add(CreateAxis(_yAxisConfigurator.Value, AxisPosition.Bottom));

        foreach (var (signal, configurator) in _signals)
            BuildSignal(signal, configurator);

        return model;

        void BuildSignal(IEnumerable<DataPoint> signal, SeriesConfigurator? configurator)
        {
            var lineSeries = new LineSeries
            {
                ItemsSource = signal
            };

            if (configurator != null)
            {
                if (configurator.Title != null)
                    lineSeries.Title = configurator.Title;
                if (configurator.Color != null)
                    lineSeries.Color = OxyColor.Parse(configurator.Color);
            }

            model.Series.Add(lineSeries);
        }
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