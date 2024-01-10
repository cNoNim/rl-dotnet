using System.Runtime.CompilerServices;
using System.Threading;
using RL.Core;
using RL.Generators;

namespace RL.Progress;

public static class ProgressExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ProgressGenerator<TakeGenerator<TG, T>, T> Progress<TG, T>(this TakeGenerator<TG, T> generator,
        string? title = null, CancellationToken token = default)
        where TG : IGenerator<T> =>
        Progress<TakeGenerator<TG, T>, T>(generator, title, token);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ProgressGenerator<IGenerator<T>, T> Progress<T>(this IGenerator<T> generator, string? title = null,
        CancellationToken token = default) =>
        Progress<IGenerator<T>, T>(generator, title, token);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ProgressGenerator<TG, T> Progress<TG, T>(this TG generator, string? title = null,
        CancellationToken token = default)
        where TG : IGenerator<T> =>
        new(generator, title, token);
}