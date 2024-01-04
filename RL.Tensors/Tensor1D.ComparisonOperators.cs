using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Core;
using RL.Generators;

namespace RL.Tensors;

public readonly partial struct Tensor1D<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Tensor1D<T> other) =>
        Shape == other.Shape && CombineToBoolean(this, other, static (t, o) => t.Equals(o));

    public override bool Equals(object? obj) =>
        obj is Tensor1D<T> other && Equals(other);

    public override int GetHashCode() => (int)StableHashCode.HashGenerator(
        (uint)Shape,
        _array.AsGenerator().Select(static v => (uint)v.GetHashCode())
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator ==(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l == r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator ==(Tensor1D<T> left, T right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l == r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator ==(T left, Tensor1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l == r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator !=(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l != r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator !=(Tensor1D<T> left, T right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l != r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator !=(T left, Tensor1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l != r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator >(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l > r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator >(Tensor1D<T> left, T right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l > r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator >(T left, Tensor1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l > r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator >=(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l >= r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator >=(Tensor1D<T> left, T right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l >= r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator >=(T left, Tensor1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l >= r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator <(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l < r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator <(Tensor1D<T> left, T right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l < r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator <(T left, Tensor1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l < r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator <=(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l <= r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator <=(Tensor1D<T> left, T right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l <= r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanTensor1D operator <=(T left, Tensor1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l <= r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IEqualityOperators<Tensor1D<T>, Tensor1D<T>, bool>.operator ==(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToBoolean(left, right, static (l, r) => l == r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IEqualityOperators<Tensor1D<T>, Tensor1D<T>, bool>.operator !=(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToBoolean(left, right, static (l, r) => l != r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IComparisonOperators<Tensor1D<T>, Tensor1D<T>, bool>.operator >(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToBoolean(left, right, static (l, r) => l > r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IComparisonOperators<Tensor1D<T>, Tensor1D<T>, bool>.operator >=(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToBoolean(left, right, static (l, r) => l >= r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IComparisonOperators<Tensor1D<T>, Tensor1D<T>, bool>.operator <(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToBoolean(left, right, static (l, r) => l < r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IComparisonOperators<Tensor1D<T>, Tensor1D<T>, bool>.operator <=(Tensor1D<T> left, Tensor1D<T> right) =>
        CombineToBoolean(left, right, static (l, r) => l <= r);
}