using Unity.Entities;
using Unity.Physics.Systems;

[DisableAutoCreation]
[UpdateAfter(typeof(ExportPhysicsWorld))]
public class ClearEntities : SystemBase
{
    EntityCommandBufferSystem m_entityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        //var entities = World.DefaultGameObjectInjectionWorld.EntityManager.GetAllEntities(Allocator.TempJob);
        //World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(entities);
        //entities.Dispose();

        EntityCommandBuffer commandBuffer = m_entityCommandBufferSystem.CreateCommandBuffer();

        Entities
            .ForEach((Entity entity) => { commandBuffer.DestroyEntity(entity); }).Schedule();

        m_entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}