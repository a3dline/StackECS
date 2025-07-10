using System;
using System.Runtime.CompilerServices;

namespace StackECS
{
    internal static class ArrayUtilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResizeArray<T>(ref T[] array, int requestedSize)
        {
            if (array.Length >= requestedSize) return;

            var newSize = array.Length * 2;
            while (requestedSize > newSize) newSize *= 2;

            Array.Resize(ref array, newSize);
        }
    }
}