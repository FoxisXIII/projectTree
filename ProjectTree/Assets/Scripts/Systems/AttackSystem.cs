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
    [UpdateBefore(typeof(ResolveDamageSystem))]
    public class AttackSystem : JobComponentSystem
    {
        private EntityManager manager;

        protected override void OnCreate()
        {
            manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var deltaTime = Time.DeltaTime;
            var playerBase = GameController.GetInstance().Base;

            var buffers = GetBufferFromEntity<EnemyPosition>();
            var healthGroup = GetBufferFromEntity<Damage>();
            var translations = GetComponentDataFromEntity<Translation>();

            Entities
                .ForEach(
                    (ref AIData aiData, ref Translation translation, ref MovementData movementData,
                        ref Entity entity,ref AnimationData animationData) =>
                    {
                        if (math.distance(buffers[entity][buffers[entity].Length - 1].position, translation.Value) <
                            aiData.attackDistanceBase)
                        {
                            if (aiData.attackWait >= aiData.attackRate)
                            {
                                playerBase.ReceiveDamage(aiData.attackDamage);
                                aiData.attackWait = 0;
                            }

                            aiData.attackWait += deltaTime;
                        }
                        else if (aiData.goToEntity)
                        {
                            if (aiData.canFly &&
                                math.distance(
                                    new float3(translations[aiData.entity].Value.x, 0,
                                        translations[aiData.entity].Value.z),
                                    new float3(translation.Value.x, 0, translation.Value.z)) <
                                aiData.attackDistancePlayer
                                || !aiData.canFly &&
                                math.distance(translations[aiData.entity].Value, translation.Value) <
                                aiData.attackDistancePlayer)
                                if (aiData.attackWait >= aiData.attackRate)
                                {
                                    animationData._animationType = 2;
                                    if (aiData.canFly)
                                    {
                                        //INSTANTIATE LASER
                                    }
                                    else
                                    {
                                        if (healthGroup.Exists(aiData.entity))
                                            healthGroup[aiData.entity].Add(new Damage() {Value = aiData.attackDamage});
                                    }

                                    aiData.attackWait = 0;
                                }

                            aiData.attackWait += deltaTime;
                        }
                    }).WithoutBurst().Run();
            return default;
        }
    }
}