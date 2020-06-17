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
        public ComponentDataFromEntity<ExplosionComponent> explosionGroup;
        public ComponentDataFromEntity<Translation> translationGroup;

        public void Execute(TriggerEvent triggerEvent)
        {
            if (explosionGroup.HasComponent(triggerEvent.Entities.EntityA))
            {
                MakeDamage(triggerEvent.Entities.EntityA, triggerEvent.Entities.EntityB);
            }

            if (explosionGroup.HasComponent(triggerEvent.Entities.EntityB))
            {
                MakeDamage(triggerEvent.Entities.EntityB, triggerEvent.Entities.EntityA);
            }
        }

        private void MakeDamage(Entity bomb, Entity enemy)
        {
            var explosionComponent = explosionGroup[bomb];
            if (explosionComponent.explode)
            {
                if (damageGroup.Exists(enemy) &&
                    enemiesGroup.Exists(enemy))
                {
                    var damage = (int) (explosionComponent.damage *
                                        (1 - min(1,
                                            (distance(translationGroup[bomb].Value, translationGroup[enemy].Value) -
                                             2) / 6)));
                    damageGroup[enemy].Add(new Damage
                    {
                        Value = damage
                    });
                }
            }
            else
            {
                if (damageGroup.Exists(enemy) &&
                    enemiesGroup.Exists(enemy))
                {
                    if (distance(translationGroup[bomb].Value,
                            translationGroup[enemy].Value) < 2f)
                    {
                        explosionComponent.timer =  explosionComponent.ttl;
                        explosionGroup[bomb] = explosionComponent;
                    }
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
            explosionGroup = GetComponentDataFromEntity<ExplosionComponent>(),
            enemiesGroup = GetComponentDataFromEntity<AIData>(),
            translationGroup = GetComponentDataFromEntity<Translation>()
        };
        JobHandle collisionHandle =
            triggerJob.Schedule(stepPhysicsWorldSystem.Simulation, ref physicsWorld, inputDependencies);

        return collisionHandle;
    }
}