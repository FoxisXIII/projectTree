using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[AlwaysSynchronizeSystem]
public class MovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        float deltaTime = UnityEngine.Time.deltaTime;
        Entities.ForEach((ref PhysicsVelocity velocity, in MovementData movementData) =>
        {
            velocity.Linear.z = -movementData.speed * deltaTime;
        }).Run();
        return default;
    }
}