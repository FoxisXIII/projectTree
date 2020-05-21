using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct TrapComponent :IComponentData
{
        public int Deaths;
        public int Damage;

}
