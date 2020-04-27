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
        private static void AttackSequence()
        {
            throw new NotImplementedException();
        }
    
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var basePosition = float3.zero;
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
                        if (math.distance(basePosition, translation.Value) < aiData.attackDistance)
                        {
                            AttackSequence();
                        }
                        else
                        {
                        }

                        if (math.distance(playerPosition, translation.Value) < aiData.attackDistance)
                        {
                            AttackSequence();
                        }
                        else
                        {
                        }
                    }).Run();
            return default;
        }
    }
}