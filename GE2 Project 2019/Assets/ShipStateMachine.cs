using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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
    private GameObject targetObject;
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
                if (!targetObject)
                {
                    print("Target Lost");
                    target = null;
                    ChangeState(Behavior.dodge);
                    enemiesWithinRange.Remove(targetObject);
                    bool noEnemiesLeft = false;
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

                if (!refreshing)
                {

                    refreshing = true;
                    float timeTillRefresh = UnityEngine.Random.Range(refreshTime.x, refreshTime.y);
                    Invoke("RefreshClosestEnemy", timeTillRefresh);
                }
                break;
            case Behavior.pursuit:
                
                if (!targetObject)
                {
                    print("Target Lost");
                    target = null;
                    ChangeState(Behavior.pursuit);
                    enemiesWithinRange.Remove(targetObject);
                    bool noEnemiesLeft = false;
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
            if (GetComponent<Flee>())
            {
                GetComponent<Flee>().enabled = true;
                GetComponent<Flee>().targetGameObject = target?.gameObject;
            }
            if (GetComponent<Spin>())
            {
                GetComponent<Spin>().enabled = true;
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
            else if (GetComponent<AccurateShooting>())
            {
                GetComponent<AccurateShooting>().enabled = true;
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
        refreshing = false;
        //Check if target is dead
        if (target?.GetComponent<ShipHealth>()?.healthPoints <= 0)
        {
            enemiesWithinRange.Remove(target.gameObject);
            if (enemiesWithinRange.Count == 0)
            {
                ChangeState(Behavior.idle);
            }
            return;
        }

        enemiesWithinRange = enemiesWithinRange.Where(item => item != null).ToList();
        
        GameObject possibleTarget = target?.gameObject;
        GameObject enemyBehind = target?.gameObject;
        float distanceToEnemy = possibleTarget ? Vector3.Distance(transform.position, possibleTarget.transform.position) : 0;
        bool changeToDodge = false;

        foreach (GameObject enemy in enemiesWithinRange)
        {
            
            Vector3 directionToEnemy = Vector3.Normalize(enemy.transform.position - transform.position);
            float dot = Vector3.Dot(directionToEnemy, transform.forward);
            if (Mathf.Round(dot) == -1)
            {
                if (shipState != Behavior.dodge)
                {
                    enemyBehind = enemy;
                    distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                    changeToDodge = true;
                }
                else if (shipState == Behavior.dodge && Vector3.Distance(transform.position, enemy.transform.position) < distanceToEnemy)
                {
                    enemyBehind = enemy;
                    distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                }
                continue;
            }

            if (Vector3.Distance(transform.position, enemy.transform.position) < distanceToEnemy || !possibleTarget)
            {
                possibleTarget = enemy;
                distanceToEnemy = Vector3.Distance(transform.position, possibleTarget.transform.position);
            }
        }

        
        if (shipState == Behavior.dodge || changeToDodge)
        {
            if (target != enemyBehind.GetComponent<Boid>())
            {
                target = enemyBehind.GetComponent<Boid>();
                targetObject = target.gameObject;
            }
            if (changeToDodge)
                ChangeState(Behavior.dodge);
            return;
        }


        Boid possibleTargetBoid = possibleTarget?.GetComponent<Boid>();
        if (target != possibleTargetBoid)
        {

            target = possibleTargetBoid;
            targetObject = target.gameObject;
            if (GetComponent<Pursue>())
            {
                GetComponent<Pursue>().target = target;
            }
            else if (GetComponent<Seek>())
            {
                GetComponent<Seek>().targetGameObject = target.gameObject;
            }
            //CHANGE STEERINGBEHAVIOR TARGETS
            if (shipState != Behavior.pursuit)
            ChangeState(Behavior.pursuit);

        }
                
        
        
        
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == enemyTag && other.GetComponent<Boid>())
        {
            enemiesWithinRange.Add(other.gameObject);
            if (enemiesWithinRange.Count > 0)
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
