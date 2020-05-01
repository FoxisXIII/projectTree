using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Entities;
using UnityEngine;

public class Bullet : MonoBehaviour, IConvertGameObjectToEntity
{

    public float lifetime;
    public float speed;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Invoke("LifePass",lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    void LifePass()
    {
        Destroy(gameObject);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent(entity, typeof(MovesForwardComponent));
        dstManager.AddComponentData(entity, new SpeedComponent {Value = speed});
    }
}
