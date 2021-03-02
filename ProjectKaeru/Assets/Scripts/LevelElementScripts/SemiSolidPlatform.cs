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
        layer = LayerMask.NameToLayer("Player"); // The name of the layer we want the platform to be semisolid for. Hardcoded as player lol
    }

    // Update is called once per frame
    void Update()
    {
        // If the player presses down and jump make the platform not solid so the player can go through it
        if (Input.GetButton("Crouch") && Input.GetButtonDown("Jump") && !canGoThrough)
        {
            effector.colliderMask ^= (1 << layer);
            canGoThrough = true;
        }

        // When the player stops holding crouch make the platform solid again
        else if(canGoThrough && !Input.GetButton("Crouch"))
        {
            effector.colliderMask |= (1 << layer);
            canGoThrough = false;
        }
    }
}
