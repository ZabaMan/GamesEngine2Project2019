using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAttention : MonoBehaviour
{
    [HideInInspector]public Camera camera;

    public void TellCameraObjectIsDead(GameObject killer)
    {
        camera.GetComponent<LookAtKiller>().ChangeTarget(killer.transform);
    }
}
