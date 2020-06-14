using Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

[UpdateBefore(typeof(MoveToSystem))]
[UpdateBefore(typeof(MovementSystem))]
public class EnemiesCollisionsSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorldSystem;
    private StepPhysicsWorld stepPhysicsWorldSystem;

    [BurstCompile]
    public struct CollisionJob : ICollisionEventsJob
    {
        public ComponentDataFromEntity<AIData> enemiesGroup;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.Entities.EntityA;
            Entity entityB = collisionEvent.Entities.EntityB;

            if (enemiesGroup.Exists(entityA) && enemiesGroup.Exists(entityB))
            {
                StopEntity(entityA, entityB);
                StopEntity(entityB, entityA);
            }
        }

        private void StopEntity(Entity entityA,
            Entity entityB)
        {
            var aiDataB = enemiesGroup[entityB];
            if (aiDataB.stop)
            {
                var aiData = enemiesGroup[entityA];
                if (aiData.state == 1)
                {
                    aiData.stop = true;
                    aiData.stopByCollision = true;
                    enemiesGroup[entityA] = aiData;
                    // Debug.LogError(entityA + " : " + aiData.stopByCollision + " - " + entityB);
                }
            }
        }
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        inputDependencies = JobHandle.CombineDependencies(inputDependencies, buildPhysicsWorldSystem.FinalJobHandle);

        var physicsWorld = buildPhysicsWorldSystem.PhysicsWorld;

        var collisionJob = new CollisionJob
        {
            enemiesGroup = GetComponentDataFromEntity<AIData>(),
        };
        JobHandle collisionHandle =
            collisionJob.Schedule(stepPhysicsWorldSystem.Simulation, ref physicsWorld, inputDependencies);

        return collisionHandle;
    }
}