using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : ShootingBehavior
{
    private int spawnPos = 0;
    [SerializeField] private float timeBetweenSpawn;
    private bool nextSpawn = true;
    [SerializeField] private GameObject targetGameObject;

    private void Start()
    {
        return;
    }

    public override void Calculate()
    {
        if (canShoot && nextSpawn)
        {
            Instantiate(projectile, projectileSpawns[spawnPos].position, transform.rotation);
            if (projectile.GetComponent<Seek>())
            {

                projectile.GetComponent<Seek>().targetGameObject = this.targetGameObject;
                print("Assigned");
            }
            else
            {
                projectile.GetComponent<MoveForward>().shotFrom = gameObject;
            }
            spawnPos++;
            if (spawnPos >= projectileSpawns.Length)
            {
                canShoot = false;
                if (reloadTime>0)
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
