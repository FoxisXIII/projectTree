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
        }

        return soundEvent;
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
}
