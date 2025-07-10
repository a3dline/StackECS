namespace StackECS.Pools
{
    internal interface IIntArrayPool
    {
        int[] Rent(int minimumLength);
        void Return(int[] array);
    }
}