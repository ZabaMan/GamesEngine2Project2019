using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forward : SteeringBehaviour
{
    private void Start()
    {
        return;
    }

    public override Vector3 Calculate()
    {
        return Vector3.forward * boid.maxSpeed;
    }
}
