using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class BulletShootingSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        float timer;
        Entities.WithAll<TowerTag, TowerCurrentTarget, MuzzleComponent>().ForEach((Entity entity, ref AttackSpeedComponent attackSpeed, ref BulletPrefabComponent bullet, ref MuzzleComponent muzzle
            ) =>
        {
            timer = attackSpeed.AttackSpeed;
            timer -= Time.DeltaTime;
            if (timer <= 0)
            {
                timer = attackSpeed.AttackSpeed;
                Entity bulletEntity = EntityManager.Instantiate(bullet.prefab);
                EntityManager.SetComponentData(bulletEntity, new Translation{Value = muzzle.transform.position});
                EntityManager.SetComponentData(bulletEntity, new Rotation{Value = muzzle.transform.rotation});
            }
        });
    }
}
