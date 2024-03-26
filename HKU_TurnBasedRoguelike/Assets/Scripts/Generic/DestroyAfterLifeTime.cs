using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterLifeTime : MonoBehaviour
{
    // script to put on particle effects that need to last when parent object is destroyed - See ParticleUtils

    [SerializeField] bool destroyAfterLifeTime = false;
    float timer;
    [SerializeField] float lifeTime;
    // Start is called before the first frame update
    void Start()
    {
        if (!destroyAfterLifeTime) { return; }
        timer = Time.time + lifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timer && timer != 0) { Destroy(gameObject); }
    }

    // call in former-parent of particles to start timer
    public void DestroyAfterDelay(GameObject obj)
    {
        timer = Time.time + lifeTime;
    }
}
