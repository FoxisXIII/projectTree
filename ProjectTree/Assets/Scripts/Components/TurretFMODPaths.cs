using System;
using Unity.Collections;
using Unity.Entities;


public struct TurretFMODPaths : IComponentData
{
   public NativeString32 ShotPath;
   public NativeString32 DestroyPath;
   public NativeString32 AuraPath;
   public NativeString32 HealPath;
   public NativeString32 BuffPath;
}
