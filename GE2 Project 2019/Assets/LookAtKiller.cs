using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtKiller : MonoBehaviour
{
    [SerializeField] private Boid initialTarget;
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        target = initialTarget.transform;
        initialTarget.gameObject.AddComponent<CameraAttention>().camera = gameObject.GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
    }

    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
        target.gameObject.AddComponent<CameraAttention>().camera = gameObject.GetComponent<Camera>();
    }
}
