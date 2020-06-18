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


    protected override void OnCreate()
    {
        _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var towerGroup = GetComponentDataFromEntity<TowerTag>();
        var buffTowerGroup = GetComponentDataFromEntity<BuffTurretData>();
        var enemyGroup = GetComponentDataFromEntity<AIData>();
        var playerGroup = GetComponentDataFromEntity<PlayerTag>();

        var findTargetJob = new FindTargetTriggerJob()
        {
            towerGroup = towerGroup,
            buffTowerGroup = buffTowerGroup,
            enemyGroup = enemyGroup,
            playerGroup = playerGroup
        };

        findTargetJob.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps).Complete();

        return inputDeps;
    }

    private struct FindTargetTriggerJob : ITriggerEventsJob
    {
        public ComponentDataFromEntity<TowerTag> towerGroup;
        public ComponentDataFromEntity<BuffTurretData> buffTowerGroup;
        public ComponentDataFromEntity<AIData> enemyGroup;
        public ComponentDataFromEntity<PlayerTag> playerGroup;


        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.Entities.EntityA;
            Entity entityB = triggerEvent.Entities.EntityB;
            if (enemyGroup.HasComponent(entityA))
            {
                if (towerGroup.Exists(entityB) || buffTowerGroup.Exists(entityB))
                {
                    AddEnemyInRange(entityB, entityA);
                }
            }

            if (enemyGroup.HasComponent(entityB))
            {
                if (towerGroup.Exists(entityA) || buffTowerGroup.Exists(entityA))
                {
                    AddEnemyInRange(entityA, entityB);
                }
            }
        }

        private void AddEnemyInRange(Entity turret, Entity enemy)
        {
            var aiData = enemyGroup[enemy];
            if (!aiData.goToEntity && !aiData.boss && !aiData.horde &&
                (!playerGroup.Exists(turret) || playerGroup.Exists(turret) && aiData.canAttackPlayer) ||
                aiData.horde && aiData.hordeMove)
            {
                aiData.entity = turret;
                aiData.goToEntity = true;
                enemyGroup[enemy] = aiData;
            }
        }
    }
}