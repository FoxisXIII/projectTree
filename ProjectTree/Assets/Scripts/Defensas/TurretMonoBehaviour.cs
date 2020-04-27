using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Serialization;

public class TurretMonoBehaviour : MonoBehaviour
{
    public float Range;
    [Range(0,1)]
    public float FireRate;
    private float timer = 0f;

    public Transform gunBarrel;
    public GameObject BulletPrefab;
    private Entity bulletEntityPrefab;
    private EntityManager _manager;

    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SphereCollider>().radius = Range;
        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        bulletEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(BulletPrefab,
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, new BlobAssetStore()));
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (target != null)
        {
            PointAt();
            if (timer >= FireRate)
            {
                Quaternion rotation = gunBarrel.rotation;
                SpawnBullet(rotation);
                timer = 0f;
            }
        }
    }

    private void PointAt()
    {
        float3 dir = target.transform.position - this.transform.position;
        dir.y = 0f;
        this.transform.rotation = quaternion.LookRotation(dir, math.up());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            target = other.gameObject;
        }
    }

    void SpawnBullet(Quaternion rotation)
    {
        Entity bulletEntity = _manager.Instantiate(bulletEntityPrefab);
        
        _manager.SetComponentData(bulletEntity, new Translation{Value = gunBarrel.position});
        _manager.SetComponentData(bulletEntity, new Rotation{Value = rotation});
        
    }
}
