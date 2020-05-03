using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[InternalBufferCapacity(20)]
[Serializable]
[GenerateAuthoringComponent]
public struct EnemiesInRange : IBufferElementData
{
    public Entity Value;
}
