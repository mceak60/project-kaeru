using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private GameObject platform;
    private Transform point1;
    private Transform point2;
    private Rigidbody2D rb;
    private Transform currPoint;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        platform = transform.GetChild(0).gameObject;
        point1 = transform.GetChild(1);
        point2 = transform.GetChild(2);
        rb = platform.GetComponent<Rigidbody2D>();
        currPoint = point2;
    }

    // Update is called once per frame
    void Update()
    {
        platform.transform.position = Vector3.MoveTowards(platform.transform.position, currPoint.position, speed * Time.deltaTime);
        if (platform.transform.position == currPoint.position && currPoint == point2)
        {
            currPoint = point1;
        }
        if (platform.transform.position == currPoint.position && currPoint == point1)
        {
            currPoint = point2;
        }
    }
}
