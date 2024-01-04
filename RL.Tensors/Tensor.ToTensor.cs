using System;
using System.Numerics;
using RL.Core;
using RL.Generators;

namespace RL.Tensors;

public static partial class Tensor
{
    public static Tensor1D<TTo> ToTensor<TG, T, TTo, TContext>(this SelectGenerator<TG, T, TTo, TContext> generator)
        where TG : IGenerator<T>
        where TTo :
        IAdditionOperators<TTo, TTo, TTo>,
        IDivisionOperators<TTo, TTo, TTo>,
        IComparisonOperators<TTo, TTo, bool>,
        IModulusOperators<TTo, TTo, TTo>,
        IMultiplyOperators<TTo, TTo, TTo>,
        ISubtractionOperators<TTo, TTo, TTo>,
        IUnaryPlusOperators<TTo, TTo>,
        IUnaryNegationOperators<TTo, TTo>,
        IEquatable<TTo> =>
        ToTensor<SelectGenerator<TG, T, TTo, TContext>, TTo>(generator);

    public static Tensor1D<T> ToTensor<TG, T>(this TakeGenerator<TG, T> generator)
        where TG : IGenerator<T>
        where T :
        IAdditionOperators<T, T, T>,
        IDivisionOperators<T, T, T>,
        IComparisonOperators<T, T, bool>,
        IModulusOperators<T, T, T>,
        IMultiplyOperators<T, T, T>,
        ISubtractionOperators<T, T, T>,
        IUnaryPlusOperators<T, T>,
        IUnaryNegationOperators<T, T>,
        IEquatable<T> =>
        ToTensor<TakeGenerator<TG, T>, T>(generator);

    public static Tensor1D<T> ToTensor<T>(this IGenerator<T> generator)
        where T :
        IAdditionOperators<T, T, T>,
        IDivisionOperators<T, T, T>,
        IComparisonOperators<T, T, bool>,
        IModulusOperators<T, T, T>,
        IMultiplyOperators<T, T, T>,
        ISubtractionOperators<T, T, T>,
        IUnaryPlusOperators<T, T>,
        IUnaryNegationOperators<T, T>,
        IEquatable<T> =>
        ToTensor<IGenerator<T>, T>(generator);

    public static Tensor1D<T> ToTensor<TG, T>(this TG generator)
        where TG : IGenerator<T>
        where T :
        IAdditionOperators<T, T, T>,
        IDivisionOperators<T, T, T>,
        IComparisonOperators<T, T, bool>,
        IModulusOperators<T, T, T>,
        IMultiplyOperators<T, T, T>,
        ISubtractionOperators<T, T, T>,
        IUnaryPlusOperators<T, T>,
        IUnaryNegationOperators<T, T>,
        IEquatable<T>
    {
        if (!generator.IsFinite)
            throw new OverflowException();
        var array = generator.Count.Zeroes<T>();
        foreach (var (value, index) in generator.Index<TG, T>())
            array[index] = value;
        return array;
    }
}