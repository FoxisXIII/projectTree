using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using static Unity.Mathematics.math;

[UpdateBefore(typeof(TimeToLiveSystem))]
public class HealthSystem : JobComponentSystem
{
    private BuildPhysicsWorld _buildPhysicsWorldSystem;

    private StepPhysicsWorld _stepPhysicsWorldSystem;
    

    protected override void OnCreate()
    {
        base.OnCreate();
        _buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    [BurstCompile]
    struct CollisionSampleJob : ICollisionEventsJob
    {
        public ComponentDataFromEntity<HealthData> enemyGroup;
        public ComponentDataFromEntity<DealsDamage> bulletGroup;

        public Entity GetEntityFromComponentGroup<T>(Entity entityA, Entity entityB,
            ComponentDataFromEntity<T> componentGroup) where T : struct, IComponentData
        {
            if (componentGroup.Exists(entityA))
                return entityA;
            if (componentGroup.Exists(entityB))
                return entityB;
            return Entity.Null;
        }


        public void Execute(CollisionEvent collisionEvent)
        {
            var entityA = collisionEvent.Entities.EntityA;
            var entityB = collisionEvent.Entities.EntityB;
            var enemyEntity = GetEntityFromComponentGroup(entityA, entityB, enemyGroup);
            var bulletEntity = GetEntityFromComponentGroup(entityA, entityB, bulletGroup);
            if (enemyEntity != Entity.Null && bulletEntity != Entity.Null)
            {
                // COLLISION
                var enemy = enemyGroup[enemyEntity];
                //enemy.Value = 0;
                //enemyGroup[enemyEntity] = enemy;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new CollisionSampleJob
        {
            enemyGroup = GetComponentDataFromEntity<HealthData>(),
            bulletGroup = GetComponentDataFromEntity<DealsDamage>()
        };

        return job.Schedule(_stepPhysicsWorldSystem.Simulation, ref _buildPhysicsWorldSystem.PhysicsWorld,
            inputDependencies);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    static bool CheckCollision(float3 posA, float3 posB, float radiusSqr)
    {
        float3 delta = posA - posB;
        float distanceSquare = delta.x * delta.x + delta.z * delta.z;

        return distanceSquare <= radiusSqr;
    }
}