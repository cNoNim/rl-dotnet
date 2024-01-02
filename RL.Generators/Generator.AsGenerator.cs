using System.Runtime.CompilerServices;

namespace RL.Generators;

public static partial class Generator
{
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static Array1D<T> AsGenerator<T>(this T[] array) =>
    //     new(array);
    //
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static Array2D<T> AsGenerator<T>(this T[,] array) =>
    //     new(array);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyAdapter<T> AsGenerator<T>(this IReadOnlyList<T> list) =>
        new(list);
}