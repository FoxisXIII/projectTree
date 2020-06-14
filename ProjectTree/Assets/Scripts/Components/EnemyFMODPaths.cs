using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct EnemyFMODPaths : IComponentData
{
    public NativeString32 AttackPlayerPath;
    public NativeString32 AttackBasePath;
    public NativeString32 HitPath;
    public NativeString32 DiePath;
}
