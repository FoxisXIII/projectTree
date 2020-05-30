using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;


namespace Unity.Transforms
{
    public class MoveForwardSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<MovesForwardComponent>().ForEach(
                (Entity a, ref Translation position, ref Rotation rotation, ref MoveSpeed speed) =>
                {
                    float3 pos = position.Value;
                    pos += Time.DeltaTime * speed.Value * math.forward(rotation.Value);
                    position.Value = pos;
                });
        }
    }
}

