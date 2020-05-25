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
            var healthGroup = GetBufferFromEntity<Damage>();
            var translations = GetComponentDataFromEntity<Translation>();

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
                                playerBase.ReceiveDamage(aiData.attackDamage);
                                aiData.attackWait = 0;
                            }

                            aiData.attackWait += deltaTime;
                        }
                        else if (aiData.goToEntity &&
                                 math.distance(translations[aiData.entity].Value, translation.Value) <
                                 aiData.attackDistancePlayer)
                        {
                            if (aiData.attackWait >= aiData.attackRate)
                            {
                                if (healthGroup.Exists(aiData.entity))
                                    healthGroup[aiData.entity].Add(new Damage() {Value = aiData.attackDamage});
                                aiData.attackWait = 0;
                            }

                            aiData.attackWait += deltaTime;
                        }
                    }).WithoutBurst().Run();
            return default;
        }
    }
}