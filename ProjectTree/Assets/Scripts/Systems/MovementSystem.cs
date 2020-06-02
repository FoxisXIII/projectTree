using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using float3 = Unity.Mathematics.float3;

namespace Systems
{
    [AlwaysSynchronizeSystem]
    public class MovementSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            float deltaTime = UnityEngine.Time.deltaTime;
            Entities.ForEach((ref PhysicsVelocity velocity, ref Rotation rotation, ref Translation translation,
                in MovementData movementData) =>
            {
                velocity.Linear.x = movementData.directionX * movementData.speed * deltaTime;
                if (!movementData.freezePosY)
                    velocity.Linear.y = movementData.directionY * movementData.speed * deltaTime;
                else
                    velocity.Linear.y = 0;
                velocity.Linear.z = movementData.directionZ * movementData.speed * deltaTime;

                if (movementData.lockRotation)
                {
                    velocity.Angular.x = 0;
                    velocity.Angular.y = 0;
                    velocity.Angular.z = 0;

                    rotation.Value = Quaternion.Lerp(rotation.Value, movementData.rotation, 2 * deltaTime);
                }
            }).Run();
            return default;
        }
    }
}