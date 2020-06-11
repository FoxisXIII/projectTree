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

[UpdateAfter(typeof(MoveToSystem))]
[UpdateBefore(typeof(MovementSystem))]
public class EnemiesCollisionsSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorldSystem;
    private StepPhysicsWorld stepPhysicsWorldSystem;

    [BurstCompile]
    public struct CollisionJob : ICollisionEventsJob
    {
        public ComponentDataFromEntity<Translation> TranslationGroup;
        public ComponentDataFromEntity<MovementData> MovementGroup;
        public ComponentDataFromEntity<AIData> enemiesGroup;
        public BufferFromEntity<EnemyPosition> enemiesPositions;
        public BufferFromEntity<CollisionEnemy> CollisionBuffers;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.Entities.EntityA;
            Entity entityB = collisionEvent.Entities.EntityB;
            bool entityAIsCharacter = CollisionBuffers.Exists(entityA);
            bool entityBIsCharacter = CollisionBuffers.Exists(entityB);


            if (entityAIsCharacter && entityBIsCharacter)
            {
                var calculateHitPointA = CalculateHitPoint(TranslationGroup[entityA], TranslationGroup[entityB]);
                var calculateHitPointB = CalculateHitPoint(TranslationGroup[entityB], TranslationGroup[entityA]);

                var directionA = new float3(MovementGroup[entityA].directionX, MovementGroup[entityA].directionY,
                    MovementGroup[entityA].directionZ);
                var directionB = new float3(MovementGroup[entityB].directionX, MovementGroup[entityB].directionY,
                    MovementGroup[entityB].directionZ);


                StopEntity(directionB, calculateHitPointA, directionA, entityA, entityB);
                StopEntity(directionA, calculateHitPointB, directionB, entityB, entityA);
            }
        }

        private void StopEntity(float3 directionB, float3 calculateHitPoint, float3 directionA, Entity entityA,
            Entity entityB)
        {
            var aiDataB = enemiesGroup[entityB];
            if (aiDataB.stop)
            {
                var aiData = enemiesGroup[entityA];
                if (aiData.state == 1)
                {
                    aiData.stop = true;
                    enemiesGroup[entityA] = aiData;
                }
            }
        }

        private float3 CalculateHitPoint(Translation translation, Translation translation1)
        {
            return normalize(translation1.Value - translation.Value);
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
            TranslationGroup = GetComponentDataFromEntity<Translation>(),
            MovementGroup = GetComponentDataFromEntity<MovementData>(),
            enemiesGroup = GetComponentDataFromEntity<AIData>(),
            enemiesPositions = GetBufferFromEntity<EnemyPosition>(),
            CollisionBuffers = GetBufferFromEntity<CollisionEnemy>()
        };
        JobHandle collisionHandle =
            collisionJob.Schedule(stepPhysicsWorldSystem.Simulation, ref physicsWorld, inputDependencies);

        return collisionHandle;
    }
}