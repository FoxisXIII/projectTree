using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class DebugTargetSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<TowerTag, TowerCurrentTarget>().ForEach((Entity e, ref Translation position, ref TowerCurrentTarget target) =>
        {
            Translation targetTranslation = World.EntityManager.GetComponentData<Translation>(e);
            Debug.Log(targetTranslation);
        });
    }

    
}
