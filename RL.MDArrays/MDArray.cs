using System.Runtime.CompilerServices;

namespace RL.MDArrays;

public static class MDArray
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Flatten(this (int x, int y) shape) => shape.x * shape.y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Flatten(this (int x, int y) index, (int x, int y) shape) => index.x * shape.y + index.y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Flatten(this (Index x, Index y) index, (int x, int y) shape) =>
        Flatten((index.x.GetOffset(shape.x), index.y.GetOffset(shape.y)), shape);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int x, int y) Multi(this int index, (int x, int y) shape) => (index / shape.y, index % shape.y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> Zeroes<T>(this int shape) => new(shape);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array2D<T> Zeroes<T>(this (int x, int y) shape) => new(shape);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array2D<T> Zeroes<T>(int x, int y) => Zeroes<T>((x, y));
}