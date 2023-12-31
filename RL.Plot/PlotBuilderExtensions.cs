using System.Numerics;
using RL.Core;

namespace RL.Plot;

public static class PlotBuilderExtensions
{
    public static PlotBuilder Signal<T, TTo>(
        this PlotBuilder builder,
        SelectList<IReadOnlyList<T>, T, TTo> data,
        Func<SeriesConfigurator, SeriesConfigurator>? configure = null
    ) where TTo : INumberBase<TTo> =>
        builder.Signal<SelectList<IReadOnlyList<T>, T, TTo>, TTo>(data, configure);

    public static PlotBuilder Signal<T>(
        this PlotBuilder builder,
        IReadOnlyList<T> data,
        Func<SeriesConfigurator, SeriesConfigurator>? configure = null
    ) where T : INumberBase<T> =>
        builder.Signal<IReadOnlyList<T>, T>(data, configure);
}
