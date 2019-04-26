using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipHealth : MonoBehaviour
{
    Rigidbody rb;
    public int healthPoints;
    [SerializeField] private float destroyAfter;
    [Tooltip("Tag of enemy bullet")] [SerializeField] private string enemyTag;
    [SerializeField] private bool explosionOnHit;
    [SerializeField] private int explosionSize;
    [Range(0, 5)] [SerializeField] private float timeForceApplied;
    [Header("Point where death force is applied")]
    [SerializeField] private Vector3 heaviestPoint;
    [SerializeField] private float force;
    [SerializeField] private bool test;
    [SerializeField] private GameObject explosion;
    [SerializeField] private int gizmoSize = 100;
    private bool changedTarget = false;

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the heaviest point's position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(heaviestPoint, 10 * gizmoSize);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (test)
        {
            StartCoroutine("forceOverTime");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == enemyTag)
        {
            if(explosionOnHit)
            {
                if (healthPoints-- <= 0 && GetComponent<CameraAttention>() && !changedTarget)
                {
                    GetComponent<CameraAttention>().TellCameraObjectIsDead(other.GetComponent<MoveForward>().shotFrom);
                    changedTarget = true;
                }
                healthPoints--;
                GameObject explosionSpawned = Instantiate(explosion, other.transform.position, Quaternion.identity, transform) as GameObject;
                explosionSpawned.transform.localScale = new Vector3(explosionSize, explosionSize, explosionSize);
                Destroy(other.gameObject);
                Destroy(explosionSpawned, 1);
                Destroy(gameObject, destroyAfter);

                if (healthPoints <= 0)
                {
                    StartCoroutine("forceOverTime");
                    GetComponent<Boid>().maxSpeed = 0;
                }
            }
            else
            {
                explosion.SetActive(true);
                StartCoroutine("forceOverTime");
            }
        }
    }

    

    private IEnumerator forceOverTime()
    {
        float t = Time.time + timeForceApplied;
        do
        {
            ApplyForce();
            yield return null;
        } while (Time.time < t);
    }

    void ApplyForce()
    {
        
        rb.AddForceAtPosition(Vector3.down*force, heaviestPoint);
    }
}
