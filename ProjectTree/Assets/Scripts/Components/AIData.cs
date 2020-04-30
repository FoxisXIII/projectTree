using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct AIData : IComponentData
{
    [HideInInspector] public int state;

    public float attackDistance;
    public float chaseDistance;

    public int attackDamage;

    [HideInInspector] public float attackTime;

    [HideInInspector] public bool changePosition;
    [HideInInspector] public float3 position;
    [HideInInspector] public float3 finalPosition;
    [HideInInspector] public float3 positionOffset;
}