using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MoveBulletSys : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithoutBurst().WithAll<MovesForwardComponent>().ForEach((Entity e, ref Translation position, in Rotation rotation, in SpeedComponent speed) =>
        {
            position.Value += Time.DeltaTime * speed.Value * math.forward(rotation.Value);
        }).Run();
        return inputDeps;
    }
}
