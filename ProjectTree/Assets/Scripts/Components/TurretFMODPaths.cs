using System;
using Unity.Collections;
using Unity.Entities;


public struct TurretFMODPaths : IComponentData
{
   public NativeString64 ShotPath;
   public NativeString64 DestroyPath;
   public NativeString64 AuraPath;
   public NativeString64 BuffPath;
}
