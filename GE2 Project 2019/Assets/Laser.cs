using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : ShootingBehavior
{
    private int spawnPos = 0;
    [SerializeField] private float timeBetweenSpawn;
    [SerializeField] private float chargeTime;
    private bool nextSpawn = true;

    private void Start()
    {
        return;
    }


    public override void Calculate()
    {
        if (canShoot && nextSpawn)
        {
            StartCoroutine("chargeShot");
            canShoot = false;
        }
    }

    private IEnumerator chargeShot()
    {
        float t = Time.time + chargeTime;
        while (Time.time < t)
        {
            //Charge sound
            yield return null;
            
        } 
        Shoot();
        canShoot = true;
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

    private void Shoot()
    {
        Instantiate(projectile, projectileSpawns[spawnPos].position, transform.rotation);
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

