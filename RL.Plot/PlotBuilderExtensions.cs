using System;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.ImageSharp;
using RL.Core;

namespace RL.Plot;

public static class PlotBuilderExtensions
{
    public static PlotBuilder Signal<T>(
        this PlotBuilder builder,
        IGenerator<T> data,
        Func<SeriesConfigurator, SeriesConfigurator>? configure = null
    ) where T : INumberBase<T> =>
        builder.Signal<IGenerator<T>, T>(data, configure);

    public static void ToPng(this PlotBuilder builder, string path, int width = 1280, int height = 720)
    {
        var model = builder.Build();
        model.Background = OxyColors.White;

        using var stream = File.Create(path);

        var exporter = new PngExporter(width, height);
        exporter.Export(model, stream);
    }

    public static void ToSvg(this PlotBuilder builder, string path, int width = 1280, int height = 720)
    {
        var model = builder.Build();

        using var stream = File.Create(path);

        var exporter = new SvgExporter { Width = width, Height = height };
        exporter.Export(model, stream);
    }

    public static Task Show(
        this PlotBuilder builder,
        out PlotModel model,
        int width = 1280,
        int height = 720,
        CancellationToken token = default
    )
    {
        var m = builder.Build();
        model = m;
        return Task.Run(() =>
        {
            var window = new SimpleWindow(m.Title, width, height, token);
            window.AddSubView(new PlotView(m));
            window.Run();
        }, token);
    }
}
