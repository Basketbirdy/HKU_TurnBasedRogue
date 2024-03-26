using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ParticleUtils
{
    static ParticleUtils()
    {

    }

    // seperates given particles from the parent object
    public static void SeperateParticles(ParticleSystem particleSystem)
    {
        particleSystem.gameObject.transform.parent = null;
    }

    // turns the given particlesystem on or of depending on the given bool
    public static void TriggerSystem(ParticleSystem particleSystem, bool state) 
    { 
        if(!state) particleSystem.Stop(); 
        else particleSystem.Play();
    }

    // turns a given array of particlesystems on or off depending on the given bool
    public static void TriggerMultiple(ParticleSystem[] particleSystems, bool state)
    {
        foreach(ParticleSystem particle in particleSystems)
        {
            if (!state) particle.Stop();
            else particle.Play();
        }
    }

    // Seperates the given particles from the parent for when the parent gets destroyed, turns it on or off and starts lifetime timer
    public static void DestroyAfterSeperation(ParticleSystem particleSystem, bool playParticles)
    {
        SeperateParticles(particleSystem);
        TriggerSystem(particleSystem, playParticles);
        particleSystem?.GetComponent<DestroyAfterLifeTime>().DestroyAfterDelay(particleSystem.gameObject);
    }
}
