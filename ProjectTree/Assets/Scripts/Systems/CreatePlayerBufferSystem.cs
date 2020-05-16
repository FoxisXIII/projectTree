using System;
using System.Collections.Generic;
using Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.AI;
using Random = UnityEngine.Random;

public class CreatePlayerSystemBuffer : ComponentSystem
{
    protected override void OnCreate()
    {
        base.OnCreate();

    }

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref PlayerTag playerTag) =>
        {
            if (!playerTag.init)
            {
                World.DefaultGameObjectInjectionWorld.EntityManager.AddBuffer<EnemiesInRange>(entity);
                playerTag.init = true;
            }
        });
    }
}