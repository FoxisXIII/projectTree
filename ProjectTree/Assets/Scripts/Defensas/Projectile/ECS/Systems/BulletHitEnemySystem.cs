using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BulletHitEnemySystem : ComponentSystem
{
    private EntityQuery bullets;
    private EntityQuery enemies;
    
    protected override void OnCreate()
    {
        bullets = GetEntityQuery(
            ComponentType.ReadOnly<MoveForward>(),
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadWrite<TimeToLive>());

        enemies = GetEntityQuery(
            ComponentType.ReadOnly<HealthComponent>(),
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<Collider>());
    }

    protected override void OnUpdate()
    {
        Entities.With(bullets).ForEach((Entity bullet, ref Translation translation, ref TimeToLive ttl) =>
        {
            float3 bulletPos = translation.Value;
            bool collision = false;
            
            Entities.With(enemies).ForEach((Entity enemy, ref Collider enemyCollider, ref Translation enemyPosition) =>
            {
                float distSqr = math.lengthsq(enemyPosition.Value - bulletPos);
                float contactSqr = enemyCollider.bounds.SqrDistance(bulletPos);
                contactSqr *= contactSqr;

                if (distSqr < contactSqr)
                {
                    collision = true;
                }
            });

            if (collision)
            {
                ttl.Value = 0;
                collision = false;
            }
        });
    }
    
}
