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
                velocity.Linear.y = movementData.directionY * movementData.speed * deltaTime;
                velocity.Linear.z = movementData.directionZ * movementData.speed * deltaTime;

                velocity.Angular.x = 0;
                velocity.Angular.y = 0;
                velocity.Angular.z = 0;
            }).Run();
            return default;
        }
    }
}