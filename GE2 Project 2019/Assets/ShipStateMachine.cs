using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipStateMachine : MonoBehaviour
{
    private enum Behavior { idle, pursuit, dodge }
    private Behavior shipState = Behavior.idle;
    private Component[] behaviours;
    private List<Component> activeBehaviours = new List<Component>();
    private int idleBehaviourIndex;
    private List<GameObject> enemiesWithinRange;
    private Boid target;
    [SerializeField] private string enemyTag;
    [SerializeField] private Vector2 refreshTime;
    private bool refreshing;

    // Start is called before the first frame update
    void Start()
    {
        List<Component> behaviorsToAdd = new List<Component>();
        foreach(SteeringBehaviour behave in GetComponents<SteeringBehaviour>())
        {
            behaviorsToAdd.Add(behave);
            if(behaviorsToAdd[behaviorsToAdd.Count-1].GetComponent<SteeringBehaviour>().isActiveAndEnabled)
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

        


        switch (shipState)
        {
            case Behavior.dodge:
                break;
            case Behavior.pursuit:
                if (!refreshing)
                {

                    refreshing = true;
                    float timeTillRefresh = Random.Range(refreshTime.x, refreshTime.y);
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
        if (state == Behavior.dodge)
        {
            shipState = state;
        }
        else if (state == Behavior.pursuit && shipState != Behavior.dodge && shipState != state)
        {
            shipState = state;
            if (GetComponent<Pursue>())
            {
                GetComponent<Pursue>().target = target;
            }
            else if (GetComponent<Seek>())
            {
                GetComponent<Seek>().targetGameObject = target.gameObject;
            }
        }
        else if (shipState != Behavior.dodge && shipState != Behavior.pursuit)
        {
            shipState = state;
        }
    }

    private void DisableBehaviours()
    {
        foreach(Component behaviour in activeBehaviours)
        {
            behaviour.
        }
    }

    private void RefreshClosestEnemy()
    {
        Boid previousTarget = target;
        GameObject possibleTarget = target.gameObject;
        float distanceToEnemy = !possibleTarget ? Vector3.Distance(transform.position, possibleTarget. transform.position) : 0;
        foreach (GameObject enemy in enemiesWithinRange)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < distanceToEnemy && possibleTarget)
            {
                possibleTarget = enemy;
            }

            Boid possibleTargetBoid = possibleTarget.GetComponent<Boid>();
            if (target != possibleTargetBoid)
            {
                target = possibleTargetBoid;
                //CHANGE STEERINGBEHAVIOR TARGETS
                ChangeState(Behavior.pursuit);

                
            }
            refreshing = false;
        }
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == enemyTag && other.GetComponent<Boid>())
        {
            enemiesWithinRange.Add(other.gameObject);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == enemyTag && enemiesWithinRange.Contains(other.gameObject))
        {
            if (target == other.GetComponent<Boid>())
            {
                enemiesWithinRange.Remove(other.gameObject);
                target = null;
                RefreshClosestEnemy();
            }
            else
            {
                enemiesWithinRange.Remove(other.gameObject);
            }
            
        }
    }
}
