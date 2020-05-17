using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
public class Trap : MonoBehaviour
{

    private int Kills = 0;
    private EntityManager _manager;
    public int canKill=10;
    
    // Start is called before the first frame update
    void Start()
    {
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Kills>canKill)
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
