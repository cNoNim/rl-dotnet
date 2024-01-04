using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Core;
using RL.Generators;

namespace RL.MDArrays;

public readonly partial struct Array1D<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Array1D<T> other) =>
        Shape == other.Shape && CombineToBoolean(this, other, static (t, o) => t.Equals(o));

    public override bool Equals(object? obj) =>
        obj is Array1D<T> other && Equals(other);

    public override int GetHashCode() => (int)StableHashCode.HashGenerator(
        (uint)Shape,
        _array.AsGenerator().Select(static v => (uint)v.GetHashCode())
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator ==(Array1D<T> left, Array1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l == r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator ==(Array1D<T> left, T right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l == r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator ==(T left, Array1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l == r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator !=(Array1D<T> left, Array1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l != r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator !=(Array1D<T> left, T right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l != r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator !=(T left, Array1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l != r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator >(Array1D<T> left, Array1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l > r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator >(Array1D<T> left, T right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l > r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator >(T left, Array1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l > r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator >=(Array1D<T> left, Array1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l >= r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator >=(Array1D<T> left, T right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l >= r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator >=(T left, Array1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l >= r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator <(Array1D<T> left, Array1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l < r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator <(Array1D<T> left, T right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l < r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator <(T left, Array1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l < r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator <=(Array1D<T> left, Array1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l <= r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator <=(Array1D<T> left, T right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l <= r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BooleanArray1D operator <=(T left, Array1D<T> right) =>
        CombineToBooleanTensor(left, right, static (l, r) => l <= r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IEqualityOperators<Array1D<T>, Array1D<T>, bool>.operator ==(Array1D<T> left, Array1D<T> right) =>
        CombineToBoolean(left, right, static (l, r) => l == r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IEqualityOperators<Array1D<T>, Array1D<T>, bool>.operator !=(Array1D<T> left, Array1D<T> right) =>
        CombineToBoolean(left, right, static (l, r) => l != r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IComparisonOperators<Array1D<T>, Array1D<T>, bool>.operator >(Array1D<T> left, Array1D<T> right) =>
        CombineToBoolean(left, right, static (l, r) => l > r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IComparisonOperators<Array1D<T>, Array1D<T>, bool>.operator >=(Array1D<T> left, Array1D<T> right) =>
        CombineToBoolean(left, right, static (l, r) => l >= r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IComparisonOperators<Array1D<T>, Array1D<T>, bool>.operator <(Array1D<T> left, Array1D<T> right) =>
        CombineToBoolean(left, right, static (l, r) => l < r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IComparisonOperators<Array1D<T>, Array1D<T>, bool>.operator <=(Array1D<T> left, Array1D<T> right) =>
        CombineToBoolean(left, right, static (l, r) => l <= r);
}