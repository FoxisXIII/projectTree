using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
[GenerateAuthoringComponent]
public struct EnemiesInRange : IBufferElementData
{
    public Entity Value;
}
