using System.Runtime.CompilerServices;

namespace RL.Tensors;

public readonly partial struct Tensor1D<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> operator +(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l + r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> operator +(Tensor1D<T> left, T right) =>
        CombineToTensor(left, right, static (l, r) => l + r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> operator +(T left, Tensor1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l + r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> operator -(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l - r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> operator -(Tensor1D<T> left, T right) =>
        CombineToTensor(left, right, static (l, r) => l - r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> operator -(T left, Tensor1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l - r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> operator *(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l * r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> operator *(Tensor1D<T> left, T right) =>
        CombineToTensor(left, right, static (l, r) => l * r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> operator *(T left, Tensor1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l * r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> operator /(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l / r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> operator /(Tensor1D<T> left, T right) =>
        CombineToTensor(left, right, static (l, r) => l / r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> operator /(T left, Tensor1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l / r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> operator %(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l % r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> operator %(Tensor1D<T> left, T right) =>
        CombineToTensor(left, right, static (l, r) => l % r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> operator %(T left, Tensor1D<T> right) =>
        CombineToTensor(left, right, static (l, r) => l % r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> operator +(Tensor1D<T> value) =>
        value;

    public static Tensor1D<T> operator -(Tensor1D<T> value)
    {
        var result = new Tensor1D<T>(value.Shape);
        for (var i = 0; i < value.Shape; i++)
            result[i] = -value[i];
        return result;
    }
}