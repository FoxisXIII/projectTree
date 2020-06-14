using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct TrapComponent :IComponentData
{
        
        public int Damage;
        public float Recover;
        public bool cankill;
        public int times;
}
