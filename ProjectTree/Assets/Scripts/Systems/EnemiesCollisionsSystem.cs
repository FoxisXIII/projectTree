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

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.Entities.EntityA;
            Entity entityB = collisionEvent.Entities.EntityB;
            bool entityAIsCharacter = CharactersGroup.Exists(entityA);
            bool entityBIsCharacter = CharactersGroup.Exists(entityB);


            if (entityAIsCharacter && entityBIsCharacter)
            {
                var calculateHitPoint = CalculateHitPoint(TranslationGroup[entityA], TranslationGroup[entityB]);

                var directionA = new float3(MovementGroup[entityA].directionX, MovementGroup[entityA].directionY,
                    MovementGroup[entityA].directionZ);
                var directionB = new float3(MovementGroup[entityB].directionX, MovementGroup[entityB].directionY,
                    MovementGroup[entityB].directionZ);


                StopEntity(directionB, calculateHitPoint, directionA, entityA);
                StopEntity(directionA, calculateHitPoint, directionB, entityB);
            }
        }

        private void StopEntity(float3 directionB, float3 calculateHitPoint, float3 directionA, Entity entityA)
        {
            // NoForwardCollsion(calculateHitPoint, directionA, TranslationGroup[entityA].Value, entityA);
            if (directionB.Equals(float3.zero))
                // if (directionB.Equals(float3.zero) && NoForwardCollsion(calculateHitPoint, directionA))
            {
                AIData characterA = CharactersGroup[entityA];
                if (characterA.state == 1)
                {
                    characterA.stop = true;
                    CharactersGroup[entityA] = characterA;
                }
            }
        }

        private bool NoForwardCollsion(float3 calculateHitPoint, float3 direction)
            // private bool NoForwardCollsion(float3 calculateHitPoint, float3 direction, float3 translation, Entity entityA)
        {
            // Debug.Log("Entity = " + entityA + ", Collision = " + (translation + calculateHitPoint) + ",Direction = " +
            //           (translation + direction) + ", Angle = " +
            // );
            
            //S'HA DE LLAÇAR UN RAYCAST
            return Vector3.SignedAngle(calculateHitPoint, direction, Vector3.up) == 0;
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
            CharactersGroup = GetComponentDataFromEntity<AIData>(),
        };
        JobHandle collisionHandle =
            collisionJob.Schedule(stepPhysicsWorldSystem.Simulation, ref physicsWorld, inputDependencies);

        return collisionHandle;
    }
}