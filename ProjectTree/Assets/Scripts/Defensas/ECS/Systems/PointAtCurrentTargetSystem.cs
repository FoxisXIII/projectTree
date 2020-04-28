using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PointAtCurrentTargetSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityManager entityManager = World.EntityManager;
        
        Entities.WithoutBurst().WithAll<TowerTag, Rotation, Translation>().ForEach(
            (Entity e, ref TowerCurrentTarget target, ref Rotation rotation, ref Translation position) =>
            {
                if (target.target != Entity.Null)
                {
                    Entity enemy = target.target;
                    Translation enemyPos = entityManager.GetComponentData<Translation>(enemy);

                    float3 lookAt = position.Value - enemyPos.Value;
                    rotation.Value = quaternion.LookRotation(lookAt, math.up());
                }
            }).Run();

        return inputDeps;
    }
}
