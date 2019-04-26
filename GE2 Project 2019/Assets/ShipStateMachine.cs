using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShipStateMachine : MonoBehaviour
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public string State;
    private enum Behavior { idle, pursuit, dodge }
    private Behavior shipState = Behavior.idle;
    public MonoBehaviour[] behaviours;
    private int idleBehaviourIndex;
    public List<GameObject> enemiesWithinRange;
    private Boid target;
    [SerializeField] private string enemyTag;
    [SerializeField] private Vector2 refreshTime;
    private bool refreshing;

    // Start is called before the first frame update
    void Start()
    {
        List<MonoBehaviour> behaviorsToAdd = new List<MonoBehaviour>();
        foreach(SteeringBehaviour behave in GetComponents<SteeringBehaviour>())
        {
            behaviorsToAdd.Add(behave);
            if(behave.idle)
            {
                idleBehaviourIndex = behaviorsToAdd.Count - 1;
            }
        }
        foreach (ShootingBehavior behave in GetComponents<ShootingBehavior>())
        {
            behaviorsToAdd.Add(behave);
        }
        behaviours = behaviorsToAdd.ToArray();
        
    }

    // Update is called once per frame
    void Update()
    {
        State = Enum.GetName(typeof(Behavior), shipState);

        switch (shipState)
        {
            case Behavior.dodge:
                break;
            case Behavior.pursuit:
                if (!refreshing)
                {

                    refreshing = true;
                    float timeTillRefresh = UnityEngine.Random.Range(refreshTime.x, refreshTime.y);
                    Invoke("RefreshClosestEnemy", timeTillRefresh);
                }
                break;
            case Behavior.idle:
                break;
            default:
                shipState = Behavior.idle;
                break;
        }

    }

    private void ChangeState(Behavior state)
    {
        if (state == Behavior.dodge && shipState != state)
        {
            DisableBehaviours();
            shipState = Behavior.dodge;
            if (GetComponent<Zigzag>())
            {
                GetComponent<Zigzag>().enabled = true;
            }
            
        }
        else if (state == Behavior.pursuit && shipState != Behavior.dodge && shipState != state)
        {
            
            DisableBehaviours();
            shipState = Behavior.pursuit;
            if (GetComponent<Pursue>())
            {
                GetComponent<Pursue>().target = target;
                GetComponent<Pursue>().enabled = true;
            }
            else if (GetComponent<Seek>())
            {
                GetComponent<Seek>().targetGameObject = target.gameObject;
                GetComponent<Seek>().enabled = true;
            }

            if (GetComponent<Missile>())
            {
                GetComponent<Missile>().enabled = true;
            }
        }
        else if (enemiesWithinRange.Count < 1 && shipState != state)
        {
            DisableBehaviours();
            shipState = Behavior.idle;
            behaviours[idleBehaviourIndex].enabled = true;
            print(behaviours[idleBehaviourIndex]);
        }
    }

    private void DisableBehaviours()
    {
        foreach(MonoBehaviour behaviour in behaviours)
        {
            behaviour.enabled = false;
        }
    }

    private void RefreshClosestEnemy()
    {
        //Check if target is dead
        if (target?.GetComponent<ShipHealth>().healthPoints <= 0)
        {
            enemiesWithinRange.Remove(target.gameObject);
            if (enemiesWithinRange.Count == 0)
            {
                ChangeState(Behavior.idle);
            }
            return;
        }

        Boid previousTarget = target;
        GameObject possibleTarget = target?.gameObject;
        
        float distanceToEnemy = possibleTarget ? Vector3.Distance(transform.position, possibleTarget.transform.position) : 0;
        foreach (GameObject enemy in enemiesWithinRange)
        {
            if (!possibleTarget)
            {
                possibleTarget = enemy;
                distanceToEnemy = Vector3.Distance(transform.position, possibleTarget.transform.position);
            }
            else if (Vector3.Distance(transform.position, enemy.transform.position) < distanceToEnemy)
            {
                possibleTarget = enemy;
                distanceToEnemy = Vector3.Distance(transform.position, possibleTarget.transform.position);
            }
        }

        Boid possibleTargetBoid = possibleTarget?.GetComponent<Boid>();
        if (target != possibleTargetBoid)
        {

            target = possibleTargetBoid;
            //CHANGE STEERINGBEHAVIOR TARGETS
            if (shipState != Behavior.pursuit)
            ChangeState(Behavior.pursuit);

        }
                
            
        refreshing = false;
        
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == enemyTag && other.GetComponent<Boid>())
        {
            enemiesWithinRange.Add(other.gameObject);
            if (enemiesWithinRange.Count >= 0)
            {
                RefreshClosestEnemy();
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == enemyTag && enemiesWithinRange.Contains(other.gameObject))
        {

            bool noEnemiesLeft = false;
            if (target == other.GetComponent<Boid>())
            {
                target = null;
                
            }

            enemiesWithinRange.Remove(other.gameObject);
            if (enemiesWithinRange.Count <= 0)
            {
                ChangeState(Behavior.idle);
                noEnemiesLeft = true;
            }

            if (!noEnemiesLeft)
            {
                RefreshClosestEnemy();
            }

        }
    }
}
