using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FindTargetSystem))]
[BurstCompile]
public class PointAtCurrentTargetSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<TowerTag>().ForEach(
            (Entity e, ref TowerCurrentTarget target, ref Rotation rotation, ref Translation position,
                ref RangeComponent turretRange) =>
            {
                if (World.EntityManager.Exists(target.target)&&World.EntityManager.HasComponent<Translation>(target.target))
                {
                    Entity enemy = target.target;
                    Translation enemyPos = World.EntityManager.GetComponentData<Translation>(enemy);
                    if (math.distance(position.Value, enemyPos.Value) <= turretRange.Value)
                    {
                        enemyPos.Value.y += 1f;
                        float3 lookAt = math.normalize(enemyPos.Value - position.Value);
                        rotation.Value = quaternion.LookRotation(lookAt, math.up());
                        rotation.Value = math.mul(rotation.Value, quaternion.RotateY(math.radians(lookAt.y)));
                        rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(lookAt.z)));
                    }
                    else
                    {
                        UnbindTarget(e);
                    }
                }
                else
                {
                    UnbindTarget(e);
                }
            });
    }

    void UnbindTarget(Entity e)
    {
        PostUpdateCommands.RemoveComponent(e, typeof(TowerCurrentTarget));
    }
}