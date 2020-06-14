using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class LowLifeSoundSystem : ComponentSystem
{
    private EventInstance lowLifeSound = new EventInstance();
    
    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerTag, HealthData>().ForEach((Entity e, ref HealthData hp) =>
        {
            if (hp.value <= hp.maxValue * 0.2)
            {
                if (!SoundManager.GetInstance().IsPlaying(lowLifeSound))
                {
                    lowLifeSound = SoundManager.GetInstance().PlayEvent("event:/FX/Character/LowLife", float3.zero, 7f);
                    GameController.GetInstance().GetLowLifeSoundEvent(lowLifeSound);
                }
            }
        });
    }
}
