﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
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
            fmodGroup = GetComponentDataFromEntity<EnemyFMODPaths>(true),
            translationGroup = GetComponentDataFromEntity<Translation>(true)
        };
        
        damageCollisionjob.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps).Complete();

        return inputDeps;
    }

    private struct DamageCollisionJob : ICollisionEventsJob
    {
        public BufferFromEntity<Damage> damageGroup;
        [ReadOnly] public ComponentDataFromEntity<DealsDamage> dealDamageGroup;
        [ReadOnly] public ComponentDataFromEntity<EnemyFMODPaths> fmodGroup;
        [ReadOnly] public ComponentDataFromEntity<Translation> translationGroup;
        
        public void Execute(CollisionEvent collisionEvent)
        {
            if (dealDamageGroup.HasComponent(collisionEvent.Entities.EntityA))
            {
                if (damageGroup.Exists(collisionEvent.Entities.EntityB))
                {
                    damageGroup[collisionEvent.Entities.EntityB].Add(new Damage
                    {
                        Value = dealDamageGroup[collisionEvent.Entities.EntityA].Value
                    });
                    if (fmodGroup.Exists(collisionEvent.Entities.EntityB) && translationGroup.Exists(collisionEvent.Entities.EntityB))
                    {
                        SoundManager.GetInstance().PlayOneShotSound(fmodGroup[collisionEvent.Entities.EntityB].DiePath.ToString(), translationGroup[collisionEvent.Entities.EntityB].Value);
                    }
                }
            }

            if (dealDamageGroup.HasComponent(collisionEvent.Entities.EntityB))
            {
                if (damageGroup.Exists(collisionEvent.Entities.EntityA))
                {
                    damageGroup[collisionEvent.Entities.EntityA].Add(new Damage
                    {
                        Value = dealDamageGroup[collisionEvent.Entities.EntityB].Value
                    });
                    if (fmodGroup.Exists(collisionEvent.Entities.EntityA) && translationGroup.Exists(collisionEvent.Entities.EntityA))
                    {
                        SoundManager.GetInstance().PlayOneShotSound(fmodGroup[collisionEvent.Entities.EntityA].DiePath.ToString(), translationGroup[collisionEvent.Entities.EntityA].Value);
                    }
                }
            }
        }
    }
}
