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
public class TrapDamageSystem : JobComponentSystem
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
            dealDamageGroup = GetComponentDataFromEntity<TrapComponent>()
        };
        damageCollisionjob.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps).Complete();

        return inputDeps;
    }
    
    
    private struct DamageCollisionJob : ITriggerEventsJob
    {
        public BufferFromEntity<Damage> damageGroup;
        public ComponentDataFromEntity<TrapComponent> dealDamageGroup;

        public void Execute(TriggerEvent triggerEvent)
        {
            if (dealDamageGroup.HasComponent(triggerEvent.Entities.EntityA))
            {
                var trapComponent = dealDamageGroup[triggerEvent.Entities.EntityA];
                if (damageGroup.Exists(triggerEvent.Entities.EntityB)&&!trapComponent.cankill)
                {
                    damageGroup[triggerEvent.Entities.EntityB].Add(new Damage
                    {
                        Value = dealDamageGroup[triggerEvent.Entities.EntityA].Damage
                    });
                    //trapComponent.Deaths++;
                    trapComponent.cankill = true;
                    dealDamageGroup[triggerEvent.Entities.EntityA] = trapComponent;
                }
            }

            if (dealDamageGroup.HasComponent(triggerEvent.Entities.EntityB))
            {
                var trapComponent = dealDamageGroup[triggerEvent.Entities.EntityB];
                if (damageGroup.Exists(triggerEvent.Entities.EntityA)&&!trapComponent.cankill)
                {
                    damageGroup[triggerEvent.Entities.EntityA].Add(new Damage
                    {
                        Value = dealDamageGroup[triggerEvent.Entities.EntityB].Damage
                    });
                    
                    //trapComponent.Deaths++;
                    trapComponent.cankill = true;
                    dealDamageGroup[triggerEvent.Entities.EntityB] = trapComponent;
                }
            }
        }
    }
}
