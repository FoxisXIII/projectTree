using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.Rendering;

[UpdateAfter(typeof(DamageCollisionSystem))]
public class DestroyOnContactSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem ecbSystem;
    private BuildPhysicsWorld _buildPhysicsWorld;
    private StepPhysicsWorld _stepPhysicsWorld;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var destroyGroup = GetComponentDataFromEntity<DestroyOnContact>(true);
        var towerGroup = GetComponentDataFromEntity<TowerTag>(true);
        var aiDataGroup = GetComponentDataFromEntity<AIData>(true);
        var ecb = ecbSystem.CreateCommandBuffer();

        var destroyCollisionJob = new DestroyCollisionJob
        {
            ecb = ecb,
            destroyGroup = destroyGroup,
            towerGroup = towerGroup,
            aiDataGroup = aiDataGroup
        };

        destroyCollisionJob.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps)
            .Complete();

        return inputDeps;
    }

    private struct DestroyCollisionJob : ICollisionEventsJob
    {
        public EntityCommandBuffer ecb;
        [ReadOnly] public ComponentDataFromEntity<DestroyOnContact> destroyGroup;
        [ReadOnly] public ComponentDataFromEntity<TowerTag> towerGroup;
        [ReadOnly] public ComponentDataFromEntity<AIData> aiDataGroup;

        public void Execute(CollisionEvent collisionEvent)
        {
            if (destroyGroup.HasComponent(collisionEvent.Entities.EntityA))
            {
                CheckDestroy(collisionEvent.Entities.EntityA, collisionEvent.Entities.EntityB);
            }

            if (destroyGroup.HasComponent(collisionEvent.Entities.EntityB))
            {
                CheckDestroy(collisionEvent.Entities.EntityB, collisionEvent.Entities.EntityA);
            }
        }

        private void CheckDestroy(Entity toDestroy, Entity entity)
        {
            if (destroyGroup[toDestroy].notDestroying == 0 && !towerGroup.Exists(entity))
                ecb.DestroyEntity(toDestroy);
            else if (destroyGroup[toDestroy].notDestroying == 1 && !aiDataGroup.Exists(entity))
                ecb.DestroyEntity(toDestroy);
        }
    }
}