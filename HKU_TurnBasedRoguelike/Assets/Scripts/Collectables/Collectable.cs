using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour, ICollectable
{
    public enum CollectableType { cheese };
    public CollectableType type;

    [SerializeField] ParticleSystem particles;

    public void Collect()
    {
        switch (type)
        {
            case CollectableType.cheese:
                GameManager.instance.CollectCheese();
                ParticleUtils.DestroyAfterSeperation(particles, true);
                Destroy(gameObject);
                break;
        }
    }
}
