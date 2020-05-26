using System;
using System.Collections.Generic;
using Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.AI;
using Random = UnityEngine.Random;

[UpdateAfter(typeof(EnemiesCollisionsSystem))]
[UpdateBefore(typeof(MovementSystem))]
[AlwaysSynchronizeSystem]
public class MoveToSystem : JobComponentSystem
{
    private static float Magnitude(float3 vector)
    {
        var magnitude = math.sqrt(math.pow(vector.x, 2) + math.sqrt(math.pow(vector.y, 2)) + math.pow(vector.z, 2));
        return magnitude;
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var buffers = GetBufferFromEntity<EnemyPosition>();
        var translations = GetComponentDataFromEntity<Translation>();


        Entities
            .ForEach(
                (ref AIData aiData, ref Translation translation, ref MovementData movementData,
                    ref Entity entity, ref DynamicBuffer<CollisionEnemy> collisionEnemies) =>
                {
                    aiData.state = 1;
                    if (aiData.stop)
                    {
                        movementData = StopMovement(movementData);

                        var direction = float3.zero;
                        if (aiData.goToEntity)
                            direction = aiData.entityPosition - translation.Value;
                        else
                            direction = buffers[entity][aiData.counter].position - translation.Value;
                        movementData = SetRotation(movementData, direction, aiData.canFly);

                        if (aiData.goToEntity)
                        {
                            direction = translations[aiData.entity].Value - translation.Value;
                            var magnitude = Magnitude(direction);
                            if (magnitude > aiData.attackDistancePlayer)
                                aiData.stop = false;
                        }
                        else
                        {
                            direction = translation.Value - buffers[entity][buffers[entity].Length - 1].position;
                            if (Magnitude(direction) > 1 && aiData.counter < buffers[entity].Length - 1)
                                aiData.stop = false;
                        }
                    }
                    else
                    {
                        if (aiData.goToEntity)
                        {
                            float3 direction;

                            if (aiData.canFly)
                                direction = new float3(translations[aiData.entity].Value.x, 0,
                                    translations[aiData.entity].Value.z) - new float3(translation.Value.x,
                                    0, translation.Value.z);
                            else
                                direction = translations[aiData.entity].Value - translation.Value;

                            var magnitude = Magnitude(direction);
                            if (magnitude < aiData.attackDistancePlayer)
                            {
                                movementData = StopMovement(movementData);
                            }
                            else
                            {
                                movementData = SetDirection(movementData, direction, magnitude);
                            }

                            if (aiData.entity != Entity.Null)
                                movementData = SetRotation(movementData,
                                    translations[aiData.entity].Value - translation.Value, aiData.canFly);

                            if (aiData.counter < buffers[entity].Length)
                            {
                                direction = buffers[entity][aiData.counter].position - translation.Value;
                                if (Magnitude(direction) < 1) aiData.counter++;
                            }
                        }
                        else
                        {
                            var position = buffers[entity][aiData.counter].position;
                            var direction = position - translation.Value;
                            var magnitude = Magnitude(direction);
                            if (magnitude < 1)
                            {
                                direction = translation.Value - buffers[entity][buffers[entity].Length - 1].position;
                                if (Magnitude(direction) > 1 && aiData.counter < buffers[entity].Length - 1)
                                    aiData.counter++;
                                else
                                {
                                    aiData.counter = buffers[entity].Length - 1;
                                    movementData = StopMovement(movementData);
                                }
                            }
                            else
                            {
                                movementData = SetRotation(movementData, direction, aiData.canFly);
                                movementData = SetDirection(movementData, direction, magnitude);
                            }
                        }
                    }
                }).Run();
        return default;
    }

    private static bool CheckCollisions(DynamicBuffer<CollisionEnemy> buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i].AiData.stop)
                return true;
        }

        return false;
    }

    private static MovementData StopMovement(MovementData movementData)
    {
        movementData.directionX = 0;
        movementData.directionY = 0;
        movementData.directionZ = 0;
        return movementData;
    }

    private static MovementData SetDirection(MovementData movementData, float3 direction, float magnitude)
    {
        direction /= magnitude;
        movementData.directionX = direction.x;
        movementData.directionY = direction.y;
        movementData.directionZ = direction.z;
        movementData.freezePosY = false;
        return movementData;
    }

    private static MovementData SetRotation(MovementData movementData, float3 direction, bool fly)
    {
        if (fly)
        {
            var rotation = quaternion.LookRotation(direction, math.up());
            movementData.rotation = math.mul(rotation, quaternion.Euler(0, -89.5f, 0));
        }
        else
        {
            var rotation = quaternion.LookRotation(direction, math.up());
            movementData.rotation = math.mul(rotation, quaternion.Euler(0, -89.5f, 0));
        }

        return movementData;
    }
}