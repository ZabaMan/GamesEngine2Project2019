﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetPursue : SteeringBehaviour
{
    public Boid leader;

    Vector3 targetPos;
    Vector3 worldTarget;
    public Vector3 offset;
    [SerializeField] private bool moveToLeader;
    [SerializeField] Transform targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        // There is a bug here!! On the false statement
        if(!moveToLeader)
        offset = transform.position - leader.transform.position;
    }

    public override Vector3 Calculate()
    {
        if (!moveToLeader)
        {
            worldTarget = leader.transform.TransformPoint(offset);

            float dist = Vector3.Distance(worldTarget, transform.position);
            float time = dist / boid.maxSpeed;
            targetPos = worldTarget + (leader.velocity * time);
            return boid.ArriveForce(targetPos);
        }
        else
        {
            worldTarget = targetPosition.position;

            float dist = Vector3.Distance(worldTarget, transform.position);
            float time = dist / boid.maxSpeed;
            targetPos = worldTarget + (targetPosition.GetComponentInParent<Boid>().velocity * time);
            return boid.ArriveForce(targetPos);
        }
    }
 
}
