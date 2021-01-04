using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GrappleHook : MonoBehaviour
{
    [SerializeField] private LayerMask grappleable;
    public Transform grapplePointUL;
    public Transform grapplePointBR;
    public Rigidbody2D rb;

    private Collider2D lastClosest;
    public 
    //public SpringJoint2D grapple;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

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

            Vector2 angle = new Vector2(closestPoint.gameObject.transform.position.x - gameObject.transform.position.x, closestPoint.gameObject.transform.position.y - gameObject.transform.position.y);//Vector2.Distance(closestPoint.gameObject.transform.position, gameObject.transform.position);
            angle.Normalize();
            //If click then activate grapple
            if (Input.GetKey(KeyCode.Mouse0))
            {
                //Debug.Log("Here");
                rb.AddForce(angle * 100);
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                Debug.Log("Here");
                rb.AddForce(angle * 1000);
            }
        }
        else
        {
            if (lastClosest != null)
            {
                lastClosest.GetComponent<SpriteRenderer>().color = Color.white;
                lastClosest = null;
            }
        }
    }
}
