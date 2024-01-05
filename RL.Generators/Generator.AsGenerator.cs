using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyAdapter<T> AsGenerator<T>(this IReadOnlyList<T> list) =>
        new(list);
}