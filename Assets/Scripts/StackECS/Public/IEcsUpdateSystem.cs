namespace StackECS
{
    public interface IEcsUpdateSystem : IEcsSystem
    {
        void Update(IEcs ecs);
    }
}