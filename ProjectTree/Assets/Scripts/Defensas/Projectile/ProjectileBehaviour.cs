using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{
    public float speed = 50f;

    public float lifeTime = 1f;

    private Rigidbody bulletRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        bulletRigidBody = GetComponent<Rigidbody>();
        Invoke("RemoveProjectile", lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = Time.deltaTime * speed * transform.forward;
        bulletRigidBody.MovePosition(transform.position + movement);
    }

    private void OnCollisionEnter(Collision other)
    {
        RemoveProjectile();
    }

    void RemoveProjectile()
    {
        Destroy(gameObject);
    }
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent(entity, typeof(MoveForward));
        dstManager.AddComponent(entity, typeof(DestroyOnContact));
        dstManager.AddComponentData(entity, new DealsDamage {Value = 50f});
        MoveSpeed moveSpeed = new MoveSpeed{Value = speed};
        dstManager.AddComponentData(entity, moveSpeed);
        TimeToLive ttl = new TimeToLive{Value = lifeTime};
        dstManager.AddComponentData(entity, ttl);
    }
}
