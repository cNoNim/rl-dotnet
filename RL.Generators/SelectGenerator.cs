using RL.Core;

namespace RL.Generators;

public readonly struct SelectGenerator<TG, T, TTo>(
    TG generator,
    Func<T, TTo> selector
) : IGenerator<SelectGenerator<TG, T, TTo>, TTo>
    where TG : IGenerator<T>
{
    public TTo this[int index] => selector(generator[index]);
    public int Count => generator.Count;

    public GeneratorEnumerator<SelectGenerator<TG, T, TTo>, TTo> GetEnumerator() =>
        new(this);

    bool IGenerator<TTo>.IsFinite => generator.IsFinite;
    bool IGenerator<TTo>.TryGetNext(int current, out int next) => generator.TryGetNext(current, out next);
}

public readonly struct SelectGenerator<TG, T, TTo, TContext>(
    TG generator,
    TContext context,
    Func<TContext, T, TTo> selector
) : IGenerator<SelectGenerator<TG, T, TTo, TContext>, TTo>
    where TG : IGenerator<T>
{
    public TTo this[int index] => selector(context, generator[index]);
    public int Count => generator.Count;

    public GeneratorEnumerator<SelectGenerator<TG, T, TTo, TContext>, TTo> GetEnumerator() =>
        new(this);

    bool IGenerator<TTo>.IsFinite => generator.IsFinite;
    bool IGenerator<TTo>.TryGetNext(int current, out int next) => generator.TryGetNext(current, out next);
}
