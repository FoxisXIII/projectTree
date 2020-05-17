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


        Entities
            .ForEach(
                (ref AIData aiData, ref Translation translation, ref MovementData movementData,
                    ref Entity entity, ref DynamicBuffer<CollisionEnemy> collision) =>
                {
                    if (HasCollision(collision))
                    {
                        movementData.directionX = 0;
                        movementData.directionY = 0;
                        movementData.directionZ = 0;
                    }
                    else
                    {
                        aiData.state = 1;
                        if (aiData.goToEntity)
                        {
                            var direction = aiData.entityPosition - translation.Value;
                            var magnitude = Magnitude(direction);
                            if (magnitude < 1)
                            {
                                movementData.directionX = 0;
                                movementData.directionY = 0;
                                movementData.directionZ = 0;
                            }
                            else
                            {
                                direction /= magnitude;
                                movementData.directionX = direction.x;
                                movementData.directionY = 0;
                                movementData.directionZ = direction.z;
                                movementData.rotationY = direction.x * direction.z;
                            }

                            if (aiData.counter < buffers[entity].Length)
                            {
                                direction = buffers[entity][aiData.counter].position - translation.Value;
                                if (Magnitude(direction) < 1)
                                {
                                    aiData.counter++;
                                }
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
                                if (Magnitude(direction) > 5 && aiData.counter < buffers[entity].Length)
                                {
                                    aiData.counter++;
                                }
                                else
                                {
                                    movementData.directionX = 0;
                                    movementData.directionY = 0;
                                    movementData.directionZ = 0;
                                }
                            }
                            else
                            {
                                direction /= magnitude;
                                movementData.directionX = direction.x;
                                movementData.directionY = 0;
                                movementData.directionZ = direction.z;

                                var rotation = quaternion.LookRotation(direction, math.up());
                                rotation = math.mul(rotation, quaternion.RotateY(math.radians(direction.y)));
                                movementData.rotationY = rotation.value.y;
                            }
                        }
                    }

                    collision.Clear();
                }).Run();
        return default;
    }

    private static bool HasCollision(DynamicBuffer<CollisionEnemy> collision)
    {
        for (int i = 0; i < collision.Length; i++)
        {
            if (collision[i].Value)
                return true;
        }

        return false;
    }
}