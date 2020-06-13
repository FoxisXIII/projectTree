using System;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.AI;
using Random = UnityEngine.Random;

[UpdateBefore(typeof(DamageCollisionSystem))]
public class BuffTurretSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var deltaTime = Time.DeltaTime;
        var player = GameController.GetInstance().Player;
        var playerPosition = player.transform.position;

        Entity entityPlayer = default;

        Entities.WithAll<PlayerTag>().ForEach((Entity entity) => { entityPlayer = entity; }).Run();

        var healthGroup = GetComponentDataFromEntity<HealthData>();

        Entities
            .ForEach(
                (ref BuffTurretData buffTurretData, ref Translation translation, ref Entity entity, ref TurretFMODPaths paths) =>
                {
                    if (math.distance(playerPosition, translation.Value) <= buffTurretData.range)
                    {
                        if (buffTurretData.health != 0 && buffTurretData.buffTimer >= buffTurretData.buffRate)
                        {
                            if (healthGroup.HasComponent(entityPlayer))
                            {
                                var healthData = healthGroup[entityPlayer];
                                healthData.value = math.min(healthData.value + buffTurretData.health,
                                    healthData.maxValue);
                                healthGroup[entityPlayer] = healthData;
                                SoundManager.GetInstance().PlayOneShotSound(paths.HealPath.ToString(), playerPosition);
                            }

                            buffTurretData.buffTimer = 0;
                        }
                        else if (buffTurretData.buffTimer < buffTurretData.buffRate)
                            buffTurretData.buffTimer += deltaTime;
                        else if (buffTurretData.resources != 0 &&
                                 buffTurretData.buffTimer >= buffTurretData.buffRate)
                        {
                            player.IncreaseResources(buffTurretData.resources);
                            SoundManager.GetInstance().PlayOneShotSound(paths.BuffPath.ToString(), playerPosition);
                            buffTurretData.buffTimer = 0;
                        }
                        else if (buffTurretData.buffTimer < buffTurretData.buffRate)
                            buffTurretData.buffTimer += deltaTime;
                        else if (!player.hasBuff || player.hasBuff && !player.buffEntity.Equals(entity))
                        {
                            if (buffTurretData.attack != 0)
                                player.IncreaseAttack(buffTurretData.attack);
                            else if (buffTurretData.speed != 0)
                                player.IncreaseSpeed(buffTurretData.speed);
                            else if (buffTurretData.shotgun != 0)
                                player.Shotgun(buffTurretData.shotgun);

                            SoundManager.GetInstance().PlayOneShotSound(paths.BuffPath.ToString(), playerPosition);
                            player.buffEntity = entity;
                            player.hasBuff = true;
                        }
                    }
                    else
                    {
                        if (player.hasBuff)
                        {
                            if (player.buffEntity.Equals(entity) &&
                                buffTurretData.buffTimer >= buffTurretData.buffDisapear)
                            {
                                buffTurretData.buffTimer = 0;
                                player.hasBuff = false;
                                player.StopBuffs();
                            }
                            else
                            {
                                buffTurretData.buffTimer += deltaTime;
                            }
                        }
                    }
                }).WithoutBurst().Run();
        return default;
    }
}