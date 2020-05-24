using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class TrapDeathSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        
        EntityCommandBuffer ecb = _entityCommandBufferSystem.CreateCommandBuffer();
        
        Entities.ForEach((TrapComponent trapComponent,Entity entity) =>
        {
            if (trapComponent.Deaths>10)
            {
                ecb.DestroyEntity(entity);
            }
        }).Run();
        
        
        return default;
    }
}
