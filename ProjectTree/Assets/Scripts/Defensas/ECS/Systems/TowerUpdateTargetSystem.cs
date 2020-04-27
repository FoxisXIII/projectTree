using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class TowerUpdateTargetSystem : ComponentSystem
{
    private EntityQuery towers;
    private EntityQuery enemies;

    protected override void OnCreate()
    {
        towers = GetEntityQuery(ComponentType.ReadOnly<AimComponent>(),
            ComponentType.ReadOnly<TurretTarget>(),
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<TowerTag>());

        enemies = GetEntityQuery(ComponentType.ReadOnly<HealthComponent>(),
            ComponentType.ReadOnly<Translation>());
    }

    protected override void OnUpdate()
    {
        Entities.With(towers).ForEach((Entity t, ref Translation towerPosition, ref AimComponent aimComponent) =>
        {
            DynamicBuffer<TurretTarget> towerTargets = EntityManager.GetBuffer<TurretTarget>(t);
            towerTargets.Clear();
            float3 towerPos = towerPosition.Value;
            float towerRadius = aimComponent.Range;
            
            Entities.With(enemies).ForEach((Entity m, ref Translation monsterPosition) =>
            {
                float distSqr = math.lengthsq(monsterPosition.Value - towerPos);
                float contactSquared = towerRadius + monsterPosition.Value.x;
                contactSquared += contactSquared;

                if (distSqr < contactSquared)
                {
                    towerTargets.Add(new TurretTarget {target = m});
                }
            });
        });
    }
}
