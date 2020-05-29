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
                    ref Entity entity, ref DynamicBuffer<CollisionEnemy> collisionEnemies,
                    ref AnimationData animationData) =>
                {
                    aiData.state = 1;
                    if (aiData.stop)
                    {
                        animationData = ChangeAnimation(0,animationData);
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
                                animationData = ChangeAnimation(0, animationData);
                                movementData = StopMovement(movementData);
                            }
                            else
                            {
                                animationData = ChangeAnimation(1, animationData);
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
                                    animationData = ChangeAnimation(0, animationData);
                                    aiData.counter = buffers[entity].Length - 1;
                                    movementData = StopMovement(movementData);
                                }
                            }
                            else
                            {
                                animationData = ChangeAnimation(1, animationData);
                                movementData = SetRotation(movementData, direction, aiData.canFly);
                                movementData = SetDirection(movementData, direction, magnitude);
                            }
                        }
                    }
                }).Run();
        return default;
    }

    private static AnimationData ChangeAnimation(int animation, AnimationData animationData)
    {
        animationData._animationType = animation;
        return animationData;
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
        // if (fly)
        // {
        //     float3 lookAt = math.normalize(direction);
        //     var rotation = quaternion.LookRotation(lookAt, math.up());
        //     rotation = math.mul(rotation, quaternion.RotateY(math.radians(lookAt.y)));
        //     rotation = math.mul(rotation, quaternion.RotateZ(math.radians(lookAt.z)));
        //     movementData.rotation = rotation;
        // }
        // else
        // {
        //     float3 lookAt = math.normalize(direction);
        //     var rotation = quaternion.LookRotation(lookAt, math.up());
        //     rotation = math.mul(rotation, quaternion.RotateY(math.radians(lookAt.y)));
        //     rotation = math.mul(rotation, quaternion.RotateZ(math.radians(lookAt.z)));
        //     movementData.rotation = rotation;
        // }

        return movementData;
    }
}