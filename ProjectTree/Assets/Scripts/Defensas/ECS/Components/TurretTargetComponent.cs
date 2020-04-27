using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
[GenerateAuthoringComponent]
public struct TurretTarget : IBufferElementData
{
    public Entity target;
}
