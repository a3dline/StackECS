using System;
using System.Collections.Generic;

namespace StackECS.Pools
{
    internal class HeapPool : IUlongListPool,
                              IEntityEnumeratorEntryListPool
    {
        private readonly Stack<List<EntityEnumeratorEntry>> _entityEnumeratorEntryPool = new();
        private readonly int _initialTypeCapacity;
        private readonly Stack<List<ulong>> _ulongPool = new();

        public HeapPool(int initialTypeCapacity)
        {
            _initialTypeCapacity = initialTypeCapacity;
        }

        List<EntityEnumeratorEntry> IEntityEnumeratorEntryListPool.Rent()
        {
            return _entityEnumeratorEntryPool.Count > 0
                       ? _entityEnumeratorEntryPool.Pop()
                       : new List<EntityEnumeratorEntry>(64);
        }

        void IEntityEnumeratorEntryListPool.Return(List<EntityEnumeratorEntry> entityEnumeratorEntries)
        {
            entityEnumeratorEntries.Clear();
            _entityEnumeratorEntryPool.Push(entityEnumeratorEntries);
        }

        List<ulong> IUlongListPool.Rent()
        {
            return _ulongPool.Count > 0 ? _ulongPool.Pop() : new List<ulong>(Math.Max(_initialTypeCapacity / 64, 1));
        }

        void IUlongListPool.Return(List<ulong> ulongList)
        {
            ulongList.Clear();
            _ulongPool.Push(ulongList);
        }
    }
}