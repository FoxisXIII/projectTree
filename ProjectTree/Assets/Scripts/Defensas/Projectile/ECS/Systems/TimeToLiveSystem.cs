using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(MoveForwardSystem))]
public class TimeToLiveSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<TimeToLive>().ForEach((Entity a, ref TimeToLive ttl) =>
        {
            
            ttl.Value -= Time.DeltaTime;
            if (ttl.Value <= 0)
            {
                PostUpdateCommands.DestroyEntity(a);
            }
        });
    }
}
