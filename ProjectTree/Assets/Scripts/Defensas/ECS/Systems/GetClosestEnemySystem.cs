using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class GetClosestEnemySystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        Entities.WithAll<TowerCurrentTarget>().ForEach((Entity e, ref TowerCurrentTarget tct, ref Translation position, ref RangeComponent range) =>
        {
            
            if (tct.target == Entity.Null || !manager.Exists(tct.target) || math.distance(position.Value, manager.GetComponentData<Translation>(tct.target).Value) > range.Value 
                || manager.HasComponent(tct.target, typeof(Dead)))
            {
                PostUpdateCommands.RemoveComponent(e, typeof(TowerCurrentTarget));
            }
        });

        Entities.WithNone<TowerCurrentTarget>().WithAll<EnemiesInRange, TowerTag>().ForEach((Entity e, ref Translation position, DynamicBuffer<EnemiesInRange> inRange) =>
        {
            var closestEnemy = Entity.Null;
            float3 turretPos = position.Value;
            float3 closestPos = float3.zero;

            foreach (var enemy in inRange)
            {
                if(manager.Exists(enemy.Value))
                {
                    float3 enemyPos = manager.GetComponentData<Translation>(enemy.Value).Value;
                    if (closestEnemy == Entity.Null)
                    {
                        closestEnemy = enemy.Value;
                        closestPos = enemyPos;
                    }
                    else
                    {
                        if (math.distance(turretPos, enemyPos) < math.distance(turretPos, closestPos))
                        {
                            closestEnemy = enemy.Value;
                            closestPos = manager.GetComponentData<Translation>(enemy.Value).Value;
                        }
                    }
                }
            }
            
            if (closestEnemy != Entity.Null && manager.Exists(closestEnemy))
            {
                PostUpdateCommands.AddComponent(e, new TowerCurrentTarget{target = closestEnemy});
            }
        });
    }
}
