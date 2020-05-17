using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using float3 = Unity.Mathematics.float3;

namespace Systems
{
    [AlwaysSynchronizeSystem]
    public class MovementSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            float deltaTime = UnityEngine.Time.deltaTime;
            Entities.ForEach((ref PhysicsVelocity velocity, ref Rotation rotation, in MovementData movementData) =>
            {
                velocity.Linear.x = movementData.directionX * movementData.speed * deltaTime;
                velocity.Linear.y = movementData.directionY * movementData.speed * deltaTime;
                velocity.Linear.z = movementData.directionZ * movementData.speed * deltaTime;
                velocity.Angular.y = movementData.rotationY * movementData.speed * deltaTime;

                if (movementData.lockRotation)
                {
                    velocity.Angular.x = 0;
                    velocity.Angular.z = 0;
                    rotation.Value.value.x = 0;
                    rotation.Value.value.y = 0;
                    rotation.Value.value.z = 0;
                }
            }).Run();
            return default;
        }
    }
}