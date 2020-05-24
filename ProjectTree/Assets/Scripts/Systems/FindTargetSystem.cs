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
        var towerGroup = GetBufferFromEntity<EnemiesInRange>();
        var turretGroup = GetBufferFromEntity<TurretsInRange>();
        var enemyGroup = GetComponentDataFromEntity<AIData>();
        var playerTag = GetComponentDataFromEntity<PlayerTag>();

        var findTargetJob = new FindTargetTriggerJob()
        {
            towerGroup = towerGroup,
            turretGroup = turretGroup,
            enemyGroup = enemyGroup,
            playerTag = playerTag,
            ecb = this.ecb.CreateCommandBuffer()
        };

        findTargetJob.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps).Complete();

        return inputDeps;
    }

    private struct FindTargetTriggerJob : ITriggerEventsJob
    {
        public BufferFromEntity<EnemiesInRange> towerGroup;
        public BufferFromEntity<TurretsInRange> turretGroup;
        public ComponentDataFromEntity<AIData> enemyGroup;
        public ComponentDataFromEntity<PlayerTag> playerTag;
        public EntityCommandBuffer ecb;


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

            if (towerGroup.Exists(entityA) && !playerTag.HasComponent(entityA))
            {
                if (towerGroup.Exists(entityB) && !playerTag.HasComponent(entityB))
                {
                    if (!ContainsEntity(turretGroup[entityA], entityB))
                        turretGroup[entityA].Add(new TurretsInRange() {Value = entityB});
                    if (!ContainsEntity(turretGroup[entityB], entityA))
                        turretGroup[entityB].Add(new TurretsInRange() {Value = entityA});
                }
            }
        }

        private void AddEnemyInRange(Entity turret, Entity enemy)
        {
            if (towerGroup[turret].Length <= 0)
                ecb.AddComponent<GenerateAttackPositionComponent>(turret);
            if (!ContainsEntity(towerGroup[turret], enemy) && !enemyGroup[enemy].goToEntity)
            {
                if (!playerTag.HasComponent(turret) && towerGroup[turret].Length <=
                    (GameController.GetInstance().CurrentEnemies) /
                    (turretGroup[turret].Length + 1) ||
                    playerTag.HasComponent(turret) && towerGroup[turret].Length <= 15)
                {
                    towerGroup[turret].Add(new EnemiesInRange {Value = enemy});
                    var aiData = enemyGroup[enemy];
                    aiData.goToEntity = true;
                    enemyGroup[enemy] = aiData;
                }
                
                
            }
        }

        private bool ContainsEntity(DynamicBuffer<EnemiesInRange> buffer, Entity entity)
        {
            foreach (var enemiesInRange in buffer)
            {
                if (enemiesInRange.Value.Equals(entity))
                    return true;
            }

            return false;
        }

        private bool ContainsEntity(DynamicBuffer<TurretsInRange> buffer, Entity entity)
        {
            foreach (var enemiesInRange in buffer)
            {
                if (enemiesInRange.Value.Equals(entity))
                    return true;
            }

            return false;
        }
    }
}