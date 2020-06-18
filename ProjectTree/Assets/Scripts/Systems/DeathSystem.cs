using System.Collections;
using System.Collections.Generic;
using FMOD;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class DeathSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer ecb = _entityCommandBufferSystem.CreateCommandBuffer();
        var enemies = GetComponentDataFromEntity<AIData>();
        var translations = GetComponentDataFromEntity<Translation>();
        var parents = GetComponentDataFromEntity<ParentComponent>();
        var player = GetComponentDataFromEntity<PlayerTag>();

        Entities.WithoutBurst().WithAll<Dead>().ForEach((Entity e) =>
        {
            if(!player.Exists(e))
            {
                if (parents.Exists(e))
                {
                    var parent = parents[e].parent;
                    GameController.GetInstance().InstantiateParticles("TowerDie", translations[e].Value);
                    SoundManager.GetInstance().PlayOneShotSound("event:/FX/Turret/Destroy", translations[e].Value);
                    ecb.DestroyEntity(e);
                    ecb.DestroyEntity(parent);
                }

                else if (enemies.Exists(e))
                {
                    GameController.GetInstance().InstantiateParticles("EnemyDie", translations[e].Value);
                    GameController.GetInstance().RemoveEnemyWave();
                }
                else
                {
                    GameController.GetInstance().InstantiateParticles("TowerDie", translations[e].Value);
                }

                ecb.DestroyEntity(e);
            }
            else
            {
                // GameController.GetInstance().gameOver("KILLED BY X AE A12");
            }
        }).Run();

        return default;
    }
}