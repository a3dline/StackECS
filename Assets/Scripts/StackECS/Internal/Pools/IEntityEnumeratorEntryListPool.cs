using System.Collections.Generic;

namespace StackECS.Pools
{
    internal interface IEntityEnumeratorEntryListPool
    {
        List<EntityEnumeratorEntry> Rent();
        void Return(List<EntityEnumeratorEntry> entityEnumeratorEntries);
    }
}