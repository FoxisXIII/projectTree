using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(MoveForwardSystem))]
public class TimeToLiveSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem buffer;

    protected override void OnCreate()
    {
        buffer = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    struct DestroyJob : IJobForEachWithEntity<TimeToLive>
    {
        public float deltaTime;
        public EntityCommandBuffer.Concurrent commands;
        
        public void Execute(Entity entity, int index, ref TimeToLive time)
        {
            time.Value -= deltaTime;
            if (time.Value<=0) 
                commands.DestroyEntity(index, entity);
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new DestroyJob
        {
            commands = buffer.CreateCommandBuffer().ToConcurrent(),
            deltaTime = Time.DeltaTime
        };
        
        var handle = job.Schedule(this, inputDeps);
        buffer.AddJobHandleForProducer(handle);

        return handle;
    }
}
