using System.Collections;
using System.Numerics;

namespace RL.Core;

public static class EpsilonGreedy
{
    public static EpsilonGreedyProbabilityList EpsilonGreedyProbabilities<T>(this MatrixRowList<T> list, double epsilon)
        where T : IMinMaxValue<T>, IComparable<T>, INumberBase<T> =>
        EpsilonGreedyProbabilities<MatrixRowList<T>, T>(list, epsilon);

    public static EpsilonGreedyProbabilityList EpsilonGreedyProbabilities<T>(this RangeList<T> list, double epsilon)
        where T : IMinMaxValue<T>, IComparable<T>, INumberBase<T> =>
        EpsilonGreedyProbabilities<RangeList<T>, T>(list, epsilon);

    public static EpsilonGreedyProbabilityList EpsilonGreedyProbabilities<T>(this IReadOnlyList<T> list, double epsilon)
        where T : IMinMaxValue<T>, IComparable<T> =>
        EpsilonGreedyProbabilities<IReadOnlyList<T>, T>(list, epsilon);

    public static EpsilonGreedyProbabilityList EpsilonGreedyProbabilities<TList, T>(this TList list, double epsilon)
        where TList : IReadOnlyList<T>
        where T : IMinMaxValue<T>, IComparable<T> =>
        new(list.Count, list.MaxIndex<TList, T>(), epsilon, epsilon / double.CreateChecked(list.Count));

    public readonly struct EpsilonGreedyProbabilityList(int count, int max, double epsilon, double em) :
        IReadOnlyList<double>
    {
        public double this[int index] => index == max ? 1.0 - epsilon + em : em;

        public int Count => count;

        public ReadOnlyListStructEnumerator<EpsilonGreedyProbabilityList, double> GetEnumerator() => new(this);
        IEnumerator<double> IEnumerable<double>.GetEnumerator() => GetEnumerator().AsEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator().AsEnumerator();
    }
}
