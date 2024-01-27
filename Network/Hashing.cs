using System.Runtime.CompilerServices;
using System.Text;

namespace LethalMDK.Network;

// shamelessly copied from unity netcode which for some reason is internal?
public static class Hashing
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe uint Hash32(byte* input, int length, uint seed = 0)
    {
      uint num1 = seed + 374761393U;
      if (length >= 16)
      {
        uint num2 = (uint) ((int) seed - 1640531535 - 2048144777);
        uint num3 = seed + 2246822519U;
        uint num4 = seed;
        uint num5 = seed - 2654435761U;
        int num6 = length >> 4;
        for (int index = 0; index < num6; ++index)
        {
          uint num7 = *(uint*) input;
          uint num8 = *(uint*) (input + 4);
          uint num9 = *(uint*) (input + 8);
          uint num10 = *(uint*) (input + 12);
          uint num11 = num2 + num7 * 2246822519U;
          num2 = (num11 << 13 | num11 >> 19) * 2654435761U;
          uint num12 = num3 + num8 * 2246822519U;
          num3 = (num12 << 13 | num12 >> 19) * 2654435761U;
          uint num13 = num4 + num9 * 2246822519U;
          num4 = (num13 << 13 | num13 >> 19) * 2654435761U;
          uint num14 = num5 + num10 * 2246822519U;
          num5 = (num14 << 13 | num14 >> 19) * 2654435761U;
          input += 16;
        }
        num1 = (uint) (((int) num2 << 1 | (int) (num2 >> 31)) + ((int) num3 << 7 | (int) (num3 >> 25)) + ((int) num4 << 12 | (int) (num4 >> 20)) + ((int) num5 << 18 | (int) (num5 >> 14)));
      }
      uint num15 = num1 + (uint) length;
      for (length &= 15; length >= 4; length -= 4)
      {
        uint num16 = num15 + *(uint*) input * 3266489917U;
        num15 = (uint) (((int) num16 << 17 | (int) (num16 >> 15)) * 668265263);
        input += 4;
      }
      for (; length > 0; --length)
      {
        uint num17 = num15 + (uint) *input * 374761393U;
        num15 = (uint) (((int) num17 << 11 | (int) (num17 >> 21)) * -1640531535);
        ++input;
      }
      uint num18 = (num15 ^ num15 >> 15) * 2246822519U;
      uint num19 = (num18 ^ num18 >> 13) * 3266489917U;
      return num19 ^ num19 >> 16;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ulong Hash64(byte* input, int length, uint seed = 0)
    {
      ulong num1 = (ulong) seed + 2870177450012600261UL;
      if (length >= 32)
      {
        ulong num2 = (ulong) ((long) seed + -7046029288634856825L + -4417276706812531889L);
        ulong num3 = (ulong) seed + 14029467366897019727UL;
        ulong num4 = (ulong) seed;
        ulong num5 = (ulong) seed - 11400714785074694791UL;
        int num6 = length >> 5;
        for (int index = 0; index < num6; ++index)
        {
          ulong num7 = (ulong) *(long*) input;
          ulong num8 = (ulong) *(long*) (input + 8);
          ulong num9 = (ulong) *(long*) (input + 16);
          ulong num10 = (ulong) *(long*) (input + 24);
          ulong num11 = num2 + num7 * 14029467366897019727UL;
          num2 = (num11 << 31 | num11 >> 33) * 11400714785074694791UL;
          ulong num12 = num3 + num8 * 14029467366897019727UL;
          num3 = (num12 << 31 | num12 >> 33) * 11400714785074694791UL;
          ulong num13 = num4 + num9 * 14029467366897019727UL;
          num4 = (num13 << 31 | num13 >> 33) * 11400714785074694791UL;
          ulong num14 = num5 + num10 * 14029467366897019727UL;
          num5 = (num14 << 31 | num14 >> 33) * 11400714785074694791UL;
          input += 32;
        }
        ulong num15 = (ulong) (((long) num2 << 1 | (long) (num2 >> 63)) + ((long) num3 << 7 | (long) (num3 >> 57)) + ((long) num4 << 12 | (long) (num4 >> 52)) + ((long) num5 << 18 | (long) (num5 >> 46)));
        ulong num16 = num2 * 14029467366897019727UL;
        ulong num17 = (num16 << 31 | num16 >> 33) * 11400714785074694791UL;
        ulong num18 = (ulong) ((long) (num15 ^ num17) * -7046029288634856825L + -8796714831421723037L);
        ulong num19 = num3 * 14029467366897019727UL;
        ulong num20 = (num19 << 31 | num19 >> 33) * 11400714785074694791UL;
        ulong num21 = (ulong) ((long) (num18 ^ num20) * -7046029288634856825L + -8796714831421723037L);
        ulong num22 = num4 * 14029467366897019727UL;
        ulong num23 = (num22 << 31 | num22 >> 33) * 11400714785074694791UL;
        ulong num24 = (ulong) ((long) (num21 ^ num23) * -7046029288634856825L + -8796714831421723037L);
        ulong num25 = num5 * 14029467366897019727UL;
        ulong num26 = (num25 << 31 | num25 >> 33) * 11400714785074694791UL;
        num1 = (ulong) ((long) (num24 ^ num26) * -7046029288634856825L + -8796714831421723037L);
      }
      ulong num27 = num1 + (ulong) length;
      for (length &= 31; length >= 8; length -= 8)
      {
        ulong num28 = (ulong) (*(long*) input * -4417276706812531889L);
        ulong num29 = (ulong) (((long) num28 << 31 | (long) (num28 >> 33)) * -7046029288634856825L);
        ulong num30 = num27 ^ num29;
        num27 = (ulong) (((long) num30 << 27 | (long) (num30 >> 37)) * -7046029288634856825L + -8796714831421723037L);
        input += 8;
      }
      if (length >= 4)
      {
        ulong num31 = num27 ^ (ulong) *(uint*) input * 11400714785074694791UL;
        num27 = (ulong) (((long) num31 << 23 | (long) (num31 >> 41)) * -4417276706812531889L + 1609587929392839161L);
        input += 4;
        length -= 4;
      }
      for (; length > 0; --length)
      {
        ulong num32 = num27 ^ (ulong) *input * 2870177450012600261UL;
        num27 = (ulong) (((long) num32 << 11 | (long) (num32 >> 53)) * -7046029288634856825L);
        ++input;
      }
      ulong num33 = (num27 ^ num27 >> 33) * 14029467366897019727UL;
      ulong num34 = (num33 ^ num33 >> 29) * 1609587929392839161UL;
      return num34 ^ num34 >> 32;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe uint Hash32(this byte[] buffer)
    {
      int length = buffer.Length;
      fixed (byte* input = buffer)
        return Hash32(input, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Hash32(this string text) => Encoding.UTF8.GetBytes(text).Hash32();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Hash32(this Type type) => type.FullName.Hash32();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Hash32<T>() => typeof (T).Hash32();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ulong Hash64(this byte[] buffer)
    {
      int length = buffer.Length;
      fixed (byte* input = buffer)
        return Hash64(input, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Hash64(this string text) => Encoding.UTF8.GetBytes(text).Hash64();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Hash64(this Type type) => type.FullName.Hash64();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Hash64<T>() => typeof (T).Hash64();
}