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
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var deltaTime = Time.DeltaTime;
            var playerBase = GameController.GetInstance().Base;
            var player = GameController.GetInstance().Player;
            var playerPosition = player.transform.position;
            Entities
                .ForEach(
                    (ref AIData aiData, ref Translation translation, ref MovementData movementData) =>
                    {
                        if (math.distance(aiData.finalPosition, translation.Value) < aiData.attackDistanceBase)
                        {
                            if (aiData.attackWait >= aiData.attackRate)
                            {
                                playerBase.ReceiveDamage(aiData.attackDamage);
                                aiData.attackWait = 0;
                            }

                            aiData.attackWait += deltaTime;
                        }
                        else if (math.distance(playerPosition, translation.Value) < aiData.attackDistancePlayer)
                        {
                            if (aiData.attackWait >= aiData.attackRate)
                            {
                                player.ReceiveDamage(aiData.attackDamage);
                                aiData.attackWait = 0;
                            }

                            aiData.attackWait += deltaTime;
                        }
                    }).WithoutBurst().Run();
            return default;
        }
    }
}