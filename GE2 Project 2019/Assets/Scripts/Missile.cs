using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : ShootingBehavior
{
    private int spawnPos = 0;
    [SerializeField] private float timeBetweenSpawn;
    private bool nextSpawn = true;

    public override void Calculate()
    {
        if (canShoot && nextSpawn)
        {
            Instantiate(projectile, projectileSpawns[spawnPos].position, transform.rotation);
            if (projectile.GetComponent<Pursue>())
            {

                projectile.GetComponent<Pursue>().target = boid.GetComponent<Seek>().targetGameObject.GetComponent<Boid>();
                print("Assigned");
            }
            spawnPos++;
            if (spawnPos >= projectileSpawns.Length)
            {
                canShoot = false;
                Invoke("CanShoot", reloadTime);
            }
            else
            {
                nextSpawn = false;
                Invoke("NextSpawn", timeBetweenSpawn);
            }
        }
    }

    private void NextSpawn()
    {
        nextSpawn = true;
    }

    private void CanShoot()
    {
        canShoot = true;
        spawnPos = 0;
    }
}
