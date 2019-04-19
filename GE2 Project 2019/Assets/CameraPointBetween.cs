using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPointBetween : MonoBehaviour
{
    [SerializeField] private Transform target, otherTarget;

    [Range(0, 1)]public float scalar;

    private void OnDrawGizmos()
    {
        if (target && otherTarget)
        {
            Gizmos.color = Color.blue;
            float dist = Vector3.Distance(target.position, otherTarget.position);
            Vector3 pointInBetween = (target.position - otherTarget.position).normalized * (dist * scalar);
            Gizmos.DrawLine(otherTarget.position, otherTarget.position + pointInBetween);
        }
    }

    private void Update()
    {
        if (target && otherTarget)
        {
            Vector3 pointInBetween = (target.position - otherTarget.position).normalized * scalar;
            transform.LookAt(otherTarget.position + pointInBetween);
            //pursue boid
        }
    }
}

