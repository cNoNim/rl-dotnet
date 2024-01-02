using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GeneratorEnumerable<TG, T> AsGeneratorEnumerable<TG, T>(
        this TG generator
    ) where TG : IGenerator<T> => new(generator);
}