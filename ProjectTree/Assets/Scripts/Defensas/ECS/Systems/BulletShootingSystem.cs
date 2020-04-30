using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BulletShootingSystem : ComponentSystem
{
    float timer;
    protected override void OnUpdate()
    {
        Entities.WithAll<TowerTag, TowerCurrentTarget>().ForEach((Entity entity, ref AttackSpeedComponent attackSpeed, ref BulletPrefabComponent bullet, ref Translation position, ref Rotation rotation
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
                //Debug.Log(where);
                //Debug.Log(rotation.Value);
                EntityManager.SetComponentData(bulletEntity, new Translation{Value = position.Value});
                EntityManager.SetComponentData(bulletEntity, new Rotation{Value = rotation.Value});
                if (EntityManager.Exists(bulletEntity))
                {
                    //Debug.Log("yas");
                    timer = attackSpeed.AttackSpeed;
                }
            }
        });
    }
}
