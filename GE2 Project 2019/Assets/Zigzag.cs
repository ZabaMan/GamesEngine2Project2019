using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zigzag : SteeringBehaviour
{
    [SerializeField] private float angle;
    [SerializeField] private float distance;
    [SerializeField] private float frequency;
    private int flip = 1;

    private Vector3 target;
    private bool flipping;

    private void OnDrawGizmos()
    {
        Vector3 gizmoTarget = Vector3.forward * distance;
        gizmoTarget += transform.position;
        gizmoTarget.x = angle * flip;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, gizmoTarget);
    }

    public override Vector3 Calculate()
    {
        if (!flipping)
        {
            StartCoroutine("turnZigToZag");
            flipping = true;
        }

        target = Vector3.forward * distance;
        target += transform.position;
        target.x = angle * flip;
        
        
        return boid.SeekForce(target);
    }

    private IEnumerator turnZigToZag()
    {
        yield return new WaitForSeconds(frequency);
        flip *= -1;
        flipping = false;
        
    }

    private void Update()
    {
        
    }
}
