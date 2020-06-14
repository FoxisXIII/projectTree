using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem particle = GetComponent<ParticleSystem>();
        float totalDuration = particle.duration + particle.startLifetime;
        Destroy(gameObject, totalDuration);
    }
}
