using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private GameObject platform; // The gameObject that will move
    private Transform point1;
    private Transform point2; // First point the platform will move to
    private Rigidbody2D rb;
    private Transform currPoint;
    public float speed; // Speed the platform will move at
    // Start is called before the first frame update
    void Start()
    {
        platform = transform.GetChild(0).gameObject; // The first child is the object that you want to move
        point1 = transform.GetChild(1); // The next two children are the points you want to move to
        point2 = transform.GetChild(2);
        rb = platform.GetComponent<Rigidbody2D>();
        currPoint = point2;
    }

    // Update is called once per frame
    void Update()
    {
        //Starts moving platform towards current target point. Point 2 by default
        platform.transform.position = Vector3.MoveTowards(platform.transform.position, currPoint.position, speed * Time.deltaTime);
        //If we're at point 2 change target point to point 1
        if (platform.transform.position == currPoint.position && currPoint == point2)
        {
            currPoint = point1;
        }
        //If we're at point 1 change target point to point 2
        if (platform.transform.position == currPoint.position && currPoint == point1)
        {
            currPoint = point2;
        }
    }
}
