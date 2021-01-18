using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class enemyAI : MonoBehaviour
{
    public Transform target; //where we are pathfinding to

    public float enemyDetectionRange = 6.5f; //Distance before this enemy starts pathfinding to the player

    public Transform groundDetection; //Helps with ground detection (don't walk off the side of a cliff)
    public float distanceToGround = 2f; //Distance of our ray to the ground. Default 2
    private bool canIMove = true; //Variable helps determine if the enemy can move. Useful for not walking off a cliff
    private bool facingLeft = true; //Am I facing left?

    public float attackSpeed = 400f;
    public float patrolSpeed = 200f;
    public float nextWaypointDistance = 1f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("PlayerNew(Clone)").transform;

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void UpdatePath()
    {
        if(seeker.IsDone())
        {
            if(target == null) //If the player dies, we need to find them again to begin pathfinding
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
        //Enemy needs a walkin animation
    {
        if (Math.Abs(Vector3.Distance(target.transform.position, transform.position)) < enemyDetectionRange) //If in range, move towards the player
        {
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
                Vector2 force = direction * attackSpeed * Time.deltaTime; //force in our direction
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
        else
        {
            //idle or patrol movement for when the player isn't in range
            Vector2 force;
            if (facingLeft)
                force = new Vector2(-1,0) * patrolSpeed * Time.deltaTime; //force to the left
            else
                force = new Vector2(1, 0) * patrolSpeed * Time.deltaTime; //force to the right
            rb.AddForce(force); //adding force to the rigid body
            

            edgeDetection();
            if(!canIMove)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1f, 1f, 1f); //flip our direction if we're on a ledge
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
        if (!Physics2D.Raycast(groundDetection.position, Vector2.down, distanceToGround, LayerMask.GetMask("FloorsNWalls"))) //if we dont see ground ahead. Stop moving
        {
            if (canIMove) //only do the next line when we first turn off canIMove
                rb.velocity = new Vector2(0, rb.velocity.y); //Set velocity to zero to stop all horizontal momentum
            canIMove = false;

        }
        else
        {
            canIMove = true;
        }
    }
}
