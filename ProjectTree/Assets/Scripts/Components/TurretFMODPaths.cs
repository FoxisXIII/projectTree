using System;
using Unity.Collections;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct TurretFMODPaths : IComponentData
{
   public NativeString32 ShotPath;
   public NativeString32 HealPath;
   public NativeString32 BuffPath;
}
