using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateBefore(typeof(AttackPositionSystem))]
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
        var enemiesInRange = GetBufferFromEntity<EnemiesInRange>();
        var turretsInRange = GetBufferFromEntity<TurretsInRange>();

        Entities.WithoutBurst().WithAll<Dead>().ForEach((Entity e) =>
        {
            if (enemiesInRange.Exists(e)) RemoveBuffer(enemiesInRange[e]);
            if (turretsInRange.Exists(e)) RemoveBuffer(turretsInRange[e]);
            if (enemies.Exists(e)) GameController.GetInstance().RemoveEnemyWave();

            ecb.DestroyEntity(e);
        }).Run();

        return default;
    }

    private void RemoveBuffer(DynamicBuffer<EnemiesInRange> buffer)
    {
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        for (int i = 0; i < buffer.Length; i++)
        {
            var enemy = buffer[i].Value;

            if (manager.Exists(enemy) && manager.HasComponent<AIData>(enemy))
            {
                var aiData = manager.GetComponentData<AIData>(enemy);
                aiData.goToEntity = false;
                aiData.entityPosition = float3.zero;
                aiData.entity = Entity.Null;
                manager.SetComponentData(enemy, aiData);
            }

            buffer.RemoveAt(i);
        }
    }

    private void RemoveBuffer(DynamicBuffer<TurretsInRange> buffer)
    {
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        for (int i = 0; i < buffer.Length; i++)
        {
            var enemy = buffer[i].Value;

            if (manager.Exists(enemy) && manager.HasComponent<AIData>(enemy))
            {
                var aiData = manager.GetComponentData<AIData>(enemy);
                aiData.goToEntity = false;
                aiData.entityPosition = float3.zero;
                aiData.entity = Entity.Null;
                manager.SetComponentData(enemy, aiData);
            }

            buffer.RemoveAt(i);
        }
    }
}