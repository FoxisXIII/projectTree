using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{
    public float speed = 50f;

    public float lifeTime = 1f;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new DealsDamage {Value = 20});
        MoveSpeed moveSpeed = new MoveSpeed{Value = speed};
        dstManager.AddComponentData(entity, moveSpeed);
        TimeToLive ttl = new TimeToLive{Value = lifeTime};
        dstManager.AddComponentData(entity, ttl);
    }
}
