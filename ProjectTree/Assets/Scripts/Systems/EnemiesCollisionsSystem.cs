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

[UpdateAfter(typeof(EndFramePhysicsSystem))]
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
            if (directionB.Equals(float3.zero) && ForwardCollsion(calculateHitPoint, directionA, entityA))
            {
                var aiData = enemiesGroup[entityA];
                if (aiData.state == 1)
                {
                    aiData.stop = true;
                    enemiesGroup[entityA] = aiData;
                    CheckAndAdd(CollisionBuffers[entityA], entityB);
                }
            }
            // else
            // {
            //     // var aiData = enemiesGroup[entityA];
            //     // if (aiData.stop)
            //     // {
            //     //     aiData.stop = false;
            //     //     enemiesGroup[entityA] = aiData;
            //     //     CheckAndRemove(CollisionBuffers[entityA], entityB);
            //     // }
            // }
        }

        private DynamicBuffer<CollisionEnemy> CheckAndRemove(DynamicBuffer<CollisionEnemy> buffer, Entity entity)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].AiData.Equals(enemiesGroup[entity]))
                {
                    buffer.RemoveAt(i);
                    return buffer;
                }
            }
            return buffer;
        }
        private DynamicBuffer<CollisionEnemy> CheckAndAdd(DynamicBuffer<CollisionEnemy> buffer, Entity entity)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].AiData.Equals(enemiesGroup[entity]))
                    return buffer;
            }

            buffer.Add(new CollisionEnemy() {AiData = enemiesGroup[entity], Entity = entity});
            return buffer;
        }

        private bool ForwardCollsion(float3 calculateHitPoint, float3 direction, Entity entity)
        {
            if (direction.Equals(float3.zero))
            {
                if (enemiesGroup[entity].goToEntity)
                    direction = enemiesGroup[entity].entityPosition - TranslationGroup[entity].Value;
                else
                    direction = enemiesPositions[entity][enemiesGroup[entity].counter].position -
                                TranslationGroup[entity].Value;
            }

            var dot = calculateHitPoint.x * direction.x + calculateHitPoint.y * direction.y +
                      calculateHitPoint.z * direction.z;
            var magnitude = (Magnitude(calculateHitPoint) * Magnitude(direction));
            float angle = acos(dot / magnitude) * 180 / PI;
            return angle <= 15 && angle >= 0;
        }


        private float3 CalculateHitPoint(Translation translation, Translation translation1)
        {
            return normalize(translation1.Value - translation.Value);
        }
    }

    private static float Magnitude(float3 vector)
    {
        var magnitude = math.sqrt(math.pow(vector.x, 2) + math.sqrt(math.pow(vector.y, 2)) + math.pow(vector.z, 2));
        return magnitude;
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