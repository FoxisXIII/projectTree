using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent] 
public struct AimComponent : IComponentData
{
    public float Range;
    public float TurnSpeed;
}
