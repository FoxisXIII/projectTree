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
                
                if(directionB.Equals(float3.zero))
                {
                    AIData characterA = CharactersGroup[entityA];
                    characterA.stop = true;
                    CharactersGroup[entityA] = characterA;
                }
                if(directionA.Equals(float3.zero))
                {
                    AIData characterB = CharactersGroup[entityB];
                    characterB.stop = true;
                    CharactersGroup[entityB] = characterB;
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
            CharactersGroup = GetComponentDataFromEntity<AIData>(),
        };
        JobHandle collisionHandle =
            collisionJob.Schedule(stepPhysicsWorldSystem.Simulation, ref physicsWorld, inputDependencies);

        return collisionHandle;
    }
}