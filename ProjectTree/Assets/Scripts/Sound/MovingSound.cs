using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Unity.Entities;
using UnityEngine;

public class MovingSound
{
    private Transform _transform;
    private EventInstance _soundEvent;
    private Entity _entity;
    
    public MovingSound(Transform transform, EventInstance soundEvent)
    {
        _transform = transform;
        _soundEvent = soundEvent;
    }

    public MovingSound(EventInstance soundEvent, Entity entity)
    {
        _soundEvent = soundEvent;
        _entity = entity;
    }

    public Transform GetTransform()
    {
        return _transform;
    }

    public Entity GetEntity()
    {
        return _entity;
    }

    public EventInstance GetSoundEvent()
    {
        return _soundEvent;
    }
}
