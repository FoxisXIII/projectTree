using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Transforms
{
    public class MoveForwardSystem : JobComponentSystem
    {
        [BurstCompile]
        [RequireComponentTag(typeof(MoveForward))]
        struct MoveForwardRotation : IJobForEach<Translation, Rotation, MoveSpeed>
        {
            public float DeltaTime;
            
            public void Execute(ref Translation position, ref Rotation rotation, ref MoveSpeed speed)
            {
                position.Value = position.Value + (DeltaTime * speed.Value * math.forward(rotation.Value));
            }
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var moveForwardRotationJob = new MoveForwardRotation
            {
                DeltaTime = Time.DeltaTime
            };
            return moveForwardRotationJob.Schedule(this, inputDeps);
        }
    }
}

