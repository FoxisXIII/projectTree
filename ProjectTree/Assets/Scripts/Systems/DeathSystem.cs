using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
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
        var collisions = GetBufferFromEntity<CollisionEnemy>();
        var parents = GetComponentDataFromEntity<ParentComponent>();

        Entities.WithoutBurst().WithAll<Dead>().ForEach((Entity e) =>
        {
            if (parents.Exists(e))
            {
                var parent = parents[e].parent;
                ecb.DestroyEntity(e);
                ecb.DestroyEntity(parent);
            }

            if (enemies.Exists(e))
            {
                ResetCollisionBuffer(collisions[e]);
                GameController.GetInstance().RemoveEnemyWave();
            }
            ecb.DestroyEntity(e);
        }).Run();

        return default;
    }

    private void ResetCollisionBuffer(DynamicBuffer<CollisionEnemy> buffer)
    {
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        for (int i = 0; i < buffer.Length; i++)
        {
            var enemy = buffer[i].Entity;

            if (manager.Exists(enemy) && manager.HasComponent<AIData>(enemy))
            {
                var aiData = manager.GetComponentData<AIData>(enemy);
                aiData.stop = false;
                manager.SetComponentData(enemy, aiData);
            }
        }

        buffer.Clear();
    }
}