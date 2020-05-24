using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
[Serializable]
public struct DealsDamage : IComponentData
{
    public int Value;
}
