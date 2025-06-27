using System;

namespace StackECS
{
    internal class ComponentPool<T> : IComponentPool where T : unmanaged
    {
        private T[] _data;

        public ComponentPool(int capacity)
        {
            _data = new T[capacity];
        }

        public ref T Get(uint entity)
        {
            return ref _data[entity];
        }

        public ref T Add(uint entity)
        {
            _data[entity] = default;
            return ref _data[entity];
        }
    }
}