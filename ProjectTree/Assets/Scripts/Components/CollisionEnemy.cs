using Unity.Entities;

public struct CollisionEnemy : IBufferElementData
{
    public AIData AiData;
    public Entity Entity;
}