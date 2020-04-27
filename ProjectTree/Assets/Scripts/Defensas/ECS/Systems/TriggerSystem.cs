using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

[UpdateBefore(typeof(AimSystem))]
public class TriggerSystem /*: JobComponentSystem*/
{
    private BuildPhysicsWorld _buildPhysicsWorldSystem;

    private StepPhysicsWorld _stepPhysicsWorldSystem;
    
    // protected override void OnCreate()
    // {
    //     base.OnCreate();
    //     _buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
    //     _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
    // }
    
    //NativeArray<Entity> enemiesInRange = new NativeArray<Entity>(100, Allocator.Persistent);

    // [BurstCompile]
    // struct TriggerSampleJob : ITriggerEventsJob
    // {
    //     //[ReadOnly] public ComponentDataFromEntity<EnemyTag> EnemyGroup;
    //     //[ReadOnly] public ComponentDataFromEntity<TurretTag> TurretGroup;
    //
    //     public Entity GetEntityFromComponentGroup<T>(Entity entityA, Entity entityB,
    //         ComponentDataFromEntity<T> componentGroup) where T : struct, IComponentData
    //     {
    //         // if (componentGroup.Exists(entityA))
    //         //     return entityA;
    //         // if (componentGroup.Exists(entityB))
    //         //     return entityB;
    //          return Entity.Null;
    //     }
    //
    //
    //     public void Execute(TriggerEvent collisionEvent)
    //     {
    //         // var entityA = collisionEvent.Entities.EntityA;
    //         // var entityB = collisionEvent.Entities.EntityB;
    //         // //var EnemyEntity = GetEntityFromComponentGroup(entityA, entityB, EnemyGroup);
    //         // //var turretEntity = GetEntityFromComponentGroup(entityA, entityB, TurretGroup);
    //         // //if (EnemyEntity != Entity.Null && turretEntity != Entity.Null)
    //         // {
    //         //      // COLLISION
    //         //      ComponentDataFromEntity<PointedEnemyComponent> target;
    //         //      
    //         // }
    //     }
    // }
    //
    // protected override JobHandle OnUpdate(JobHandle inputDependencies)
    // {
    //     var job = new TriggerSampleJob();
    //
    //     // Assign values to the fields on your job here, so that it has
    //     // everything it needs to do its work when it runs later.
    //     // For example,
    //     //     job.deltaTime = UnityEngine.Time.deltaTime;
    //
    //
    //     // Now that the job is set up, schedule it to be run. 
    //     return job.Schedule(_stepPhysicsWorldSystem.Simulation, ref _buildPhysicsWorldSystem.PhysicsWorld,
    //         inputDependencies);
    // }
}
