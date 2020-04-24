using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
using quaternion = Unity.Mathematics.quaternion;

public class TurretAttackSystem : JobComponentSystem
{
    
    [BurstCompile]
    struct AimJob : IJobForEach<AttackComponent, PointedEnemyComponent>
    {
        public float deltaTime;

        public void Execute(ref AttackComponent attack, ref PointedEnemyComponent target)
        {
            
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new AimJob
        {
            deltaTime = UnityEngine.Time.deltaTime
        };

        // Now that the job is set up, schedule it to be run. 
        return job.Schedule(this, inputDependencies);
    }
}
