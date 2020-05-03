using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateBefore(typeof(DeathSystem))]
public class ResolveDamageSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem ecb;

    protected override void OnCreate()
    {
        ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer ecb = this.ecb.CreateCommandBuffer();
        Entities.WithoutBurst().WithNone<Dead>().ForEach(
            (Entity e, ref DynamicBuffer<Damage> damageBuffer, ref HealthData hp) =>
            {
                foreach (var damage in damageBuffer)
                {
                    hp.Value -= damage.Value;

                    if (hp.Value <= 0)
                    {
                        GameController.GetInstance().RemoveEnemyWave();
                        hp.Value = 0;
                        ecb.AddComponent<Dead>(e);
                        break;
                    }
                }
                damageBuffer.Clear();
            }).Run();

        return default;
    }
}
