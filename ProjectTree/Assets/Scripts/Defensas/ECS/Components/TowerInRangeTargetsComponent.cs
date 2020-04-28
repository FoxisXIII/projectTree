using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
[GenerateAuthoringComponent]
public struct TowerInRangeTargets : IBufferElementData
{
    public Entity target;
}
