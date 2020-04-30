using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Serialization;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(PointAtCurrentTargetSystem))]
[BurstCompile]
public class BulletShootingSystem : ComponentSystem
{
    float timer;

    private static float3 Direction(float3 v1, float3 v2)
    {
        var direction = v2 - v1;
        var magnitude = math.sqrt(math.pow(direction.x, 2) + math.pow(direction.y, 2) + math.pow(direction.z, 2));
        return direction / magnitude;
    }

    protected override void OnUpdate()
    {
        Entities.WithAll<TowerTag, TowerCurrentTarget>().ForEach((Entity entity, ref AttackSpeedComponent attackSpeed,
            ref BulletPrefabComponent bullet, ref Translation position, ref Rotation rotation,
            ref TowerCurrentTarget tct
        ) =>
        {
            timer -= Time.DeltaTime;
            //Debug.Log(timer);
            if (timer <= 0)
            {
                //Debug.Log("shoot");
                Entity bulletEntity = EntityManager.Instantiate(bullet.prefab);
                float3 where = position.Value;
                where.z += 10f;
                //Debug.Log(rotation.Value);
                var direction = Direction(position.Value,
                    EntityManager.GetComponentData<Translation>(tct.target).Value);
                EntityManager.SetComponentData(bulletEntity, new Translation {Value = position.Value});

                var movementData = EntityManager.GetComponentData<MovementData>(bulletEntity);

                movementData.directionX = direction.x;
                movementData.directionY = direction.y;
                movementData.directionZ = direction.z;

                EntityManager.SetComponentData(bulletEntity, movementData);

                // EntityManager.SetComponentData(bulletEntity, new Rotation{Value = rotation.Value});
                if (EntityManager.Exists(bulletEntity))
                {
                    //Debug.Log("yas");
                    timer = attackSpeed.AttackSpeed;
                }
            }
        });
    }
}