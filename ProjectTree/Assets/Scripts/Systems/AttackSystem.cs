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
    [AlwaysSynchronizeSystem]
    public class AttackSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            EntityCommandBuffer ecb = _entityCommandBufferSystem.CreateCommandBuffer();

            var deltaTime = Time.DeltaTime;
            var playerBase = GameController.GetInstance().Base;
            var player = GameController.GetInstance().Player;

            var buffers = GetBufferFromEntity<EnemyPosition>();
            var healthGroup = GetComponentDataFromEntity<HealthData>();

            Entities
                .ForEach(
                    (ref AIData aiData, ref Translation translation, ref MovementData movementData,
                        ref Entity entity) =>
                    {
                        if (math.distance(buffers[entity][buffers[entity].Length - 1].position, translation.Value) <
                            aiData.attackDistanceBase)
                        {
                            if (aiData.attackWait >= aiData.attackRate)
                            {
                                // playerBase.ReceiveDamage(aiData.attackDamage);
                                aiData.attackWait = 0;
                            }

                            aiData.attackWait += deltaTime;
                        }
                        else if (math.distance(player.transform.position, translation.Value) <
                                 aiData.attackDistancePlayer)
                        {
                            if (aiData.attackWait >= aiData.attackRate)
                            {
                                player.ReceiveDamage(aiData.attackDamage);
                                aiData.attackWait = 0;
                            }

                            aiData.attackWait += deltaTime;
                        }
                        // else if (aiData.turret != Entity.Null)
                        // {
                        //     if (aiData.attackWait >= aiData.attackRate)
                        //     {
                        //         var healthData = healthGroup[aiData.turret];
                        //         healthData.Value -= aiData.attackDamage;
                        //         if (healthData.Value <= 0)
                        //         {
                        //             ecb.DestroyEntity(aiData.turret);
                        //         }
                        //
                        //         aiData.attackWait = 0;
                        //     }
                        //
                        //     aiData.attackWait += deltaTime;
                        // }
                    }).WithoutBurst().Run();
            return default;
        }
    }
}