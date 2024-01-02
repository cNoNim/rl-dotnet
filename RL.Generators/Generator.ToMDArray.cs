using RL.Core;
using RL.MDArrays;

namespace RL.Generators;

public static partial class Generator
{
    public static Array1D<TTo> ToMDArray<TG, T, TTo, TContext>(
        this SelectGenerator<TG, T, TTo, TContext> generator
    ) where TG : IGenerator<T> =>
        ToMDArray<SelectGenerator<TG, T, TTo, TContext>, TTo>(generator);

    public static Array1D<T> ToMDArray<TG, T>(this TakeGenerator<TG, T> generator)
        where TG : IGenerator<T> =>
        ToMDArray<TakeGenerator<TG, T>, T>(generator);

    public static Array1D<T> ToMDArray<T>(this IGenerator<T> generator) =>
        ToMDArray<IGenerator<T>, T>(generator);

    public static Array1D<T> ToMDArray<TG, T>(this TG generator)
        where TG : IGenerator<T>
    {
        if (!generator.IsFinite)
            throw new OverflowException();
        var array = generator.Count.Zeroes<T>();
        foreach (var (value, index) in generator.Index<TG, T>())
            array[index] = value;
        return array;
    }
}
