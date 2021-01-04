using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/* TODO:
 * If the player crosses over the hitbox of the target we want to preserve the last angle from right before they cross over the target and use that as an angle to launch the player real high
 * It also kinda doesn't work with horizontal velocity for some reason
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

    private Vector2 angle;
    //public SpringJoint2D grapple;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            grapple = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0) && grapple)
        {
            grapple = false;
            wasGrapple = true;
        }
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
                    //Debug.Log(closestPoint.gameObject.name);
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

            angle = new Vector2(closestPoint.gameObject.transform.position.x - gameObject.transform.position.x, closestPoint.gameObject.transform.position.y - gameObject.transform.position.y);
            angle.Normalize();
            //If clicking then activate grapple
            if (grapple)
            {
                rb.AddForce(angle * 100);
            }
            //If the mouse was released early we're going to give the player a boost of force 
            if (wasGrapple)
            {
                rb.AddForce(angle * 500);
                wasGrapple = false;
            }
        }
        else
        {
            if (lastClosest != null)
            {
                //rb.AddForce(angle * 500);
                lastClosest.GetComponent<SpriteRenderer>().color = Color.white;
                lastClosest = null;
            }
            
            wasGrapple = false;
            grapple = false;
        }
    }
}
