using System.Collections;
using System.Collections.Generic;
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

        Entities.WithoutBurst().WithAll<Dead>().ForEach((Entity e) =>
        {
            if (parents.Exists(e))
            {
                var parent = parents[e].parent;
                GameController.GetInstance().InstantiateParticles("TowerDie", translations[e].Value);
                ecb.DestroyEntity(e);
                ecb.DestroyEntity(parent);
            }

            if (enemies.Exists(e))
            {
                GameController.GetInstance().InstantiateParticles("EnemyDie", translations[e].Value);
                GameController.GetInstance().RemoveEnemyWave();
                ecb.DestroyEntity(e);
            }
        }).Run();

        return default;
    }
}