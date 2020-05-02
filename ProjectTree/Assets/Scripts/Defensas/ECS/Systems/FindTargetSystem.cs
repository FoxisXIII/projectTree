using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public class FindTargetSystem : JobComponentSystem
{
    // protected override void OnUpdate()
    // {
    //     Entities.WithNone<TowerCurrentTarget>().WithAll<TowerTag>().ForEach((Entity a, ref Translation position) => 
    //     {
    //         Entity closestTarget = Entity.Null;
    //         float3 turretPosition = position.Value;
    //         float3 closestPosition = float3.zero;
    //         Entities.WithAll<EnemyTag>().ForEach((Entity b, ref Translation targetPosition) => 
    //         {
    //             if (closestTarget == Entity.Null)
    //             {
    //                 closestTarget = b;
    //                 closestPosition = targetPosition.Value;
    //             }
    //             else
    //             {
    //                 if (math.distance(turretPosition, targetPosition.Value) < math.distance(turretPosition, closestPosition))
    //                 {
    //                     closestTarget = b;
    //                     closestPosition = targetPosition.Value;
    //                 }
    //             }
    //         });
    //
    //         if (closestTarget != Entity.Null)
    //         {
    //             //Debug.Log(closestTarget);
    //             PostUpdateCommands.AddComponent(a, new TowerCurrentTarget{target = closestTarget});
    //         }
    //     });}
    
    private BuildPhysicsWorld _buildPhysicsWorld;
    private StepPhysicsWorld _stepPhysicsWorld;
    
    protected override void OnCreate()
    {
        _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var turretGroup = GetBufferFromEntity<EnemiesInRange>();
        var enemyGroup = GetComponentDataFromEntity<EnemyTag>(true);
        
        var findTargetJob = new FindTargetTriggerJob()
        {
            towerGroup = turretGroup,
            enemyGroup = enemyGroup
        };
        
        findTargetJob.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps).Complete();
        
        return inputDeps;
    }
    
    [RequireComponentTag(typeof(EnemiesInRange))]
    private struct FindTargetTriggerJob : ITriggerEventsJob
    {
        public BufferFromEntity<EnemiesInRange> towerGroup;
        [ReadOnly] public ComponentDataFromEntity<EnemyTag> enemyGroup;
        
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.Entities.EntityA;
            Entity entityB = triggerEvent.Entities.EntityB;
            if (enemyGroup.HasComponent(entityA))
            {
                if (towerGroup.Exists(entityB))
                {
                    towerGroup[entityB].Add(new EnemiesInRange
                        {Value = entityA});
                }
            }

            if (enemyGroup.HasComponent(entityB))
            {
                if (towerGroup.Exists(entityA))
                {
                    towerGroup[entityA].Add(new EnemiesInRange
                    {
                        Value = entityB
                    });
                }
            }
        }
    }
}


