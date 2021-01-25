using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class EnemyAI : MonoBehaviour
{
    public Transform target; //where we are pathfinding to

    public Animator animator; //animator to control the enemy's animations

    public float enemyDetectionRange = 6.5f; //Distance before this enemy starts pathfinding to the player
    public float enemyAttackRange = 1f; //Distance between the player and the enemy where the enemy executes an attack

    public Transform groundDetector; //Helps with ground detection (don't walk off the side of a cliff)
    public float distanceToGround = 2f; //Distance of our ray to the ground. Default 2
    private bool canIMove = true; //Variable helps determine if the enemy can move. Useful for not walking off a cliff
    private bool facingLeft = true; //Am I facing left?

    public Transform wallDetector; //Helps with wall detection (turn around when we bump into a wall)
    public float distanceToWall = 0.15f; //Distance of our ray to the wall. Default 0.15f

    public float attackSpeed = 400f; //speed at which the enemy moves when engaing the player
    public float patrolSpeed = 200f; //speed at which the enemy moves when idling
    public float nextWaypointDistance = 1f;

    //enemy's attack
    private float attackCooldownTracker; //Tracks the cooldown of when we can attack again
    public float attackCooldown; //The set cooldown of when this enemy can attack again
    public Transform attackPos; //child of the enemy which is the base of their hitbox
    public float hitboxRadius; //radius of the hitbox

    public int damage; //How much damage an attack by this enemy does

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false; //this variable is IMPORTANT shut the fuck up

    Seeker seeker;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("PlayerNew(Clone)").transform;

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.25f);
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            if (target == null) //If the player dies, we need to find them again to begin pathfinding
            {
                target = GameObject.Find("PlayerNew(Clone)").transform;
            }
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    public void Respawn() //method to update the target gameobject because we make a new player everytime they die
    {
        target = GameObject.Find("PlayerNew(Clone)").transform;
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    //Current Problems:
    //We constantly pathfind, but it should be possible to limit it to only pathfind when the player is in range
    //Create a wall detection gameobject so we can turn around when we bump into a wall
    //Work on animation transitions
    //Combine this script with the basic enemy script
    //Figure out how Mac's attack script works. His hitboxes might actually last for more than a frame.
    //Add the wall detection gameobject child

    {
        if (Math.Abs(Vector3.Distance(target.transform.position, transform.position)) < enemyAttackRange) //If in attack range, try to attack the player
        {
            //Maybe set canIMove to false during this time
            if (attackCooldownTracker <= 0)
            {
                //we can attack

                //throw up a collider to try to attack the player
                animator.SetTrigger("Attack");
                Collider2D player = Physics2D.OverlapCircle(attackPos.position, hitboxRadius, LayerMask.GetMask("Player"));
                //Tells the player they've been hit
                //Only works if the player is actually hit by the attack
                if (player != null)
                {
                    //player.GetComponent<PlayerCombat>().hitByEnemy(damage);
                }
                //Collider2D[] playerArray = Physics2D.OverlapCircleAll(attackPos.position, hitboxRadius, LayerMask.GetMask("Player"));
                //for (int i = 0; i < playerArray.Length; i++)
                //{
                //    playerArray[i].GetComponent<PlayerCombat>().hitByEnemy(damage);
                //}
                attackCooldownTracker = attackCooldown;
            }
            else
            {
                attackCooldownTracker -= Time.fixedDeltaTime;
            }

        }
        else if (Math.Abs(Vector3.Distance(target.transform.position, transform.position)) < enemyDetectionRange) //If in detect range, move towards the player
        {
            //UpdatePath();
            if (path == null)
                return;

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            if (canIMove) //If I can move, move towards the current waypoint
            {
                Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized; //direction to move in
                Vector2 force = direction * attackSpeed * Time.fixedDeltaTime; //force in our direction
                rb.AddForce(force); //adding force to the rigid body
                facing(force);
            }
            else //In order to not be stuck on a ledge, we need to turn around when our target is behind us
            {
                Vector2 relativePosition = transform.InverseTransformPoint(target.transform.position); //target postion compared to ours 
                if (!facingLeft && relativePosition.x > 0)//When the skeleton faces us, our relative position is always negative.
                {
                    transform.localScale = new Vector3(1f, 1f, 1f); //face left
                }
                else if (facingLeft && relativePosition.x > 0)
                {
                    transform.localScale = new Vector3(-1f, 1f, 1f); //face right
                }

            }

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            edgeDetection();

        }
        else //idle or patrol movement for when the player isn't in range
        {
            Vector2 force;
            if (facingLeft)
                force = new Vector2(-1, 0) * patrolSpeed * Time.fixedDeltaTime; //force to the left
            else
                force = new Vector2(1, 0) * patrolSpeed * Time.fixedDeltaTime; //force to the right
            rb.AddForce(force); //adding force to the rigid body

            edgeDetection();
            wallDetection();
            if (!canIMove)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1f, 1f, 1f); //flip our direction if we're on a ledge or at a wall
                canIMove = true; //reset canIMove to true
                facingLeft = !facingLeft;
            }
        }

    }

    void facing(Vector2 force) //which way should we face
    {
        //The enemy flips between directions due to the force being inconsistent sometimes.
        //To combat this, measure the enemy's velocity to help determine where the enemy should face
        if (force.x >= 0.01f && rb.velocity.x >= 0.005)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f); //face right
            facingLeft = false;
        }
        else if (force.x <= -0.01f && rb.velocity.x <= -0.005)
        {
            transform.localScale = new Vector3(1f, 1f, 1f); //face left
            facingLeft = true;
        }
    }

    void edgeDetection() //stops enemy from walking off the side of a cliff
    {
        if (!Physics2D.Raycast(groundDetector.position, Vector2.down, distanceToGround, LayerMask.GetMask("FloorsNWalls"))) //if we dont see ground ahead. Stop moving
        {
            if (canIMove) //only do the next line when we first turn off canIMove
                rb.velocity = new Vector2(0, rb.velocity.y); //Set velocity to zero to stop all horizontal momentum
            canIMove = false;
            animator.SetBool("Idle", true); //enter idle animation when on the ledge and unable to approach further

        }
        else
        {
            canIMove = true;
            animator.SetBool("Idle", false); //exit idle animation when we start moving again
        }
    }

    void wallDetection() //stops enemy from aimlessly walking into a wall forever
    {
        if (facingLeft)
        {
            if (Physics2D.Raycast(wallDetector.position, Vector2.left, distanceToWall, LayerMask.GetMask("FloorsNWalls")))
                canIMove = false; //If we hit a wall facing left, then face right.
        }
        else
        {
            if (Physics2D.Raycast(wallDetector.position, Vector2.right, distanceToWall, LayerMask.GetMask("FloorsNWalls")))
                canIMove = false; ; //If we hit a wall facing right, then face left.
        }
    }

    private void OnDrawGizmosSelected() //draws the hitbox for our attack
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, hitboxRadius);
    }
}
