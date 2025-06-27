namespace StackECS
{
    public interface IEcsInitSystem : IEcsSystem
    {
        void Init(IEcs ecs);
    }
}