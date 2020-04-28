using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct MuzzleComponent : IComponentData
{
   public Transform transform;
}
