using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public class FindTargetSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone<TowerCurrentTarget>().WithAll<TowerTag>().ForEach((Entity a, ref Translation position, ref Range turretRange) => 
        {
            Entity closestTarget = Entity.Null;
            float3 turretPosition = position.Value;
            float3 closestPosition = float3.zero;
            Entities.WithAll<EnemyTag>().ForEach((Entity b, ref Translation targetPosition) => 
            {
                if (closestTarget == Entity.Null)
                {
                    closestTarget = b;
                    closestPosition = targetPosition.Value;
                }
                else
                {
                    if (math.distance(turretPosition, targetPosition.Value) < math.distance(turretPosition, closestPosition))
                    {
                        closestTarget = b;
                        closestPosition = targetPosition.Value;
                    }
                }
            });

            if (closestTarget != Entity.Null && math.distance(turretPosition, closestPosition) <= turretRange.Value)
            {
                Debug.Log(closestPosition);
                TowerCurrentTarget newTarget = new TowerCurrentTarget{target = closestTarget};
                World.EntityManager.AddComponentData(a, newTarget);
            }
        });
    }
}
