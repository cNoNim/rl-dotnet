using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Generators;
using RL.Random;
using TorchSharp;
using TorchSharp.Modules;
using TorchSharp.Utils;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace RL.Algorithms;

public static class TorchExtensions
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


    public static void SoftUpdateBy(
        this Module target,
        Module source,
        float tau
    ) => SoftUpdateBy(target.parameters(), source.parameters(), tau);

    public static void SoftUpdateBy(
        IEnumerable<Parameter> target,
        IEnumerable<Parameter> source,
        float tau
    )
    {
        using (no_grad())
            foreach (var (s, t) in source.Zip(target))
            {
                t.mul_(1 - tau);
                t.copy_(t.add(s, tau));
            }
    }
}
