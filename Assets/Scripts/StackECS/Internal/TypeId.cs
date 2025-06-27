namespace StackECS
{
    internal static class TypeId<T>
    {
        public static readonly int Id;

        static TypeId()
        {
            Id = TypeIdGenerator.GetNextId();
        }
    }

    internal static class TypeIdGenerator
    {
        private static int _counter;

        public static int GetNextId()
        {
            return _counter++;
        }
    }
}