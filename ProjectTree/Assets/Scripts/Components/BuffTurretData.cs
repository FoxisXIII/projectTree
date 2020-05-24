using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BuffTurretData : IComponentData
{
    public float range;
    
    public int health;
    public int resources;
    public int attack;
    public int speed;
    public int shotgun;

    public float buffRate;
    public float buffTimer;
    public float buffDisapear;
}