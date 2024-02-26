using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ParticleUtils
{
    static ParticleUtils()
    {

    }

    public static void SeperateParticles(ParticleSystem particleSystem)
    {
        particleSystem.gameObject.transform.parent = null;

    }

    public static void TriggerSystem(ParticleSystem particleSystem, bool state) 
    { 
        if(!state) particleSystem.Stop(); 
        else particleSystem.Play();
    }

    public static void TriggerMultiple(ParticleSystem[] particleSystems, bool state)
    {
        foreach(ParticleSystem particle in particleSystems)
        {
            if (!state) particle.Stop();
            else particle.Play();
        }
    }

    public static void DestroyAfterSeperation(ParticleSystem particleSystem, bool playParticles)
    {
        SeperateParticles(particleSystem);
        TriggerSystem(particleSystem, playParticles);
        particleSystem?.GetComponent<DestroyAfterLifeTime>().DestroyAfterDelay(particleSystem.gameObject);
    }
}
