using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursue : SteeringBehaviour
{
    public Boid target;

    Vector3 targetPos;
    private bool slowedDown = false;

    public void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetPos);
        }
    }

    public override Vector3 Calculate()
    {
        if (target)
        {
            
            float dist = Vector3.Distance(target.transform.position, transform.position);
            
            if (dist < 1000 && !slowedDown)
            {
                boid.maxSpeed = boid.maxSpeed / 10;
                slowedDown = true;
            }
            else if (dist > 1000 && slowedDown)
            {
                boid.maxSpeed = boid.maxSpeed * 10;
                slowedDown = false;
            }
            float time = dist / boid.maxSpeed;

            targetPos = target.transform.position + (target.velocity * time);

            return boid.SeekForce(targetPos);
        }

        return Vector3.forward * boid.maxSpeed;
    }

    private void Update()
    {
        if (!target)
        {
            target = null;
        }
        
    }

    private void OnDisable()
    {
        if (slowedDown)
        {
            boid.maxSpeed = boid.maxSpeed * 10;
        }
    }
}
