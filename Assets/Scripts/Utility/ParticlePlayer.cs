using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    public ParticleSystem[] allParticle;
    // Start is called before the first frame update
    void Start()
    {
        allParticle = GetComponentsInChildren<ParticleSystem>();
    }

    public void PlayParticles()
    {
        foreach(ParticleSystem p in allParticle)
        {
            p.Stop();
            p.Play();
        }
    }
}
