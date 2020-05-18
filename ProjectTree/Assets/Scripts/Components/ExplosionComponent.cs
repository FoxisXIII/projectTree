using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct ExplosionComponent : IComponentData
{
    public int damage;
    public float ttl;
    [HideInInspector] public float timer;
    [HideInInspector] public bool explode;
}