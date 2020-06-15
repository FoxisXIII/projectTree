using System;
using System.Collections.Generic;
using Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
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
        var player = GetComponentDataFromEntity<PlayerTag>();
        var translations = GetComponentDataFromEntity<Translation>();


        Entities
            .ForEach(
                (ref AIData aiData, ref Translation translation, ref MovementData movementData,
                    ref Entity entity, ref DynamicBuffer<CollisionEnemy> collisionEnemies,
                    ref AnimationData animationData, ref PhysicsGravityFactor gravityFactor) =>
                {
                    aiData.state = 1;
                    animationData._animationType = 1;
                    animationData.rotationSpeed = movementData.speed / 20;
                    if (aiData.stop)
                    {
                        movementData = StopMovement(movementData);
                        animationData._animationType = 0;
                        gravityFactor.Value = 0;

                        var direction = float3.zero;
                        if (aiData.counter == 0)
                        {
                            if (aiData.hordeMove)
                            {
                                aiData.stop = false;
                                aiData.counter++;
                            }
                            else
                            {
                                var position = buffers[entity][aiData.counter].position;
                                if (aiData.canFly)
                                    position.y += aiData.yOffset;
                                direction = position - translation.Value;

                                var magnitude = Magnitude(direction);
                                if (magnitude > 1 && !aiData.stopByCollision)
                                {
                                    aiData.stop = false;
                                }
                            }
                        }

                        if (aiData.goToEntity && translations.Exists(aiData.entity))
                        {
                            if (aiData.goToEntity)
                                direction = translations[aiData.entity].Value - translation.Value;
                            else
                                direction = buffers[entity][aiData.counter].position - translation.Value;

                            if (!aiData.canFly)
                                direction.y = 0;

                            movementData = SetRotation(movementData, direction, aiData.canFly);

                            if (aiData.goToEntity)
                            {
                                direction = translations[aiData.entity].Value - translation.Value;
                                var magnitude = Magnitude(direction);
                                if (magnitude > aiData.attackDistancePlayer)
                                {
                                    aiData.stop = false;
                                    aiData.state = 0;
                                    if (!player.Exists(aiData.entity))
                                    {
                                        aiData.goToEntity = false;
                                        aiData.entity = Entity.Null;
                                    }
                                }
                            }
                        }
                        else if (aiData.goToEntity)
                        {
                            aiData.goToEntity = false;
                            aiData.entity = Entity.Null;
                            aiData.stop = false;
                        }
                    }
                    else
                    {
                        if (aiData.goToEntity)
                        {
                            if (translations.Exists(aiData.entity))
                            {
                                float3 direction;

                                if (aiData.canFly)
                                    direction = translations[aiData.entity].Value - translation.Value;
                                else
                                {
                                    var destination = translations[aiData.entity].Value;
                                    destination.y += .5f;
                                    direction = destination - translation.Value;
                                }

                                var magnitude = Magnitude(direction);
                                if (magnitude < aiData.attackDistancePlayer)
                                {
                                    aiData.stop = true;
                                }
                                else
                                {
                                    if (aiData.canFly)
                                        direction.y += aiData.yOffset;
                                    else
                                        gravityFactor.Value = 10;

                                    movementData = SetDirection(movementData, direction, magnitude);
                                }

                                movementData = SetRotation(movementData,
                                    translations[aiData.entity].Value - translation.Value, aiData.canFly);
                                movementData.rotation.value.x = 0;
                                movementData.rotation.value.z = 0;

                                if (aiData.counter < buffers[entity].Length)
                                {
                                    direction = buffers[entity][aiData.counter].position - translation.Value;
                                    if (Magnitude(direction) < 1) aiData.counter++;
                                }
                            }
                            else
                            {
                                aiData.goToEntity = false;
                                aiData.entity = Entity.Null;
                            }
                        }
                        else
                        {
                            var position = buffers[entity][aiData.counter].position;
                            position.y += aiData.yOffset;
                            var direction = position - translation.Value;
                            var directionY = direction.y;
                            direction.y = 0;
                            var magnitude = Magnitude(direction);
                            if (magnitude < 1)
                            {
                                direction = translation.Value - buffers[entity][buffers[entity].Length - 1].position;
                                if (Magnitude(direction) > .5f && aiData.counter < buffers[entity].Length - 1)
                                {
                                    if (aiData.horde && !aiData.hordeMove)
                                        aiData.stop = true;
                                    else
                                        aiData.counter++;
                                }
                                else
                                {
                                    aiData.counter = buffers[entity].Length - 1;
                                    aiData.stop = true;
                                }
                            }
                            else
                            {
                                direction.y = directionY;
                                movementData = SetRotation(movementData, direction, aiData.canFly);
                                movementData = SetDirection(movementData, direction, magnitude);
                                if (!aiData.canFly)
                                    gravityFactor.Value = 10;
                            }
                        }
                    }

                    aiData.stopByCollision = false;
                }).Run();
        return default;
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
            float3 lookAt = math.normalize(direction);
            var rotation = quaternion.LookRotation(lookAt, math.up());
            rotation = math.mul(rotation, quaternion.RotateY(math.radians(lookAt.y)));
            rotation = math.mul(rotation, quaternion.RotateX(math.radians(lookAt.x)));
            movementData.rotation = rotation;
        }
        else
        {
            float3 lookAt = math.normalize(direction);
            var rotation = quaternion.LookRotation(lookAt, math.up());
            rotation = math.mul(rotation, quaternion.RotateY(math.radians(lookAt.y)));
            rotation = math.mul(rotation, quaternion.RotateX(math.radians(lookAt.x)));
            movementData.rotation = rotation;
        }

        return movementData;
    }
}