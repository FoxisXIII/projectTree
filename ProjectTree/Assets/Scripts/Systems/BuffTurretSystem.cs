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
                (ref BuffTurretData buffTurretData, ref Translation translation, ref Entity entity,
                    ref TurretFMODPaths paths) =>
                {
                    if (math.distance(playerPosition, translation.Value) <= buffTurretData.range)
                    {
                        if (buffTurretData.health != 0 && buffTurretData.buffTimer >= buffTurretData.buffRate)
                        {
                            if (healthGroup.HasComponent(entityPlayer))
                            {
                                Debug.Log("Heal");
                                var healthData = healthGroup[entityPlayer];
                                healthData.value = math.min(healthData.value + buffTurretData.health,
                                    healthData.maxValue);
                                healthGroup[entityPlayer] = healthData;
                                SoundManager.GetInstance().PlayOneShotSound(paths.HealPath.ToString(), playerPosition);
                            }

                            buffTurretData.buffTimer = 0;
                        }
                        else if (buffTurretData.buffTimer < buffTurretData.buffRate)
                        {
                            buffTurretData.buffTimer += deltaTime;
                            Debug.Log(buffTurretData.buffTimer);
                        }
                        else if (buffTurretData.resources != 0 &&
                                 buffTurretData.buffTimer >= buffTurretData.buffRate)
                        {
                            player.IncreaseResources(buffTurretData.resources);
                            SoundManager.GetInstance().PlayOneShotSound(paths.BuffPath.ToString(), playerPosition);
                            buffTurretData.buffTimer = 0;
                        }
                        else if (buffTurretData.buffTimer < buffTurretData.buffRate)
                            buffTurretData.buffTimer += deltaTime;
                        else if (!player.hasBuff || player.hasBuff && !player.buffEntity.Equals(entity) ||
                                 player.startBuffTimer)
                        {
                            if (buffTurretData.attack != 0)
                                player.IncreaseAttack(buffTurretData.attack, buffTurretData.buffDisapear);
                            else if (buffTurretData.speed != 0)
                                player.IncreaseSpeed(buffTurretData.speed, buffTurretData.buffDisapear);
                            else if (buffTurretData.shotgun != 0)
                                player.Shotgun(buffTurretData.shotgun, buffTurretData.buffDisapear);

                            SoundManager.GetInstance().PlayOneShotSound(paths.BuffPath.ToString(), playerPosition);
                            player.buffEntity = entity;
                            player.hasBuff = true;
                        }
                    }
                    else
                    {
                        if (player.hasBuff)
                        {
                            player.startBuffTimer = true;
                        }
                    }
                }).WithoutBurst().Run();
        return default;
    }
}