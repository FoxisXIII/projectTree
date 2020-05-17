using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Trap : MonoBehaviour
{

    private int Kills = 0;
    private Entity bulletEntityPrefab;
    private EntityManager _manager;
    private BlobAssetStore blob;
    
    // Start is called before the first frame update
    void Start()
    {
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Kills>10)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Kills++;
        }
    }
}
