using System.Runtime.CompilerServices;

namespace RL.Random;

internal static class XxHash32Algorithm
{
    private const uint P1 = 2654435761U;
    private const uint P2 = 2246822519U;
    private const uint P3 = 3266489917U;
    private const uint P4 = 668265263U;
    private const uint P5 = 374761393U;

    public static uint Hash(uint seed, byte[] buf, int len = -1)
    {
        uint h32;
        var index = 0;
        if (len == -1)
        {
            len = buf.Length;
        }

        if (len >= 16)
        {
            var limit = len - 16;
            var v1 = seed + P1 + P2;
            var v2 = seed + P2;
            var v3 = seed + 0;
            var v4 = seed - P1;

            do
            {
                v1 = SubHash(v1, buf, index);
                index += 4;
                v2 = SubHash(v2, buf, index);
                index += 4;
                v3 = SubHash(v3, buf, index);
                index += 4;
                v4 = SubHash(v4, buf, index);
                index += 4;
            } while (index <= limit);

            h32 = RotateLeft(v1, 1) + RotateLeft(v2, 7) + RotateLeft(v3, 12) + RotateLeft(v4, 18);
        }
        else
        {
            h32 = seed + P5;
        }

        h32 += (uint) len;

        while (index <= len - 4)
        {
            h32 += BitConverter.ToUInt32(buf, index) * P3;
            h32 = RotateLeft(h32, 17) * P4;
            index += 4;
        }

        while (index < len)
        {
            h32 += buf[index] * P5;
            h32 = RotateLeft(h32, 11) * P1;
            index++;
        }

        h32 ^= h32 >> 15;
        h32 *= P2;
        h32 ^= h32 >> 13;
        h32 *= P3;
        h32 ^= h32 >> 16;

        return h32;
    }

    public static uint Hash(uint seed, params uint[] values) =>
        HashList(seed, values);

    public static uint HashList(uint seed, ReadOnlySpan<uint> buf)
    {
        uint h32;
        var index = 0;
        var len = buf.Length;

        if (len < 4)
            h32 = seed + P5;
        else
        {
            var limit = len - 4;
            var v1 = seed + P1 + P2;
            var v2 = seed + P2;
            var v3 = seed + 0;
            var v4 = seed - P1;

            do
            {
                v1 = SubHash(v1, buf[index]);
                index++;
                v2 = SubHash(v2, buf[index]);
                index++;
                v3 = SubHash(v3, buf[index]);
                index++;
                v4 = SubHash(v4, buf[index]);
                index++;
            } while (index <= limit);

            h32 = RotateLeft(v1, 1) + RotateLeft(v2, 7) + RotateLeft(v3, 12) + RotateLeft(v4, 18);
        }

        h32 += (uint) len * 4;

        while (index < len)
        {
            h32 += buf[index] * P3;
            h32 = RotateLeft(h32, 17) * P4;
            index++;
        }

        h32 ^= h32 >> 15;
        h32 *= P2;
        h32 ^= h32 >> 13;
        h32 *= P3;
        h32 ^= h32 >> 16;

        return h32;
    }

    public static uint Hash(uint seed, params int[] values) =>
        HashList(seed, values);

    public static uint HashList(uint seed, ReadOnlySpan<int> values)
    {
        uint h32;
        var index = 0;
        var len = values.Length;

        if (len < 4)
            h32 = seed + P5;
        else
        {
            var limit = len - 4;
            var v1 = seed + P1 + P2;
            var v2 = seed + P2;
            var v3 = seed + 0;
            var v4 = seed - P1;

            do
            {
                v1 = SubHash(v1, (uint) values[index]);
                index++;
                v2 = SubHash(v2, (uint) values[index]);
                index++;
                v3 = SubHash(v3, (uint) values[index]);
                index++;
                v4 = SubHash(v4, (uint) values[index]);
                index++;
            } while (index <= limit);

            h32 = RotateLeft(v1, 1) + RotateLeft(v2, 7) + RotateLeft(v3, 12) + RotateLeft(v4, 18);
        }

        h32 += (uint) len * 4;

        while (index < len)
        {
            h32 += (uint) values[index] * P3;
            h32 = RotateLeft(h32, 17) * P4;
            index++;
        }

        h32 ^= h32 >> 15;
        h32 *= P2;
        h32 ^= h32 >> 13;
        h32 *= P3;
        h32 ^= h32 >> 16;

        return h32;
    }

    public static uint Hash(uint seed, int value)
    {
        var h32 = seed + P5 + 4U;
        h32 += (uint) value * P3;
        h32 = RotateLeft(h32, 17) * P4;
        h32 ^= h32 >> 15;
        h32 *= P2;
        h32 ^= h32 >> 13;
        h32 *= P3;
        h32 ^= h32 >> 16;
        return h32;
    }

    public static uint Hash(uint seed, uint value)
    {
        var h32 = seed + P5 + 4U;
        h32 += value * P3;
        h32 = RotateLeft(h32, 17) * P4;
        h32 ^= h32 >> 15;
        h32 *= P2;
        h32 ^= h32 >> 13;
        h32 *= P3;
        h32 ^= h32 >> 16;
        return h32;
    }

    private static uint SubHash(uint value, byte[] buf, int index)
    {
        var readValue = BitConverter.ToUInt32(buf, index);
        value += readValue * P2;
        value = RotateLeft(value, 13);
        value *= P1;
        return value;
    }

    private static uint SubHash(uint value, uint readValue)
    {
        value += readValue * P2;
        value = RotateLeft(value, 13);
        value *= P1;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint RotateLeft(uint value, int count) =>
        (value << count) | (value >> (32 - count));
}
