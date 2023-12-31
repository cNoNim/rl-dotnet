using System.Numerics;

namespace RL.Core;

public static partial class List
{
    public static TTo[] ToArray<T, TTo, TContext>(this SelectList<RangeList<T>, T, TTo, TContext> list)
        where T : INumberBase<T> =>
        ToArray<SelectList<RangeList<T>, T, TTo, TContext>, TTo>(list);

    public static TTo[] ToArray<T, TTo, TContext>(this SelectList<IReadOnlyList<T>, T, TTo, TContext> list)
        where T : INumberBase<T> =>
        ToArray<SelectList<IReadOnlyList<T>, T, TTo, TContext>, TTo>(list);

    public static TTo[] ToArray<T, TTo>(this SelectList<IReadOnlyList<T>, T, TTo> list) =>
        ToArray<SelectList<IReadOnlyList<T>, T, TTo>, TTo>(list);

    public static T[] ToArray<T>(this IReadOnlyList<T> list) =>
        ToArray<IReadOnlyList<T>, T>(list);

    public static T[] ToArray<TList, T>(this TList list)
        where TList : IReadOnlyList<T>
    {
        var array = new T[list.Count];
        foreach (var (value, index) in list.Index<TList, T>())
            array[index] = value;
        return array;
    }
}
