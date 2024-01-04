using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Generators;
using RL.Random;
using TorchSharp.Utils;
using static TorchSharp.torch;

namespace RL.Algorithms;

public static class TensorExtensions
{
    public static TensorAdapter<T> AsGenerator<T>(this TensorAccessor<T> accessor)
        where T : unmanaged =>
        new(accessor);

    public static TensorAdapter<T> AsGenerator<T>(this Tensor tensor)
        where T : unmanaged =>
        new(tensor.data<T>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZipGenerator<TensorAdapter<T1>, TensorAdapter<T2>, T1, T2>
        Zip<T1, T2>(this TensorAdapter<T1> generator1, TensorAdapter<T2> generator2)
        where T1 : unmanaged
        where T2 : unmanaged =>
        generator1.Zip<TensorAdapter<T1>, TensorAdapter<T2>, T1, T2>(generator2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ChoiceIndex<T>(this TensorAdapter<T> probabilities, IRandomGenerator random)
        where T : unmanaged, INumber<T> =>
        probabilities.ChoiceIndex<TensorAdapter<T>, T>(random);
}