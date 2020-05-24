using System.Collections.Generic;
using Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

[UpdateBefore(typeof(MoveToSystem))]
public class MovePlayerSystem : ComponentSystem
{
    private List<float3> list;

    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerTag>().ForEach((ref Translation translation) =>
        {
            translation.Value = GameController.GetInstance().Player.transform.position;
        });
    }
}