﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * 5);
        Destroy(gameObject, 0.5f);
    }
}