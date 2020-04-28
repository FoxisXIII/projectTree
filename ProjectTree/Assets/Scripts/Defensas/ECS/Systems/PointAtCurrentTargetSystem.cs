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
            (Entity e, ref TowerCurrentTarget target, ref Rotation rotation, ref Translation position, ref RangeComponent turretRange) =>
            {
                if (World.EntityManager.Exists(target.target))
                {
                    Debug.Log("hey");
                    Entity enemy = target.target;
                    Translation enemyPos = World.EntityManager.GetComponentData<Translation>(enemy);
                    if (math.distance(position.Value, enemyPos.Value) <= turretRange.Value)
                    {
                        float3 lookAt = math.normalize(position.Value - enemyPos.Value);
                        rotation.Value = quaternion.Euler(lookAt);
                    }
                }
                else
                {
                    PostUpdateCommands.RemoveComponent(e, typeof(TowerCurrentTarget));
                }
            });
    }
}
