namespace StackECS
{
    public interface IEcs
    {
        EcsQuery Query { get; }
        Entity CreateEntity();
    }
}