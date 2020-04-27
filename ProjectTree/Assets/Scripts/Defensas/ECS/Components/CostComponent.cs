using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct CostComponent : IComponentData
{
   public int Resource1;
   public int Resource2;
}
