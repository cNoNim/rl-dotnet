using System.Runtime.CompilerServices;

namespace RL.MDArrays;

public readonly partial struct Array1D<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> operator +(Array1D<T> left, Array1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l + r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> operator +(Array1D<T> left, T right) =>
        CombineToTensor(left, right, static (l, r) => l + r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> operator +(T left, Array1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l + r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> operator -(Array1D<T> left, Array1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l - r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> operator -(Array1D<T> left, T right) =>
        CombineToTensor(left, right, static (l, r) => l - r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> operator -(T left, Array1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l - r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> operator *(Array1D<T> left, Array1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l * r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> operator *(Array1D<T> left, T right) =>
        CombineToTensor(left, right, static (l, r) => l * r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> operator *(T left, Array1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l * r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> operator /(Array1D<T> left, Array1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l / r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> operator /(Array1D<T> left, T right) =>
        CombineToTensor(left, right, static (l, r) => l / r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> operator /(T left, Array1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l / r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> operator %(Array1D<T> left, Array1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l % r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> operator %(Array1D<T> left, T right) =>
        CombineToTensor(left, right, static (l, r) => l % r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> operator %(T left, Array1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l % r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array1D<T> operator +(Array1D<T> value) =>
        value;

    public static Array1D<T> operator -(Array1D<T> value)
    {
        var result = new Array1D<T>(value.Shape);
        for (var i = 0; i < value.Shape; i++)
            result[i] = -value[i];
        return result;
    }
}