using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using FMOD.Studio;
using FMODUnity;
using Unity.Transforms;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager Instance;
    private List<EventInstance> eventsList;

    public static SoundManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new SoundManager();
        }

        return Instance;
    }

    private void Start()
    {
        eventsList = new List<EventInstance>();
    }

    private void Update()
    {
        // if (positionEvents != null && positionEvents.Count > 0)
        // {
        //     for (int i = 0; i < positionEvents.Count; i++)
        //     {
        //         PLAYBACK_STATE state;
        //         EventInstance eventInstance = positionEvents[i].GetEventInstance();
        //         eventInstance.getPlaybackState(out state);
        //         if (state == PLAYBACK_STATE.STOPPED)
        //         {
        //             positionEvents.RemoveAt(i);
        //         }
        //         else
        //         {
        //             eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(positionEvents[i].GetTransform.position));
        //         }
        //     }
        // }
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
            eventsList.Add(soundEvent);
        }

        return soundEvent;
    }

    public void PlayEventOnGameObject(string path, Transform transform, Rigidbody r = null)
    {
        EventInstance soundEvent = RuntimeManager.CreateInstance(path);
        if (!soundEvent.Equals(null))
        {
            if (r.Equals(null))
                r = new Rigidbody();
            RuntimeManager.AttachInstanceToGameObject(soundEvent, transform, r);
            soundEvent.start();
            eventsList.Add(soundEvent);
        }
    }

    //Para objetos en movimiento que actualizan la posición del sonido
    public void PlayOneShotSound(string path, Transform transform)
    {
        EventInstance soundEvent = RuntimeManager.CreateInstance(path);
        if (!soundEvent.Equals(null))
        {
            soundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            soundEvent.start();
            //SoundManagerMovingSound movingSound = new SoundManagerMovingSound(transform, soundEvent);
            //positionEvents.Add(movingSound);
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
