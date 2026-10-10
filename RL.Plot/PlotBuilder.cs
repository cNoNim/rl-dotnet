using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using RL.Core;
using RL.Generators;

namespace RL.Plot;

public struct PlotBuilder(string title)
{
    private readonly List<AxisConfigurator> _axisConfigurators = [];

    private readonly List<(IEnumerable<DataPoint> series, SeriesConfigurator? configurator)> _signals = [];

    public PlotBuilder ConfigureAxis(Func<AxisConfigurator, AxisConfigurator> configure)
    {
        _axisConfigurators.Add(configure(new AxisConfigurator()));
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
        IEnumerable<T> array,
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

    public PlotModel Build()
    {
        var model = new PlotModel
        {
            Title = title
        };

        model.Legends.Add(new Legend
        {
            LegendPlacement = LegendPlacement.Outside,
            LegendPosition = LegendPosition.BottomCenter
        });

        foreach (var configurator in _axisConfigurators)
            model.Axes.Add(CreateAxis(configurator));

        foreach (var (signal, configurator) in _signals)
            AddSeries(model, new LineSeries { ItemsSource = signal }, configurator);

        return model;
    }

    private static void AddSeries(PlotModel model, LineSeries series, SeriesConfigurator? configurator)
    {
        if (configurator != null)
        {
            if (configurator.Title != null)
                series.Title = configurator.Title;
            if (configurator.Color != null)
                series.Color = OxyColor.Parse(configurator.Color);
        }

        model.Series.Add(series);
    }

    private static Axis CreateAxis(AxisConfigurator configurator)
    {
        var axis = new LinearAxis();

        if (configurator.Position != null)
            axis.Position = configurator.Position.Value;

        if (configurator.Title != null)
            axis.Title = configurator.Title;

        if (configurator.Key != null)
            axis.Key = configurator.Key;

        if (configurator.Min != null)
            axis.Minimum = configurator.Min.Value;

        if (configurator.Max != null)
            axis.Maximum = configurator.Max.Value;

        return axis;
    }
}
