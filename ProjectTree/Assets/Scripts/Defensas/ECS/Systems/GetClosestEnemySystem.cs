using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

public class GetClosestEnemySystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entities.WithNone<TowerCurrentTarget>().WithAll<EnemiesInRange, TowerTag>().ForEach((Entity e, ref Translation position) =>
        {
            var closestEnemy = Entity.Null;
            float3 turretPos = position.Value;
            float3 closestPos = float3.zero;

            Entities.ForEach((DynamicBuffer<EnemiesInRange> inRange) =>
            {
                foreach (var enemy in inRange)
                {
                    float3 enemyPos = manager.GetComponentData<Translation>(enemy.enemies).Value;
                    if (closestEnemy == Entity.Null)
                    {
                        closestEnemy = enemy.enemies;
                        closestPos = enemyPos;
                    }
                    else
                    {
                        if (math.distance(turretPos, enemyPos) < math.distance(turretPos, closestPos))
                        {
                            closestEnemy = enemy.enemies;
                            closestPos = manager.GetComponentData<Translation>(enemy.enemies).Value;
                        }
                    }
                }
            });
            if (closestEnemy != Entity.Null)
            {
                PostUpdateCommands.AddComponent(e, new TowerCurrentTarget{target = closestEnemy});
            }
        });
    }
}
