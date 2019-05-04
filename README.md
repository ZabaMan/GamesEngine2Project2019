# Games Engine 2 Project 2019

Name: Aaron Hamilton

Student Number: C16376101

# Description of the assignment

The project is quite closer to a story representation than a battle simulation. I have the groundworks to pit many spaceships against one another but instead tried to stick to the movie sequence as close as I could while showing off behaviors too. 
I created a finite state machine with three states, idle, pursuit and dodge. I made three camera behaviours, an abstract shooting behavior class for three shooting types, charged lasers, accurate lasers and missiles. I first thought I'd have a multiple shooting behaviors on ships and especially the frigates, but that didn't make the final project. 

There are a lot of changes from my initial story board (seen below). Some scenes were chopped in half while others were removed. The final scenes I couldn't pull off unfortunately. Partly due to the missiles in Star Wars Episode III just being unrealistically stupid. When Anakin spins his ship, they follow his spin, which in turn causes the missiles to spiral into one another. However even with the missiles seeking/pursuing two transform points on each side of the spinning ship, they still ignored the spinning and continued on the fastest route to Anakin. I was considering having Anakin spawn path points for the missiles to pathFollow but then I realised the cool gimmick couldn't be pulled off without having some silly, unnatural settup. 
I then tried putting Buzz droids on Obi-Wan's ship (little droids that stick to ships and cut into the wires to take out the controls). However, Anakin couldn't land a clean hit on them without hitting Obi-Wan for considerably long. Putting more than 2 on Obi-Wan would take very long. So I decided to scrap the Buzz Droids cause only having 1 or 2 was not worth setting up a whole special missile behavior. In the end, I stuck to a simple missile following Obi-Wan and Anakin shooting it down before it hits his master. 

# How it works

The finite state machine:
I created a finite state machine with three states, idle, pursuit and dodge. I added an Idle bool to the SteeringBehavior class so behaviors can be set as the idle behavior. From here, if an enemy comes within range (sphere trigger), they are checked for their position. If they are in front of the AI, they will enter the Pursuit State, they will be targetted with seek/pursue and a shooting behavior will be turned on. If they are behind the AI, a Dodge State, where flee/zigzag/jitterWander is activated. They constantly update a list of close by enemies, trying to determine who is closest while always prioritising dodging/fleeing.

The ship health:
A typical collision checker. It notably has a force applier on death however. When ships die, force is applied on the "heaviest" part which is pre-setup with a public Vector3. This allows for them to realistically sink towards Coruscants gravity with their centre of gravity tipping first.

___

### Scene 1
Scene 1 contains Obi-Wan and Anakin flying over the space battle as they enter their mission. Frigates in the background can be seen lightspeeding in and below are many lasers as the Republican and Seperatist forces in the Battle Over Coruscant.
The biggest visible issue in the whole project is within this scene, as the camera jitters when it tries to keep up with the Jedis. I thought it was down to frame rate for a long time, and it partially was. I improved it by putting its code within LateUpdate, yet it still persists. 

CameraKeepBackgroundObjectsInView Behavior:
```C#
private void LateUpdate()
    {
            transform.LookAt(target);
            // Get the distance between the target and the background object that is to be kept on screen
            float dist = Vector3.Distance(target.position, keepInView[0].position); 
            // Get a point that is an extension of the closest line between these 2 objects
            Vector3 extendedTarget = (target.position - keepInView[0].position).normalized * (dist + scalar); 
            // Move to this point, always in position so the camera can see the target and a background object
            transform.position = Vector3.Lerp(transform.position, keepInView[0].position + extendedTarget, Time.deltaTime*1000); 
    }
```

### Scene 2

Scene 2 contains a new behavior and the entry of the ship state machine. The behavior is a zigzag steering behavior. It is set with an angle, which it flips on time depending on the set frequency. It is a less smooth harmonic motion. I needed movement that could geniunely dodge following ships. 

ZigZag Behavior:
```C#
public override Vector3 Calculate()
    {
        if (!flipping)
        {
            StartCoroutine("turnZigToZag");
            flipping = true;
        }

        target = Vector3.forward * distance;
        target.x = angle * flip;
        return boid.SeekForce(transform.position + target);
    }

    private IEnumerator turnZigToZag()
    {
        yield return new WaitForSeconds(frequency);
        flip *= -1;
        flipping = false;
    }
```

### Scene 3

Scene 3 is when the state machine can be seen well. The X-Wing is in "Flee/Dodge" state while the enemy TriFighter is in the pursuit state. Anakin too enters the pursuit state after the TriFighter and shoots him down. Once killed, he returns to following Obi-Wan, flying off into the distance. 
The camera for this scene has a "LookAtKiller" behavior, which looks at whoever just killed its current target. It begins looking at the TriFighter and finishes watching Anakin fly off.

LookAtKiller Behavior (It notably plants a tiny component on ships which is like a tracker to see if they died): 
```C# 
    void Start()
    {
        target = initialTarget.transform;
        initialTarget.gameObject.AddComponent<CameraAttention>().camera = gameObject.GetComponent<Camera>();

    }

    void Update()
    {
        transform.LookAt(target);
    }

    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
        target.gameObject.AddComponent<CameraAttention>().camera = gameObject.GetComponent<Camera>();
    }
```

### Scene 4

Scene 4 has a change to the offset pursue shown, where ships move to an external transform position, allowing for a nice team up visual. In the background, two frigates can be seen shooting at eachother till a Republican cruiser is blown up.

### Scene 5

Scene 5 has a bunch of enemy droids come up behind the X-Wings and shoot them down. They are all in the pursue state while the X-Wings immediately try to flee. They're all blown up and the droids fly off. 

Accurate Shooting Behavior:
```C# 
public class AccurateShooting : ShootingBehavior
{
    [SerializeField] private float accuracySpherCastWidth = 500;
    private int spawnPos = 0;
    [SerializeField] private float timeBetweenSpawn;
    private bool nextSpawn = true;
    [SerializeField] private GameObject targetGameObject;
    AudioSource audioSource;
    

    private void Start()
    {
        if(GetComponent<AudioSource>())
        audioSource = GetComponent<AudioSource>();
    }

    public override void Calculate()
    {
        //Casts a sphereCast infront in search of the enemy it's pursuing/seeking
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 100000, Color.green);
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, accuracySpherCastWidth, fwd, out hit) && hit.transform.tag == "Bad")
        {
            if (canShoot && nextSpawn)
            {
                Instantiate(projectile, projectileSpawns[spawnPos].position, transform.rotation);
                audioSource?.Play();
                
                projectile.GetComponent<MoveForward>().shotFrom = gameObject; // In case the enemy has the camera's attention
                
                spawnPos++; // Ships can have an array of guns, which they shoot from in sequence or all at once
                if (spawnPos >= projectileSpawns.Length)
                {
                    canShoot = false;
                    if (reloadTime > 0)
                        Invoke("CanShoot", reloadTime);
                }
                else
                {
                    nextSpawn = false;
                    Invoke("NextSpawn", timeBetweenSpawn);
                }
            }
        }
    }

    private void NextSpawn()
    {
        nextSpawn = true;
    }

    private void CanShoot()
    {
        canShoot = true;
        spawnPos = 0;
    }
}
```

### Scene 6

Scene 6 has Anakin notice a missile following them. He enters flee state and banks right, but the missile is locked onto Obi-Wan, so he follows up behind it, enters attack state, and shoots it down. Then return to idle state and goes back to pursuing his offset to Obi-Wan. 
As Anakin was actually driving into missiles more often than shooting them down (good ol' Anakin), I had to add a slow down to the pursue behavior. If the boid gets too close to it's target, it will slow down till the distance is safe again.

# What I am most proud of in the assignment

I am most proud of my Finite State Machine. It can keep the boid realistically going in between attacking, fleeing and idle behaviors. Although I didn't use it for such a case, it can do dogfights of multiple ships, all attacking one another whilst fleeing too. 
What I am more proud of is my ability to actually create AI now. I learned a lot, especially with the attached behavior component method, and feel a lot more confident in my ability to create games. AI was always a fear of mine but now that I know a proper approach, I'm capable of tackling the problems a lot more efficiently than I could have pre-module. 

<details>
<summary>Finite State Machine Code</summary>
<br>
  
```C#
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

    void Update()
    {
        State = Enum.GetName(typeof(Behavior), shipState); // Just for debugging, gives name to Inspector

        switch (shipState)
        {
            case Behavior.dodge:
                if (!targetObject) // Used for when the target is destroyed
                {
                    print("Target Lost");
                    target = null;
                    ChangeState(Behavior.dodge); // Refreshes the behavior
                    enemiesWithinRange.Remove(targetObject);
                    bool noEnemiesLeft = false; // The following is a check if there is anyone else in the enemy list
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

                if (!refreshing) //Refreshes who's closest and if anyones dead in the enemy list
                {

                    refreshing = true;
                    float timeTillRefresh = UnityEngine.Random.Range(refreshTime.x, refreshTime.y); // I used a Vector2 as a min max field
                    Invoke("RefreshClosestEnemy", timeTillRefresh);
                }
                break;
            case Behavior.pursuit: // Pretty much the same as a dodge
                
                if (!targetObject)  // Used for when the target is destroyed
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

    private void ChangeState(Behavior state) //Checks if the state is already active, if not, disables all behaviors and applies the new 
                                             //states behaviors. 
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

        enemiesWithinRange = enemiesWithinRange.Where(item => item != null).ToList(); //Also clears out destroyed enemies
        
        GameObject possibleTarget = target?.gameObject; //Takes last target as the likely new target
        GameObject enemyBehind = target?.gameObject; //Same but for enemy behind boid
        float distanceToEnemy = possibleTarget ? Vector3.Distance(transform.position, possibleTarget.transform.position) : 0;
        bool dodgeTime = false;

        foreach (GameObject enemy in enemiesWithinRange)
        {
            //A check for the enemies position to the player, if its behind then it's dodge time
            Vector3 directionToEnemy = Vector3.Normalize(enemy.transform.position - transform.position);
            float dot = Vector3.Dot(directionToEnemy, transform.forward);
            if (Mathf.Round(dot) == -1)
            {
                if (shipState != Behavior.dodge)
                {
                    enemyBehind = enemy;
                    distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                    dodgeTime = true;
                }
                //a check to make sure it chooses the closest enemy
                else if (shipState == Behavior.dodge && Vector3.Distance(transform.position, enemy.transform.position) < distanceToEnemy)
                {
                    enemyBehind = enemy;
                    distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                    dodgeTime = true;
                }
            }

            if (dodgeTime) // Doesn't check enemies infront anymore
            {
                continue;
            }

            if (Vector3.Distance(transform.position, enemy.transform.position) < distanceToEnemy || !possibleTarget)
            {
                possibleTarget = enemy;
                distanceToEnemy = Vector3.Distance(transform.position, possibleTarget.transform.position);
            }
        }

        
        if (dodgeTime)
        {
            if (target != enemyBehind.GetComponent<Boid>())
            {
                target = enemyBehind.GetComponent<Boid>();
                targetObject = target.gameObject;
            }
            if (shipState != Behavior.dodge)
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
            
            if (shipState != Behavior.pursuit)
            ChangeState(Behavior.pursuit);

        }
    }

    

    private void OnTriggerEnter(Collider other) // Adds enemies to a list then refreshes to see if the new enemy is closer
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

    private void OnTriggerExit(Collider other) // Checks if the enemy exiting was the target, then refreshes or goes into idle depending
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
```
</details>

# Initial Story Board

![S1](https://i.imgur.com/myyhfVP.jpg)

## Scene 1

Jedi ships fly through the battle, past their own gunships, before gliding over a frigate and then swooping down to a deeper level within the ship.
Jedi ships will use a follow behavior, the second Jedi ship will offset the first.
Camera will lerp beside the first ship and also make sure to keep both the frigate and Jedi ship in frame at all times.

![S2](https://i.imgur.com/LZS6SBr.jpg)

## Scene 2

Enemy ship is shooting at X-Wing ally ship and chasing.
X-Wing is using flee and jitter to dodge.
Enemy ship is using seek and shoot when ally is close to being in line with guns.
Jedis fly by and shoot down enemy ship.
Camera follows X-Wing ally + Enemy (point in-between) till the enemy is shot down, in which it stops and lets the Jedi’s exit frame.

![S3](https://i.imgur.com/ThUwoQV.jpg)

## Scene 3

Jedi’s pass a group of ally X-Wings who then decide to support them and group up.
Multiple X-Wings organise behind the two Jedis, creating a formation. All are using offset pursue. 
The camera follows the Jedi’s but keeps the X-Wings in frame always.

![S4](https://i.imgur.com/VEc9dr7.jpg)

## Scene 4

Enemy’s swoop behind the X-Wing formation and take out a few.
Most are shot down with lasers and missiles.
Jedis flee from the missiles.
Camera watches the enemy’s and when the X-Wings begin getting shot, it goes to a new view of them

![S5](https://i.imgur.com/T3TBmFW.jpg)

## Scene 5

Jedi 2 spins on around his forward axis, causing the two following missiles to close their distance together before exploding. 
Jedi 1 pulls up completely and does a loop, to no avail.
Camera will follow Jedi 2 first, then Jedi 1.

![S6](https://i.imgur.com/TpelIa8.jpg)

## Scene 6

The rocket misses Jedi 1 barely, as he pulls the brakes on the ships thrusters, the missile passes in front of him yet opens,  releasing buzz droids. Jedi 1 moves into them as he is already in that direction, and the buzz droids attach to his ship. 
Jedi 2 shoots them off with clean shots from a side angle. 
