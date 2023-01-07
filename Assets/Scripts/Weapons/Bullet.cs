using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    int bulletLife = 3;
    [SerializeField] int bulletDamage = 10;

    private void Awake()
    {
        Destroy(gameObject, bulletLife);
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.TakeHit(bulletDamage);
        }
        else if (other.gameObject.TryGetComponent<Agent>(out Agent agent))
        {
            agent.TakeHit(bulletDamage);
        }
    }
}
