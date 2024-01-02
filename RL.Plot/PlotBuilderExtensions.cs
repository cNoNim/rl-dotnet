using System.Numerics;
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
}