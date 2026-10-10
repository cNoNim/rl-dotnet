using System;
using System.Numerics;

namespace RL.Algorithms;

public static class Functions
{
    public static Func<T, T> Linear<T>(T start, T end, T endFraction)
        where T : INumber<T> =>
        remaining =>
            T.One - remaining > endFraction ? end : start + (T.One - remaining) * (end - start) / endFraction;
}
