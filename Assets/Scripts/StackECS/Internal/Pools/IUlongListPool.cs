using System.Collections.Generic;

namespace StackECS.Pools
{
    internal interface IUlongListPool
    {
        List<ulong> Rent();
        void Return(List<ulong> ulongList);
    }
}