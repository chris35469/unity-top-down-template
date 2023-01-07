using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Agent : Unit
{

    [Header("Bullet")]
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 10f;
    NavMeshAgent navMeshAgent;
    float speedMultiplier = 2f;
    RaycastHit hit;
    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed *= speedMultiplier;
    }
    void FixedUpdate()
    {
        SensePlayer();
        CheckShoot();
    }

    void shoot()
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;
    }

    void CheckShoot()
    {
        Ray ray = new Ray(bulletSpawnPoint.position, bulletSpawnPoint.forward);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                Debug.Log("Shoot");
                shoot();
            }
        }
    }

    void SensePlayer()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, detectionRange);

        foreach (Collider collider in colliderArray)
        {
            if (collider.gameObject.tag == "Player")
            {
                navMeshAgent.destination = collider.transform.position;
            }
        }
    }

    public void TakeHit(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
