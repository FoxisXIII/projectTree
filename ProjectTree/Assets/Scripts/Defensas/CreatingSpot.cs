using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = FMOD.Debug;

public class CreatingSpot : MonoBehaviour
{
    public ParticleSystem _particleSystem;
    private bool _hasTurret, _topView;
    private EntityManager _manager;
    private Entity _associatedTurret;

    private void Start()
    {
        _particleSystem.Stop();
        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Update()
    {
        if (_hasTurret)
        {
            if (!_manager.Exists(_associatedTurret))
            {
                _hasTurret = false;
                if (_topView)
                    _particleSystem.Play();
            }
        }
    }

    public void AddTurret(Entity turret)
    {
        _hasTurret = true;
        _associatedTurret = turret;
        if (_particleSystem.isPlaying)
        {
            _particleSystem.Stop();
        }
    }

    public bool HasTurret => _hasTurret;

    public void ActivateParticles()
    {
        if (!_hasTurret)
            _particleSystem.Play();
        _topView = true;
    }

    public void StopParticles()
    {
        _particleSystem.Stop();
        _topView = false;
    }
}
