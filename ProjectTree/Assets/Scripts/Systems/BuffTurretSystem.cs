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

public class BuffTurretSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var deltaTime = Time.DeltaTime;
        var player = GameController.GetInstance().Player;
        var playerPosition = player.transform.position;
        Entities
            .ForEach(
                (ref BuffTurretData buffTurretData, ref Translation translation) =>
                {
                    if (math.distance(playerPosition, translation.Value) <= buffTurretData.range)
                    {
                        if (buffTurretData.health != 0 && buffTurretData.buffTimer >= buffTurretData.buffRate)
                        {
                            player.RecoverHealth(buffTurretData.health);
                            buffTurretData.buffTimer = 0;
                        }
                        else if (buffTurretData.buffTimer < buffTurretData.buffRate)
                            buffTurretData.buffTimer += deltaTime;
                        else if (buffTurretData.resources != 0 && buffTurretData.buffTimer >= buffTurretData.buffRate)
                        {
                            player.IncreaseResources(buffTurretData.resources);
                            buffTurretData.buffTimer = 0;
                        }
                        else if (buffTurretData.buffTimer < buffTurretData.buffRate)
                            buffTurretData.buffTimer += deltaTime;
                        else if (buffTurretData.attack != 0 && !player.hasBuff)
                            player.IncreaseAttack(buffTurretData.attack);
                        else if (buffTurretData.speed != 0 && !player.hasBuff)
                            player.IncreaseSpeed(buffTurretData.speed);
                        else if (buffTurretData.shotgun != 0 && !player.hasBuff)
                            player.Shotgun(buffTurretData.shotgun);

                        player.hasBuff = true;
                    }
                    else
                    {
                        if (player.hasBuff)
                        {
                            if (buffTurretData.buffTimer >= buffTurretData.buffDisapear)
                            {
                                buffTurretData.buffTimer = 0;
                                player.StopBuffs();
                                player.hasBuff = false;
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