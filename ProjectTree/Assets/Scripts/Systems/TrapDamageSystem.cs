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
            aiDataGroup = GetComponentDataFromEntity<AIData>(),
            trapGroup = GetComponentDataFromEntity<TrapComponent>()
        };
        damageCollisionjob.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps)
            .Complete();

        return inputDeps;
    }


    private struct DamageCollisionJob : ITriggerEventsJob
    {
        public BufferFromEntity<Damage> damageGroup;
        public ComponentDataFromEntity<AIData> aiDataGroup;
        public ComponentDataFromEntity<TrapComponent> trapGroup;

        public void Execute(TriggerEvent triggerEvent)
        {
            if (trapGroup.HasComponent(triggerEvent.Entities.EntityA))
            {
                UseTrap(triggerEvent.Entities.EntityA, triggerEvent.Entities.EntityB);
            }

            if (trapGroup.HasComponent(triggerEvent.Entities.EntityB))
            {
                UseTrap(triggerEvent.Entities.EntityB, triggerEvent.Entities.EntityA);
            }
        }

        private void UseTrap(Entity trap, Entity enemy)
        {
            var trapComponent = trapGroup[trap];
            if (damageGroup.Exists(enemy) && trapComponent.cankill)
            {
                if (aiDataGroup.Exists(enemy) && !aiDataGroup[enemy].boss)
                {
<<<<<<< HEAD
                    damageGroup[enemy].Add(new Damage
                    {
                        Value = trapComponent.Damage
                    });
                    trapComponent.cankill = false;
                    trapComponent.Recover = 0;
                    trapComponent.times--;
                    trapGroup[trap] = trapComponent;
                }
=======
                    Value = trapComponent.Damage
                });
                trapComponent.cankill = false;
                trapComponent.Recover = 0;
                trapGroup[trap] = trapComponent;
                SoundManager.GetInstance().PlayOneShotSound("event:/FX/Menu/Select", Vector3.zero);
>>>>>>> develop
            }
        }
    }
}