using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChunkGenerator<IGenerator<T>, T> Chunks<T>(this IGenerator<T> generator, int chunkSize) =>
        Chunks<IGenerator<T>, T>(generator, chunkSize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChunkGenerator<TG, T> Chunks<TG, T>(this TG generator, int chunkSize)
        where TG : IGenerator<T> =>
        new(generator, chunkSize);
}