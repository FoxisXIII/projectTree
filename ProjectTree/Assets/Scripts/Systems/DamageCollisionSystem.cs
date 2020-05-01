using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateBefore(typeof(ResolveDamageSystem))]
public class DamageCollisionSystem : JobComponentSystem
{
    private BuildPhysicsWorld _buildPhysicsWorld;
    private StepPhysicsWorld _stepPhysicsWorld;

    protected override void OnCreate()
    {
        _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var damageCollisionjob = new DamageCollisionJob
        {
            damageGroup = GetBufferFromEntity<Damage>(),
            dealDamageGroup = GetComponentDataFromEntity<DealsDamage>(true),
            deadGroup = GetComponentDataFromEntity<Dead>(true)
        };
        
        damageCollisionjob.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps).Complete();

        return inputDeps;
    }

    private struct DamageCollisionJob : ICollisionEventsJob
    {
        public BufferFromEntity<Damage> damageGroup;
        [ReadOnly] public ComponentDataFromEntity<DealsDamage> dealDamageGroup;
        [ReadOnly] public ComponentDataFromEntity<Dead> deadGroup;
        
        public void Execute(CollisionEvent collisionEvent)
        {
            if (dealDamageGroup.HasComponent(collisionEvent.Entities.EntityA))
            {
                if (damageGroup.Exists(collisionEvent.Entities.EntityB))
                {
                    // if (!deadGroup.Exists(collisionEvent.Entities.EntityB))
                    // {
                        damageGroup[collisionEvent.Entities.EntityB].Add(new Damage
                        {
                            Value = dealDamageGroup[collisionEvent.Entities.EntityA].Value
                        });
                    // }
                }
            }

            if (dealDamageGroup.HasComponent(collisionEvent.Entities.EntityB))
            {
                if (damageGroup.Exists(collisionEvent.Entities.EntityA))
                {
                    // if (deadGroup.Exists(collisionEvent.Entities.EntityA))
                    // {
                        damageGroup[collisionEvent.Entities.EntityA].Add(new Damage
                        {
                            Value = dealDamageGroup[collisionEvent.Entities.EntityB].Value
                        });
                    // }
                }
            }
        }
    }
}
