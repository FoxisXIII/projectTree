using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateAfter(typeof(TrapDamageSystem))]
public class TrapDeathSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float realTime=Time.DeltaTime;
        EntityCommandBuffer ecb = _entityCommandBufferSystem.CreateCommandBuffer();
        
        Entities.ForEach(( ref TrapComponent trapComponent, ref Entity entity) =>
        {
            // Debug.Log("tiempo :"+trapComponent.Recover);
            // Debug.Log("Puede matar: "+trapComponent.cankill);
            if (trapComponent.cankill)
            {
                trapComponent.Recover = trapComponent.Recover+realTime; 
                //ecb.DestroyEntity(entity);
            }

            if (trapComponent.cankill&& trapComponent.Recover>2)
            {
                trapComponent.cankill = false;
                trapComponent.Recover = 0f;
            }
        }).Run();
        
        
        return default;
    }
}
