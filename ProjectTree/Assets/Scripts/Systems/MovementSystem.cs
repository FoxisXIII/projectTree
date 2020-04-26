using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using float3 = Unity.Mathematics.float3;

namespace Systems
{
    [AlwaysSynchronizeSystem]
    public class MovementSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            float deltaTime = UnityEngine.Time.deltaTime;
            Entities.ForEach((ref PhysicsVelocity velocity, in MovementData movementData) =>
            {
                velocity.Linear.x = movementData.directionX * movementData.speed * deltaTime;
                velocity.Linear.z = movementData.directionZ * movementData.speed * deltaTime;
                
                velocity.Angular = float3.zero;
            }).Run();
            return default;
        }
    }
}