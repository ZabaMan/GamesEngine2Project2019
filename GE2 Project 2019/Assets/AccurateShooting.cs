using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccurateShooting : ShootingBehavior
{
    [SerializeField] private float accuracySpherCastWidth = 500;
    private int spawnPos = 0;
    [SerializeField] private float timeBetweenSpawn;
    private bool nextSpawn = true;
    [SerializeField] private GameObject targetGameObject;
    AudioSource audioSource;
    

    private void Start()
    {
        if(GetComponent<AudioSource>())
        audioSource = GetComponent<AudioSource>();
    }

    public override void Calculate()
    {
        
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 100000, Color.green);
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, accuracySpherCastWidth, fwd, out hit) && hit.transform.tag == "Bad")
        {
            if (canShoot && nextSpawn)
            {

                Instantiate(projectile, projectileSpawns[spawnPos].position, transform.rotation);
                audioSource?.Play();
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
                    if (reloadTime > 0)
                        Invoke("CanShoot", reloadTime);
                }
                else
                {
                    nextSpawn = false;
                    Invoke("NextSpawn", timeBetweenSpawn);
                }
            }
        }
        else if (Physics.SphereCast(transform.position, 500, fwd, out hit))
        {
            Debug.DrawRay(transform.position, hit.point, Color.blue);
            Debug.Log(hit.transform.name);
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
