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
            dealDamageGroup = GetComponentDataFromEntity<DealsDamage>(true)
        };
        
        damageCollisionjob.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps).Complete();

        return inputDeps;
    }

    private struct DamageCollisionJob : ITriggerEventsJob
    {
        public BufferFromEntity<Damage> damageGroup;
        [ReadOnly] public ComponentDataFromEntity<DealsDamage> dealDamageGroup;
        
        public void Execute(TriggerEvent triggerEvent)
        {
            if (dealDamageGroup.HasComponent(triggerEvent.Entities.EntityA))
            {
                if (damageGroup.Exists(triggerEvent.Entities.EntityB))
                {
                    damageGroup[triggerEvent.Entities.EntityB].Add(new Damage
                    {
                        Value = dealDamageGroup[triggerEvent.Entities.EntityA].Value
                    });
                }
            }

            if (dealDamageGroup.HasComponent(triggerEvent.Entities.EntityB))
            {
                if (damageGroup.Exists(triggerEvent.Entities.EntityA))
                {
                    damageGroup[triggerEvent.Entities.EntityA].Add(new Damage
                    {
                        Value = dealDamageGroup[triggerEvent.Entities.EntityB].Value
                    });
                }
            }
        }
    }
}
