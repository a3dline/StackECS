using System;

namespace StackECS
{
    internal struct BitArray64 : IEquatable<BitArray64>
    {
        public BitArray64(ulong hash)
        {
            Hash = hash;
        }

        public bool this[int index]
        {
            get
            {
                if ((uint)index >= 64)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return (Hash & (1UL << index)) != 0;
            }
            set
            {
                if ((uint)index >= 64)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (value)
                    Hash |= 1UL << index; // Set bit
                else
                    Hash &= ~(1UL << index); // Clear bit
            }
        }

        public readonly ulong WithBit(int index)
        {
            if ((uint)index >= 64)
                throw new ArgumentOutOfRangeException(nameof(index));

            return Hash | (1UL << index);
        }

        public readonly ulong WithoutBit(int index)
        {
            if ((uint)index >= 64)
                throw new ArgumentOutOfRangeException(nameof(index));

            return Hash & ~(1UL << index);
        }

        public static BitArray64 operator -(BitArray64 a, BitArray64 b)
        {
            return new BitArray64(a.Hash & ~b.Hash);
        }

        public static BitArray64 operator +(BitArray64 a, BitArray64 b)
        {
            return new BitArray64(a.Hash | b.Hash);
        }

        public static bool operator ==(BitArray64 a, BitArray64 b)
        {
            return a.Hash == b.Hash;
        }

        public static bool operator !=(BitArray64 a, BitArray64 b)
        {
            return a.Hash != b.Hash;
        }

        public ulong Hash { get; private set; }

        public bool Equals(BitArray64 other)
        {
            return Hash == other.Hash;
        }

        public override bool Equals(object obj)
        {
            return obj is BitArray64 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Hash.GetHashCode();
        }
    }
}