using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootingBehavior : MonoBehaviour
{
    public float weight = 1.0f;
    public bool canShoot = true;
    public GameObject projectile;
    public float reloadTime;
    public Transform[] projectileSpawns;

    [HideInInspector]
    public Boid boid;

    public void Awake()
    {
        boid = GetComponent<Boid>();
    }

    public abstract void Calculate();
}
