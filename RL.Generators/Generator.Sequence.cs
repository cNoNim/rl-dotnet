using System.Numerics;
using System.Runtime.CompilerServices;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SequenceGenerator<T> Sequence<T>()
        where T : INumberBase<T> =>
        new();
}