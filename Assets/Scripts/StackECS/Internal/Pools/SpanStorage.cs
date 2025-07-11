using System;
using System.Collections.Generic;

namespace StackECS.Pools
{
    public class SpanStorage<T> where T : unmanaged
    {
        private readonly List<T[]> _blocks = new();
        private readonly int _blockSize;
        private readonly int _chunkSize;
        private readonly Stack<Slot> _freeSlots = new();

        public SpanStorage(int chunkSize)
        {
            _chunkSize = chunkSize;
            _blockSize = chunkSize * 64;
            ExpandBuffer();
        }

        public Slot ReserveSlot()
        {
            if (_freeSlots.Count == 0)
                ExpandBuffer();

            return _freeSlots.Pop();
        }

        public Span<T> GetSpan(in Slot slot)
        {
            return _blocks[slot.BlockIndex].AsSpan(slot.Offset, _chunkSize);
        }

        public void Release(Slot slot)
        {
            _freeSlots.Push(slot);
            for (var i = 0; i < _chunkSize; i++)
            {
                var data = _blocks[slot.BlockIndex];
                data[slot.Offset + i] = default;
            }
        }

        private void ExpandBuffer()
        {
            var blockIndex = _blocks.Count;
            var buffer = new T[_blockSize];
            _blocks.Add(buffer);

            var length = _blockSize - _chunkSize;
            for (var offset = 0; offset <= length; offset += _chunkSize)
                _freeSlots.Push(new Slot(blockIndex, offset));
        }
    }

    public readonly struct Slot
    {
        public readonly int BlockIndex;
        public readonly int Offset;

        public Slot(int blockIndex, int offset)
        {
            BlockIndex = blockIndex;
            Offset = offset;
        }
    }
}