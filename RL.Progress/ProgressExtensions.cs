using System.Runtime.CompilerServices;
using RL.Core;
using RL.Generators;

namespace RL.Progress;

public static class ProgressExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ProgressGenerator<TakeGenerator<TG, T>, T> Progress<TG, T>(this TakeGenerator<TG, T> generator) 
        where TG : IGenerator<T> => 
        Progress<TakeGenerator<TG, T>, T>(generator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ProgressGenerator<IGenerator<T>, T> Progress<T>(this IGenerator<T> generator) => 
        Progress<IGenerator<T>, T>(generator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ProgressGenerator<TG, T> Progress<TG, T>(this TG generator) 
        where TG : IGenerator<T> => new(generator);
}