using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiSolidPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;
    private int layer;
    private bool canGoThrough = false;
    private bool rejump = false;
    // Start is called before the first frame update
    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
        layer = LayerMask.NameToLayer("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //
        if (Input.GetButton("Crouch") && Input.GetButtonDown("Jump") && !canGoThrough)
        {
            effector.colliderMask ^= (1 << layer);
            canGoThrough = true;
        }

        else if(canGoThrough && !Input.GetButton("Crouch"))
        {
            effector.colliderMask |= (1 << layer);
            canGoThrough = false;
        }
    }
}
