using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{
    public float speed;

    public float lifeTime;

    public int damage;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new DealsDamage {Value = damage});
        MoveSpeed moveSpeed = new MoveSpeed{Value = speed};
        dstManager.AddComponentData(entity, moveSpeed);
        TimeToLive ttl = new TimeToLive{Value = lifeTime};
        dstManager.AddComponentData(entity, ttl);
    }
}
