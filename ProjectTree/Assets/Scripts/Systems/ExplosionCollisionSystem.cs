using Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

[UpdateBefore(typeof(DamageCollisionSystem))]
public class ExplosionCollisionSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorldSystem;
    private StepPhysicsWorld stepPhysicsWorldSystem;

    [BurstCompile]
    public struct ExplosionCollsionJob : ICollisionEventsJob
    {
        public BufferFromEntity<Damage> damageGroup;
        public ComponentDataFromEntity<AIData> enemiesGroup;
        public ComponentDataFromEntity<ExplosionComponent> explosionComponent;
        public ComponentDataFromEntity<MovementData> movementGroup;

        public void Execute(CollisionEvent collisionEvent)
        {
            if (explosionComponent.HasComponent(collisionEvent.Entities.EntityA))
            {
                MakeDamage(collisionEvent.Entities.EntityA, collisionEvent.Entities.EntityB);
            }

            if (explosionComponent.HasComponent(collisionEvent.Entities.EntityB))
            {
                MakeDamage(collisionEvent.Entities.EntityB, collisionEvent.Entities.EntityA);
            }
        }

        private void MakeDamage(Entity entityA, Entity entityB)
        {
            if (!explosionComponent[entityA].explode)
            {
                if (damageGroup.Exists(entityB) &&
                    enemiesGroup.Exists(entityB))
                {
                    var movementData = movementGroup[entityA];
                    movementData.speed = 0;
                    movementGroup[entityA] = movementData;
                }
            }
        }
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        inputDependencies = JobHandle.CombineDependencies(inputDependencies, buildPhysicsWorldSystem.FinalJobHandle);

        var physicsWorld = buildPhysicsWorldSystem.PhysicsWorld;

        var collsionJob = new ExplosionCollsionJob()
        {
            damageGroup = GetBufferFromEntity<Damage>(),
            explosionComponent = GetComponentDataFromEntity<ExplosionComponent>(),
            enemiesGroup = GetComponentDataFromEntity<AIData>(),
            movementGroup = GetComponentDataFromEntity<MovementData>()
        };
        JobHandle collisionHandle =
            collsionJob.Schedule(stepPhysicsWorldSystem.Simulation, ref physicsWorld, inputDependencies);

        return collisionHandle;
    }
}