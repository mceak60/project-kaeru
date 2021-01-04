using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GrappleHook : MonoBehaviour
{
    [SerializeField] private LayerMask grappleable;
    public Transform grapplePointUL;
    public Transform grapplePointBR;
    public SpringJoint2D grapple;
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

            //Highlight closest target

            //If click then activate grapple
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Debug.Log("Here");
                grapple.connectedBody = closestPoint.gameObject.GetComponent<Rigidbody2D>();
            }
        }
    }
}
