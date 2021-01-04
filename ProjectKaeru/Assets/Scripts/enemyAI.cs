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
    private bool movingRight = true; //Determines if we are moving right
    public float distanceToGround = 2; //Distance of our ray to the ground. Default 2

    public float speed = 250f;
    public float nextWaypointDistance = 3f;

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
            if(target == null)
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
    {
        if(Math.Abs(Vector3.Distance(target.transform.position, transform.position)) < enemyDetectionRange) //If in range, move in
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

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized; //direction to move in
            Vector2 force = direction * speed * Time.deltaTime; //force in our direction

            rb.AddForce(force); //adding force to the rigid body
            
            //Debug.Log(groundInfo.collider);
            if (rb.velocity.x > 1.5) //velocity is capped at 1.5 and -1.5 respectively
            {
                rb.velocity = new Vector2(1.5f, rb.velocity.y);
            }
            else if (rb.velocity.x < -1.5)
            {
                rb.velocity = new Vector2(-1.5f, rb.velocity.y);
            }

            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceToGround);//Ground detection variable

            if (groundInfo.collider == false)//this could prevent enemies from being knocked off if they are moving in the direction of the cliff
            {
                Debug.Log("velocity = " + rb.velocity.x);
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            if (force.x >= 0.01f) //flip to face the direction we are moving in
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (force.x <= -0.01f)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        //EDIT THIS TO WORK WITH WHAT I'VE ALREADY WRITTEN
        /*
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceToGround);
        if (groundInfo.collider == false)
        {
            if (movingRight == true)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                movingRight = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                movingRight = true;
            }
        }
        */
    }
}
