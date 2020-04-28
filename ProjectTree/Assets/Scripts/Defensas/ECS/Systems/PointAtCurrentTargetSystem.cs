using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FindTargetSystem))]
public class PointAtCurrentTargetSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<TowerTag>().ForEach(
            (Entity e, ref TowerCurrentTarget target, ref Rotation rotation, ref Translation position) =>
            {
                Entity enemy = target.target;
                Translation enemyPos = World.EntityManager.GetComponentData<Translation>(enemy);
                float3 lookAt = math.normalize(position.Value - enemyPos.Value);
                rotation.Value = quaternion.Euler(lookAt);
            });
    }
}
