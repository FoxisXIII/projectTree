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

[UpdateAfter(typeof(ResolveDamageSystem))]
public class CreatePlayerSystemBuffer : ComponentSystem
{
    protected override void OnUpdate()
    {
        var healthGroup = GetComponentDataFromEntity<HealthData>();

        Entities.ForEach((Entity entity, ref PlayerTag playerTag) =>
        {
            if (GameController.GetInstance().Player != null)
                GameController.GetInstance().Player.life = healthGroup[entity].Value;
        });
    }
}