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
            var basePosition = float3.zero;
            var damage = 0;
            var playerBase = GameController.GetInstance().Base;
            var player = GameController.GetInstance().Player;
            Entities
                .ForEach(
                    (ref Translation translation, ref BaseTag tag) => { basePosition = translation.Value; }).Run();
            var playerPosition = float3.zero;
            Entities
                .ForEach(
                    (ref Translation translation, ref PlayerTag tag) => { playerPosition = translation.Value; }).Run();
            Entities
                .ForEach(
                    (ref AIData aiData, ref Translation translation, ref MovementData movementData) =>
                    {
                        Debug.Log(math.distance(basePosition, translation.Value));
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