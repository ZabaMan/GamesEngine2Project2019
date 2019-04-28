using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : SteeringBehaviour
{

    [SerializeField] int spinAmount;
    float spin = 0;
    float t = 0;

    private void Start()
    {
        return;
    }

    public override Vector3 Calculate()
    {
        
        t += Time.deltaTime * 0.5f;
        spin = Mathf.Lerp(0, spinAmount, t);
        print(spin);
        transform.GetChild(0).transform.Rotate(Vector3.up, spin * Time.deltaTime);
        //boid.spin = Vector3.Lerp(transform.up, Vector3.up + (Vector3.right * spinAmount), Time.deltaTime * 3.0f);
        return Vector3.forward;
    }
}
