using System.Numerics;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    public static int Count<T>(this SequenceGenerator<T> generator)
        where T : INumberBase<T> =>
        Count<SequenceGenerator<T>, T>(generator);

    public static int Count<T>(this IGenerator<T> generator) =>
        Count<IGenerator<T>, T>(generator);

    public static int Count<TG, T>(this TG generator)
        where TG : IGenerator<T> =>
        generator.Count;
}