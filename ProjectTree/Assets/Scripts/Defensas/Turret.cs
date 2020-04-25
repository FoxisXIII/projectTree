using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float Range;
    [Range(0,1)]
    public float AttackSpeed;
    public GameObject BulletPrefab;

    private GameObject target;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SphereCollider>().radius = Range;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            PointAt();
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
}
