using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using FMOD.Studio;
using FMODUnity;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SoundManager
{
    private static SoundManager Instance;
    private List<EventInstance> eventsList = new List<EventInstance>();
    
    private SoundManager()
    {
    }

    public static SoundManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new SoundManager();
        }

        return Instance;
    }

    public bool IsPlaying(EventInstance soundEvent)
    {
        PLAYBACK_STATE state;
        soundEvent.getPlaybackState(out state);
        return !state.Equals(PLAYBACK_STATE.STOPPED);
    }

    public EventInstance PlayEvent(string path, Vector3 position)
    {
        EventInstance soundEvent = RuntimeManager.CreateInstance(path);
        if (!soundEvent.Equals(null))
        {
            soundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(position));
            soundEvent.start();
            //eventsList.Add(soundEvent);
        }
        return soundEvent;
    }

    // public void PlayEventOnGameObject(string path, Transform transform, Rigidbody r = null)
    // {
    //     EventInstance soundEvent = RuntimeManager.CreateInstance(path);
    //     if (!soundEvent.Equals(null))
    //     {
    //         if (r.Equals(null))
    //             r = new Rigidbody();
    //         RuntimeManager.AttachInstanceToGameObject(soundEvent, transform, r);
    //         soundEvent.start();
    //         eventsList.Add(soundEvent);
    //     }
    // }
    
    
    //Para sonidos que no queremos controlar, que cuando acaban, se destruyen solos
    public void PlayOneShotSound(string path, Transform transform)
    {
        EventInstance soundEvent = RuntimeManager.CreateInstance(path);
        if (!soundEvent.Equals(null))
        {
            soundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            soundEvent.start();
            soundEvent.release();
        }
    }

    public void PlayOneShotSound(string path, Vector3 position)
    {
        EventInstance soundEvent = RuntimeManager.CreateInstance(path);
        if (!soundEvent.Equals(null))
        {
            soundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(position));
            soundEvent.start();
            soundEvent.release();
        }
    }
    
    //Para objetos con parámetros
    // public void PlayOneShotSound(string path, Vector3 pos, List<SoundManagerParameter> parameters = null)
    // {
    //     EventInstance soundEvent = RuntimeManager.CreateInstance(path);
    //     if (!soundEvent.Equals(null))
    //     {
    //         if (parameters != null)
    //         {
    //             for (int i = 0; i < parameters.Count; i++)
    //             {
    //                 soundEvent.setParameterByName(parameters[i].GetName(), parameters[i].GetValue());
    //             }
    //         }
    //
    //         soundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
    //         soundEvent.start();
    //         soundEvent.release();
    //     }
    // }
}
