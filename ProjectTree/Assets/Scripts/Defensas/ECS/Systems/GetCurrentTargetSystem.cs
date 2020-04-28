using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public class GetCurrentTargetSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithoutBurst().WithAll<TowerTag>().ForEach(
            (Entity e, ref TowerCurrentTarget currentTarget, ref DynamicBuffer<TowerInRangeTargets> inRangeTargets, ref Translation position) =>
            {
                currentTarget.target = inRangeTargets.First().target;
            }).Run();
        return default;
    }
}
