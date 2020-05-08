using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct AIData : IComponentData
{
    [HideInInspector] public int state;

    public float attackDistancePlayer;
    public float attackDistanceBase;
    public float chaseDistance;

    public int attackDamage;
    public float attackRate;

    [HideInInspector] public float attackWait;

    [HideInInspector] public float attackTime;

    [HideInInspector] public bool changePosition;
    [HideInInspector] public float3 position;
    [HideInInspector] public float3 finalPosition;
    [HideInInspector] public float3 direction;
    [HideInInspector] public int counter;
    [HideInInspector] public bool canAttackPlayer;
    [HideInInspector] public bool hasToInitialize;
}