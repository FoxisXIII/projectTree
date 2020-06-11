using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatingSpot : MonoBehaviour
{
    public ParticleSystem _particleSystem;
    private bool _hasTurret;

    private void Start()
    {
        _particleSystem.Stop();
    }

    public void AddTurret()
    {
        _hasTurret = true;
        if (_particleSystem.isPlaying)
        {
            _particleSystem.Stop();
        }
    }

    public void ActivateParticles()
    {
        if (!_hasTurret)
            _particleSystem.Play();
    }

    public void StopParticles()
    {
        _particleSystem.Stop();
    }
}
