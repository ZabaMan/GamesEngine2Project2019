using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraKeepBackgroundObjectsInView : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform[] keepInView;
    [SerializeField] private float cameraSpeed;

    public float scalar;

    private void OnDrawGizmos()
    {
        if (target)
        {
            Gizmos.color = Color.blue;
            float dist = Vector3.Distance(target.position, keepInView[0].position);
            Vector3 extendedTarget = (target.position - keepInView[0].position).normalized * (dist + scalar);
            Gizmos.DrawLine(keepInView[0].position + extendedTarget, keepInView[0].position);
        }
    }

    private void Update()
    {
        if (target)
        {
            // The step size is equal to speed times frame time.
            float step = cameraSpeed * Time.deltaTime;
            Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, step);
            //transform.LookAt(target);
            float dist = Vector3.Distance(target.position, keepInView[0].position);
            Vector3 extendedTarget = (target.position - keepInView[0].position).normalized * (dist + scalar);
            transform.position = Vector3.Lerp(transform.position, keepInView[0].position + extendedTarget, Time.deltaTime * 60);
        }
    }
}
