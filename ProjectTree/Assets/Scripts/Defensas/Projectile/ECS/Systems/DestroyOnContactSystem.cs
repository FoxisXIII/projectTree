using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

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
        var ecb = ecbSystem.CreateCommandBuffer();

        // var destroyTriggerJob = new DestroyTriggerJob
        // {
        //     ecb = ecb,
        //     destroyGroup = destroyGroup
        // };

        var destroyCollisionJob = new DestroyCollisionJob
        {
            ecb = ecb,
            destroyGroup = destroyGroup
        };

        // destroyTriggerJob.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps).Complete();
        destroyCollisionJob.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps).Complete();

        return inputDeps;
    }

    // private struct DestroyTriggerJob : ITriggerEventsJob
    // {
    //     public EntityCommandBuffer ecb;
    //     [ReadOnly] public ComponentDataFromEntity<DestroyOnContact> destroyGroup;
    //     
    //     public void Execute(TriggerEvent triggerEvent)
    //     {
    //         if (destroyGroup.HasComponent(triggerEvent.Entities.EntityA))
    //         {
    //             Debug.Log("A");
    //             ecb.DestroyEntity(triggerEvent.Entities.EntityA);
    //         }
    //         if (destroyGroup.HasComponent(triggerEvent.Entities.EntityB))
    //         {
    //             ecb.DestroyEntity(triggerEvent.Entities.EntityB);
    //             Debug.Log("B");
    //         }
    //         
    //     }
    // }
    
    private struct DestroyCollisionJob : ICollisionEventsJob
    {
        public EntityCommandBuffer ecb;
        [ReadOnly] public ComponentDataFromEntity<DestroyOnContact> destroyGroup;

        public void Execute(CollisionEvent collisionEvent)
        {
            if (destroyGroup.HasComponent(collisionEvent.Entities.EntityA))
            {
                ecb.DestroyEntity(collisionEvent.Entities.EntityA);
                Debug.Log("buena");
            }
            if (destroyGroup.HasComponent(collisionEvent.Entities.EntityB))
            {
                ecb.DestroyEntity(collisionEvent.Entities.EntityB);
                Debug.Log("buena");
            }
        }
    }
}
