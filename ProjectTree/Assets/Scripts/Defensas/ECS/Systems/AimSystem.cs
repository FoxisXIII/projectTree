using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
using quaternion = Unity.Mathematics.quaternion;

[UpdateBefore(typeof(TurretAttackSystem))]
public class AimSystem : JobComponentSystem
{
    [BurstCompile]
    struct AimJob : IJobForEach<Rotation, Translation, AimComponent>
    {
        public float deltaTime;
        private float3 enemyPosition;

        public void Execute(ref Rotation rotation, ref Translation position, [ReadOnly] ref AimComponent aim)
        {
            float3 dir = enemyPosition - position.Value;
            dir.y = 0f;
            rotation.Value = quaternion.LookRotation(dir, math.up());
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new AimJob
        {
            deltaTime = UnityEngine.Time.deltaTime,
            //enemyPosition = ;
        };

        // Now that the job is set up, schedule it to be run. 
        return job.Schedule(this, inputDependencies);
    }
}