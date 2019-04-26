using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    [SerializeField] private int speed = 30;
    [SerializeField] private float destroyAfter = 2;
    [HideInInspector] public GameObject shotFrom;
    
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed);
        Destroy(gameObject, destroyAfter);
    }
}
