using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class KoopaAI : MonoBehaviour
{
    public Animator animator; //animator to control the enemy's animations

    public Transform groundDetector; //Helps with ground detection (don't walk off the side of a cliff)
    public float distanceToGround = 2f; //Distance of our ray to the ground. Default 2
    private bool canIMove = true; //Variable helps determine if the enemy can move. Useful for not walking off a cliff
    private bool facingLeft = true; //Am I facing left?

    public Transform wallDetector; //Helps with wall detection (turn around when we bump into a wall)
    public float distanceToWall = 0.15f; //Distance of our ray to the wall. Default 0.15f

    public float patrolSpeed = 200f; //speed at which the enemy moves when idling

    public int damage; //How much damage an attack by this enemy does

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
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
}
