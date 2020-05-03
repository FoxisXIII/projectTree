using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
[Serializable]
public struct RangeComponent : IComponentData
{
    public float Value;
}
