using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
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

        Entities.WithoutBurst().WithAll<Dead>().ForEach((Entity e) =>
        {
            ecb.DestroyEntity(e);
            GameController.GetInstance().RemoveEnemyWave();
        }).Run();

        return default;
    }
}
