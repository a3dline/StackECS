namespace StackECS
{
    internal class ComponentPool<T> : IComponentPool where T : unmanaged
    {
        private T[] _data;

        public ComponentPool(int initialCapacity)
        {
            _data = new T[initialCapacity];
        }

        public ref T Get(uint entity)
        {
            return ref _data[entity];
        }

        public ref T Add(uint entity)
        {
            ArrayUtilities.ResizeArray(ref _data, (int)entity + 1);
            _data[entity] = default;
            return ref _data[entity];
        }
    }
}