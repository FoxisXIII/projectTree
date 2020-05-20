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
        var ecb = ecbSystem.CreateCommandBuffer();

        var destroyCollisionJob = new DestroyCollisionJob
        {
            ecb = ecb,
            destroyGroup = destroyGroup,
            towerGroup = towerGroup
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

        public void Execute(CollisionEvent collisionEvent)
        {
            if (!towerGroup.HasComponent(collisionEvent.Entities.EntityA) &&
                !towerGroup.HasComponent(collisionEvent.Entities.EntityB))
            {
                if (destroyGroup.HasComponent(collisionEvent.Entities.EntityA))
                {
                    ecb.DestroyEntity(collisionEvent.Entities.EntityA);
                }

                if (destroyGroup.HasComponent(collisionEvent.Entities.EntityB))
                {
                    ecb.DestroyEntity(collisionEvent.Entities.EntityB);
                }
            }
        }
    }
}