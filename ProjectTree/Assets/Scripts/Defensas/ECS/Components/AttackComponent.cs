using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct AttackComponent : IComponentData
{
    //public GameObject BulletPrefab;
    [Range(0,1)]
    public float AttackSpeed;
}
