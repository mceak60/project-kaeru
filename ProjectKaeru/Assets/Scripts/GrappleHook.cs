using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/* TODO:
 * If the player crosses over the hitbox of the target we want to preserve the last angle from right before they cross over the target and use that as an angle to launch the player real high
 * It also kinda doesn't work with horizontal velocity because the player movement overwrites it
 */
public class GrappleHook : MonoBehaviour
{
    [SerializeField] private LayerMask grappleable;
    public Transform grapplePointUL;
    public Transform grapplePointBR;
    public Rigidbody2D rb;

    private Collider2D lastClosest;
    public PlayerController controller;

    private bool grapple = false;
    private bool wasGrapple = false;
    private bool grappling = false;

    private Vector2 angle;

    public float flingTime;
    private float myFlingTime;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !grappling && !wasGrapple)
        {
            grapple = true;
        }

        if (controller.busy && myFlingTime > 0)
        {
            myFlingTime -= Time.deltaTime;
        }
        else 
        {
            controller.busy = false;
            wasGrapple = false;
            myFlingTime = 0;

        }
        /*if (Input.GetKeyUp(KeyCode.Mouse0) && grapple)
        {
            //grapple = false;
            //wasGrapple = true;
        }*/
    }

    void FixedUpdate()
    {
        Collider2D[] colliders = Physics2D.OverlapAreaAll(grapplePointUL.position, grapplePointBR.position, grappleable);

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

            if (grapple)
            {
                angle = new Vector2(closestPoint.gameObject.transform.position.x - gameObject.transform.position.x, closestPoint.gameObject.transform.position.y - gameObject.transform.position.y);
                dis = angle.magnitude;
                angle.Normalize();
                controller.busy = true;
                rb.gravityScale = 0f;
                grapple = false;
                grappling = true;
                //wasGrapple = false;
            }
            //If clicking then activate grapple
            if (grappling)
            {
                 rb.velocity = angle * 30;   
            }

            //If the mouse was released early we're going to give the player a boost of force 
            /*if (wasGrapple)
            {
                controller.busy = false;
                rb.gravityScale = controller.gravityStore;
                //rb.AddForce(angle * 500);
                wasGrapple = false;
            }*/
        }
        else
        {
            if (lastClosest != null)
            {
                lastClosest.GetComponent<SpriteRenderer>().color = Color.white;
                lastClosest = null;
            }
            
            wasGrapple = false;
            grapple = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Grappleable") && grappling && !wasGrapple)
        {
            controller.busy = true;
            myFlingTime = flingTime;
            rb.gravityScale = controller.gravityStore;
            rb.velocity = angle * 25;
            wasGrapple = true;
            grappling = false;
        }
    }


    //You occasionaly lose the power to grapple with this code for some reason -Bren
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Grappleable") && !wasGrapple)
        {
            wasGrapple = false;
            myFlingTime = flingTime;
        }
    }
}
