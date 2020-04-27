using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class DieSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref HealthData hp) =>
        {
            if (hp.Value <= 0)
            {
                PostUpdateCommands.DestroyEntity(entity);
            }
        });
    }
}
