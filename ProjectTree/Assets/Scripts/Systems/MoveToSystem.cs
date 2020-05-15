using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.AI;
using Random = UnityEngine.Random;

namespace Systems
{
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
            var player = GameController.GetInstance().Player;
            float3 playerPosition = player.transform.position;

            Entities
                .ForEach(
                    (ref AIData aiData, ref Translation translation, ref MovementData movementData,
                        ref Entity entity) =>
                    {
                        // if (math.distance(translation.Value, playerPosition) < aiData.chaseDistance)
                        // {
                        //     // if (aiData.canAttackPlayer || Random.Range(0f, 1f) < player.EnemyAttackProbability())
                        //     // {
                        //     //     if (!aiData.canAttackPlayer)
                        //     //     {
                        //     //         player.AddEnemy(entity);
                        //     //         aiData.canAttackPlayer = true;
                        //     //         Debug.Log(player.EnemyAttackProbability());
                        //     //     }
                        //     //
                        //     //     var direction = playerPosition - translation.Value;
                        //     //     var magnitude = Magnitude(direction);
                        //     //     if (magnitude < 1)
                        //     //     {
                        //     //         movementData.directionX = 0;
                        //     //         movementData.directionY = 0;
                        //     //         movementData.directionZ = 0;
                        //     //     }
                        //     //     else
                        //     //     {
                        //     //         direction /= magnitude;
                        //     //         movementData.directionX = direction.x;
                        //     //         movementData.directionY = 0;
                        //     //         movementData.directionZ = direction.z;
                        //     //     }
                        //     // }
                        // }
                        // else 
                        if (aiData.goToEntity)
                        {
                            Debug.Log("TO ENTITY");
                            var direction = aiData.entity - translation.Value;
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
                            }
                        }
                    }).Run();
            return default;
        }
    }
}