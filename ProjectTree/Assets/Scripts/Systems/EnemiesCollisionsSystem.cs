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
        public ComponentDataFromEntity<AIData> CharactersGroup;
        public BufferFromEntity<EnemyPosition> buffers;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.Entities.EntityA;
            Entity entityB = collisionEvent.Entities.EntityB;
            bool entityAIsCharacter = CharactersGroup.Exists(entityA);
            bool entityBIsCharacter = CharactersGroup.Exists(entityB);


            if (entityAIsCharacter && entityBIsCharacter)
            {
                var calculateHitPointA = CalculateHitPoint(TranslationGroup[entityA], TranslationGroup[entityB]);
                var calculateHitPointB = CalculateHitPoint(TranslationGroup[entityB], TranslationGroup[entityA]);

                var directionA = new float3(MovementGroup[entityA].directionX, MovementGroup[entityA].directionY,
                    MovementGroup[entityA].directionZ);
                var directionB = new float3(MovementGroup[entityB].directionX, MovementGroup[entityB].directionY,
                    MovementGroup[entityB].directionZ);


                StopEntity(directionB, calculateHitPointA, directionA, entityA);
                StopEntity(directionA, calculateHitPointB, directionB, entityB);
            }
        }

        private void StopEntity(float3 directionB, float3 calculateHitPoint, float3 directionA, Entity entity)
        {
            if (directionB.Equals(float3.zero) )//&& ForwardCollsion(calculateHitPoint, directionA, entity))
            {
                AIData aiData = CharactersGroup[entity];
                if (aiData.state == 1)
                {
                    aiData.stop = true;
                    CharactersGroup[entity] = aiData;
                }
            }
        }

        private bool ForwardCollsion(float3 calculateHitPoint, float3 direction, Entity entity)
        {
            if (direction.Equals(float3.zero))
            {
                if (CharactersGroup[entity].goToEntity)
                    direction = CharactersGroup[entity].entityPosition - TranslationGroup[entity].Value;
                else
                    direction = buffers[entity][CharactersGroup[entity].counter].position -
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
            CharactersGroup = GetComponentDataFromEntity<AIData>(),
            buffers = GetBufferFromEntity<EnemyPosition>()
        };
        JobHandle collisionHandle =
            collisionJob.Schedule(stepPhysicsWorldSystem.Simulation, ref physicsWorld, inputDependencies);

        return collisionHandle;
    }
}