using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(GetClosestEnemySystem))]
public class FindTargetSystem : JobComponentSystem
{
    private BuildPhysicsWorld _buildPhysicsWorld;
    private StepPhysicsWorld _stepPhysicsWorld;

    private EndSimulationEntityCommandBufferSystem ecb;

    protected override void OnCreate()
    {
        ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var towerGroup = GetComponentDataFromEntity<TowerTag>();
        var enemyGroup = GetComponentDataFromEntity<AIData>();

        var findTargetJob = new FindTargetTriggerJob()
        {
            towerGroup = towerGroup,
            enemyGroup = enemyGroup
        };

        findTargetJob.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps).Complete();

        return inputDeps;
    }

    private struct FindTargetTriggerJob : ITriggerEventsJob
    {
        public ComponentDataFromEntity<TowerTag> towerGroup;
        public ComponentDataFromEntity<AIData> enemyGroup;


        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.Entities.EntityA;
            Entity entityB = triggerEvent.Entities.EntityB;
            if (enemyGroup.HasComponent(entityA))
            {
                if (towerGroup.Exists(entityB))
                {
                    AddEnemyInRange(entityB, entityA);
                }
            }

            if (enemyGroup.HasComponent(entityB))
            {
                if (towerGroup.Exists(entityA))
                {
                    AddEnemyInRange(entityA, entityB);
                }
            }
        }

        private void AddEnemyInRange(Entity turret, Entity enemy)
        {
            if (!enemyGroup[enemy].goToEntity)
            {
                var aiData = enemyGroup[enemy];
                aiData.entity = turret;
                aiData.goToEntity = true;
                enemyGroup[enemy] = aiData;
            }
        }
    }
}