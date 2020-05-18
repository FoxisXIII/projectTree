using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct AttackSpeedComponent : IComponentData
{
    [Range(0,3)]
    public float AttackSpeed;
}
