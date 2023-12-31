using System.Runtime.CompilerServices;

namespace RL.Core;

public readonly struct SelectList<TList, T, TTo>(
    TList list,
    Func<T, TTo> selector
) : IStructList<SelectList<TList, T, TTo>, TTo>
    where TList : IReadOnlyList<T>
{
    public int Count => list.Count;
    public TTo this[int index] => selector(list[index]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StructEnumerator<SelectList<TList, T, TTo>, TTo> GetEnumerator() => new(this);
}

public readonly struct SelectList<TList, T, TTo, TContext>(
    TList list,
    TContext context,
    Func<TContext, T, TTo> selector
) : IStructList<SelectList<TList, T, TTo, TContext>, TTo>
    where TList : IReadOnlyList<T>
{
    public int Count => list.Count;
    public TTo this[int index] => selector(context, list[index]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StructEnumerator<SelectList<TList, T, TTo, TContext>, TTo> GetEnumerator() => new(this);
}
