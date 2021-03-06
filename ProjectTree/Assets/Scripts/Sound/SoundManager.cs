﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using FMOD.Studio;
using FMODUnity;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager Instance;
    private List<MovingSound> movingEvents;
    private EntityManager _entityManager;
    public float volume = 1.0f;

    private SoundManager()
    {
    }

    public static SoundManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject();
            Instance = go.AddComponent<SoundManager>();
            Instance.name = "SoundManager";
        }

        return Instance;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        movingEvents = new List<MovingSound>();
    }

    private void Update()
    {
        if (movingEvents != null && movingEvents.Count > 0)
        {
            for (int i = 0; i < movingEvents.Count; i++)
            {
                PLAYBACK_STATE state;
                EventInstance eventInstance = movingEvents[i].GetSoundEvent();
                eventInstance.getPlaybackState(out state);
                if (state == PLAYBACK_STATE.STOPPED)
                {
                    eventInstance.start();
                    eventInstance.release();
                }
                else
                {
                    Transform mTransform = movingEvents[i].GetTransform();
                    if (mTransform == null)
                    {
                        Entity entity = movingEvents[i].GetEntity();
                        if (_entityManager.Exists(entity) && _entityManager.HasComponent<Translation>(entity))
                            eventInstance.set3DAttributes(
                                RuntimeUtils.To3DAttributes(_entityManager.GetComponentData<Translation>(entity)
                                    .Value));
                        else
                        {
                            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                            movingEvents.RemoveAt(i);
                        }
                    }
                    else
                    {
                        eventInstance.set3DAttributes(
                            RuntimeUtils.To3DAttributes(movingEvents[i].GetTransform().position));
                    }
                }
            }
        }
    }

    public void StopAllSounds()
    {
        for (int i=0; i<movingEvents.Count; i++)
        {
            movingEvents[i].GetSoundEvent().stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            movingEvents.RemoveAt(i);
        }
        //RuntimeManager.PauseAllEvents(true);
    }

    //Saber si se está reproduciendo
    public bool IsPlaying(EventInstance soundEvent)
    {
        PLAYBACK_STATE state;
        soundEvent.getPlaybackState(out state);
        return !state.Equals(PLAYBACK_STATE.STOPPED);
    }

    //Sonidos que queramos controlar des de fuera
    public EventInstance PlayEvent(string path, Vector3 position, float volume)
    {
        EventInstance soundEvent = RuntimeManager.CreateInstance(path);
        if (!soundEvent.Equals(null))
        {
            soundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(position));
            soundEvent.setVolume(volume);
            soundEvent.start();
        }

        return soundEvent;
    }

    //Para sonidos que no queremos controlar, que cuando acaban, se destruyen solos

    //Sonidos que se mueven con Gameobject
    public void PlayOneShotSound(string path, Transform transform)
    {
        EventInstance soundEvent = RuntimeManager.CreateInstance(path);
        if (!soundEvent.Equals(null))
        {
            soundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            soundEvent.start();
            MovingSound movingSound = new MovingSound(transform, soundEvent);
            movingEvents.Add(movingSound);
            soundEvent.release();
        }
    }

    //Sonidos que no se mueven
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

    //Sonidos que se mueven con Entity
    public void PlayOneShotSound(string path, Entity entity)
    {
        EventInstance soundEvent = RuntimeManager.CreateInstance(path);
        Vector3 position = _entityManager.GetComponentData<Translation>(entity).Value;
        if (!soundEvent.Equals(null))
        {
            soundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(position));
            soundEvent.start();
            MovingSound movingSound = new MovingSound(soundEvent, entity);
            movingEvents.Add(movingSound);
            soundEvent.release();
        }
    }
    
    public void ChangeVolume(float volume)
    {
        string masterBusString = "Bus:/";
        FMOD.Studio.Bus masterBus;

        masterBus = FMODUnity.RuntimeManager.GetBus(masterBusString);
        masterBus.setVolume(volume);
    }
}