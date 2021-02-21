using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/* TODO:
 * Find more permanent control scheme for grappling hook and attacking
 * Create the swing?
 * Disable all player movement when grappling (They can currently dash and attack)
 */
public class GrappleHook : MonoBehaviour
{
    [SerializeField] private LayerMask grappleable;
    [SerializeField] private LayerMask grappleableInterupt;
    public Transform grapplePointUL; // Upper left corner of grapple hitbox
    public Transform grapplePointBR; // Bottom right corner of grapple hitbox
    public Rigidbody2D rb; // Ridgidbody of the player

    private Collider2D lastClosest; // The previous grappleable point that the player was closest to
    public PlayerController controller; // The PlayerController script. Used only to change the "grapple" boolean value to prevent player movement while grappling

    private bool grapple = false; // Detects the first time the player clicks wto grapple
    private bool wasGrapple = false; // Whether or not the player can grapple again
    private bool grappling = false; // Whether or not the player is graplped to a target

    private Vector2 angle; // The angle the player is moved at

    public int grappleVelo = 30; // Speed at which player grapples to target
    public int flingVelo = 22; // Speed at which player is launched upon reaching target

    public float flingTime; // The amount of time the player's control is stopped also how long horizontal force is added to the player
    private float myFlingTime; // Current time until the player can move again

    public LineRenderer lr;
    public GameObject emissionPoint;

    public ItemManager itemManager; // Reference to ItemManager script

    public bool preventGrapple = false;


    void Update()
    {
        //If the player clicks and isn't currenlty grappling then try to grapple
        if(itemManager.hasGrapplingHookPowerup && !preventGrapple)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) && !grappling && !wasGrapple)
            {
                grapple = true;
            }
        }

        //If the player's control is disabled then count down until its reenabled
        if (controller.grapple && myFlingTime > 0)
        {
            myFlingTime -= Time.deltaTime;
        }
        // Re enable player control
        else if(wasGrapple)
        {
            controller.grapple = false;
            wasGrapple = false;
            myFlingTime = 0;

        }
    }

    void FixedUpdate()
    {
        if (itemManager.hasGrapplingHookPowerup)
        {
            //Find all grapple points in the hitbox rectangle
            Collider2D[] coll = Physics2D.OverlapAreaAll(grapplePointUL.position, grapplePointBR.position, grappleable);
            List<Collider2D> colli = new List<Collider2D>();

            // See if the line of sight to them is obstructed by an object
            for (int i = 0; i < coll.Length; i++)
            {
                Vector2 raycastAngle = new Vector2(coll[i].gameObject.transform.position.x - gameObject.transform.position.x, coll[i].gameObject.transform.position.y - gameObject.transform.position.y);
                RaycastHit2D hitWall = Physics2D.Raycast(transform.position, raycastAngle, 20, grappleableInterupt);
                RaycastHit2D hitPoint = Physics2D.Raycast(transform.position, raycastAngle, Mathf.Infinity, grappleable);
                //If we don't hit a wall or we hit the target before we hit the wall then we can grapple to the target
                if (hitWall.collider == null || hitPoint.distance <= hitWall.distance)
                {
                    colli.Add(coll[i]);
                }
            }

            Collider2D[] colliders = colli.ToArray();

            //Iterate over them and find the closest target
            if (colliders.Length > 0)
            {
                Collider2D closestPoint = colliders[0];
                double minDistance = 1000;
                for (int i = 0; i < colliders.Length; i++)
                {
                    double distance = Math.Sqrt(Math.Pow(colliders[i].gameObject.transform.position.x - gameObject.transform.position.x, 2) + Math.Pow(colliders[i].gameObject.transform.position.y - gameObject.transform.position.y, 2));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestPoint = colliders[i];
                    }
                }

                //Highlight the closest target and dehighlight the previous closest target if that applies
                if (lastClosest == null)
                {
                    lastClosest = closestPoint;
                }
                else if (lastClosest != closestPoint)
                {
                    lastClosest.GetComponent<SpriteRenderer>().color = Color.white;
                    lastClosest = closestPoint;
                }
                closestPoint.GetComponent<SpriteRenderer>().color = Color.yellow;

                float dis = 0f;
                //This statement is only called the first time that the player grapples and sets the direction the player will move and removes control from the player for a bit
                if (grapple)
                {
                    Transform point = closestPoint.gameObject.transform.Find("GrapplePoint");
                    angle = new Vector2(point.position.x - gameObject.transform.position.x, point.position.y - gameObject.transform.position.y);
                    dis = angle.magnitude;
                    angle.Normalize();

                    controller.grapple = true;
                    rb.gravityScale = 0f;

                    grapple = false;
                    grappling = true;

                    lr.positionCount = 2;
                    lr.SetPosition(0, emissionPoint.transform.position);
                    lr.SetPosition(1, point.position);

                    controller.makeHeavy();
                }

                //If the player hasn't collided with the target we move them towards it at a speed of grappleVelo
                if (grappling)
                {
                    rb.velocity = angle * grappleVelo;
                    lr.SetPosition(0, emissionPoint.transform.position);
                }

            }
            // If there are no targets in range don't highlight any of them
            else
            {
                if (lastClosest != null)
                {
                    lastClosest.GetComponent<SpriteRenderer>().color = Color.white;
                    lastClosest = null;
                }
                grapple = false;
            }
        }
    }

    // When the player enters a target (any of them rn not the one you specifically grapple too oops) then we give them one last boost of force in the direction they were going before
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Grappleable") && grappling && !wasGrapple)
        {
            controller.grapple = true;
            myFlingTime = flingTime;
            rb.gravityScale = controller.gravityStore; //This sets the gravity to the scene gravity and not the player's specific fall gravity that kicks in after flingTime ends
            rb.velocity = angle * flingVelo;
            wasGrapple = true;
            grappling = false;
            lr.positionCount = 0;
        }

        //This code was used to disable a grapple if the player falls into a death box but I make that call somewhere else now
        /*else if (!col.gameObject.CompareTag("Grappleable") && grappling && !wasGrapple)
        {
            controller.grapple = false;
            rb.gravityScale = controller.gravityStore; //This sets the gravity to the scene gravity and not the player's specific fall gravity that kicks in after flingTime ends
            wasGrapple = false;
            grappling = false;
        }*/

        
    }

    // This prevents the player from firing their grapple again when inside a target which would have werid effects, might not be necassary if we disable grapple points after the player uses them
    //You occasionaly lose the power to grapple with this code for some reason but it might be fixed now idk -Bren
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Grappleable") && !wasGrapple)
        {
            wasGrapple = false;
            myFlingTime = flingTime;
            lr.positionCount = 0;
        }
    }

    // Cancels Grapple if you grapple into something
    private void OnCollisionEnter2D(Collision2D col)
    {
        if(grappling)
        {
            controller.grapple = false;
            rb.gravityScale = controller.gravityStore; //This sets the gravity to the scene gravity and not the player's specific fall gravity that kicks in after flingTime ends
            wasGrapple = false;
            grappling = false;
            lr.positionCount = 0;
        }
    }

    // Cancels grapple if you die
    public void cancelGrapple()
    {
        if (grappling)
        {
            controller.grapple = false;
            rb.gravityScale = controller.gravityStore; //This sets the gravity to the scene gravity and not the player's specific fall gravity that kicks in after flingTime ends
            wasGrapple = false;
            grappling = false;
            lr.positionCount = 0;
        }
    }
}
