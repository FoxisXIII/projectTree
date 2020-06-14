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
            var playerBase = GameController.GetInstance().Base;
            var deltaTime = Time.DeltaTime;
            var buffers = GetBufferFromEntity<EnemyPosition>();
            var healthGroup = GetBufferFromEntity<Damage>();
            var translations = GetComponentDataFromEntity<Translation>();

            Entities
                .ForEach(
                    (ref AIData aiData, ref Translation translation, ref MovementData movementData,
                        ref Entity entity, ref EnemyFMODPaths paths) =>
                    {
                        if (math.distance(buffers[entity][buffers[entity].Length - 1].position, translation.Value) <
                            aiData.attackDistanceBase)
                        {
                            if (aiData.attackWait >= aiData.attackRate)
                            {
                                SoundManager.GetInstance().PlayOneShotSound(paths.AttackBasePath.ToString(), translation.Value);
                                playerBase.ReceiveDamage(aiData.attackDamage);
                                // aiData.shot = true;
                                aiData.attackWait = 0;
                            }

                            aiData.attackWait += deltaTime;
                        }
                        else if (aiData.goToEntity)
                        {
                            if (aiData.goToEntity && translations.Exists(aiData.entity))
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
                                        if (healthGroup.Exists(aiData.entity))
                                        {
                                            healthGroup[aiData.entity].Add(new Damage() {Value = aiData.attackDamage});
                                            aiData.shot = true;
                                            SoundManager.GetInstance()
                                                .PlayOneShotSound(paths.AttackPlayerPath.ToString(), translation.Value);
                                            if (GameController.GetInstance().Player != null &&
                                                manager.HasComponent(aiData.entity, typeof(PlayerTag)))
                                                GameController.GetInstance().Player.ReceiveDamage();
                                        }

                                        aiData.attackWait = 0;
                                    }
                            }
                            else
                            {
                                aiData.goToEntity = false;
                                aiData.entity = Entity.Null;
                                aiData.stop = false;
                            }

                            aiData.attackWait += deltaTime;
                        }
                    }).WithoutBurst().Run();
            return default;
        }
    }
}