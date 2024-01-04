using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TTo[] ToArray<TG, T, TTo>(this SelectGenerator<TG, T, TTo> generator)
        where TG : IGenerator<T> =>
        ToArray<SelectGenerator<TG, T, TTo>, TTo>(generator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] ToArray<T>(this IGenerator<T> generator) =>
        ToArray<IGenerator<T>, T>(generator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] ToArray<TG, T>(this TG generator)
        where TG : IGenerator<T>
    {
        var array = new T[generator.Count];
        foreach (var (value, index) in generator.Index<TG, T>())
            array[index] = value;
        return array;
    }
}