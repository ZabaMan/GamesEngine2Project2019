using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActWhenCameraViews : MonoBehaviour
{

    [SerializeField] private float stallTime;
    [SerializeField] private GameObject LightSpeedEnterShip;
    [SerializeField] private float introSpeed;
    [SerializeField] private float introTime;
    private bool entered;

    private void Update()
    {
        Vector3 self = Camera.main.WorldToViewportPoint(transform.position);
        if (self.x > 0 && self.x < 1 && self.y > 0 && self.y < 1 && !entered)
        {
            Invoke("LightSpeedEnter", stallTime);
            
            
        }
        else if (entered)
        {
            LightSpeedEnterShip.SetActive(true);
            transform.Translate(Vector3.forward * introSpeed * Time.deltaTime);
            introTime -= Time.deltaTime;
            if (introTime <= 0)
            {
                Destroy(this);
            }
        }
    }

    private void LightSpeedEnter()
    {
        entered = true;
    }
    
}
