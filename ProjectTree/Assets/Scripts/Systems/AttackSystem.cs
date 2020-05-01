using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.WSA;
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
            var basePosition = playerBase.transform.position;
            Entities
                .ForEach(
                    (ref AIData aiData, ref Translation translation, ref MovementData movementData) =>
                    {
                        var distance = math.distance(playerPosition, translation.Value);
                        Debug.Log(distance);
                        if (math.distance(basePosition, translation.Value) < aiData.attackDistance)
                        {
                            playerBase.ReceiveDamage(aiData.attackDamage);
                        }
                        else if (math.distance(playerPosition, translation.Value) < aiData.attackDistance)
                        {
                            player.ReceiveDamage(aiData.attackDamage);
                        }
                    }).WithoutBurst().Run();
            return default;
        }
    }
}