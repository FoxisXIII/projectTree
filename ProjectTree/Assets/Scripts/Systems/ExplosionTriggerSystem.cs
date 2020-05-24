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
public class ExplosionTriggerSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorldSystem;
    private StepPhysicsWorld stepPhysicsWorldSystem;

    [BurstCompile]
    public struct ExplosionTriggerJob : ITriggerEventsJob
    {
        public BufferFromEntity<Damage> damageGroup;
        public ComponentDataFromEntity<AIData> enemiesGroup;
        public ComponentDataFromEntity<ExplosionComponent> explosionComponent;

        public void Execute(TriggerEvent triggerEvent)
        {
            if (explosionComponent.HasComponent(triggerEvent.Entities.EntityA))
            {
                MakeDamage(triggerEvent.Entities.EntityA, triggerEvent.Entities.EntityB);
            }

            if (explosionComponent.HasComponent(triggerEvent.Entities.EntityB))
            {
                MakeDamage(triggerEvent.Entities.EntityB, triggerEvent.Entities.EntityA);
            }
        }

        private void MakeDamage(Entity entityA, Entity entityB)
        {
            if (explosionComponent[entityA].explode)
            {
                if (damageGroup.Exists(entityB) &&
                    enemiesGroup.Exists(entityB))
                {
                    damageGroup[entityB].Add(new Damage
                    {
                        Value = explosionComponent[entityA].damage
                    });
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

        var triggerJob = new ExplosionTriggerJob
        {
            damageGroup = GetBufferFromEntity<Damage>(),
            explosionComponent = GetComponentDataFromEntity<ExplosionComponent>(),
            enemiesGroup = GetComponentDataFromEntity<AIData>()
        };
        JobHandle collisionHandle =
            triggerJob.Schedule(stepPhysicsWorldSystem.Simulation, ref physicsWorld, inputDependencies);

        return collisionHandle;
    }
}