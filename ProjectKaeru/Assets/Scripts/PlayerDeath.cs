using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private bool isDead = false;

    //Nick commented out this code because he thought it might have caused the shadow clone glitch but I don't think thats the case and we may need to update it and use it sometime -Bren
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!isDead)
        {
            if (col.gameObject.CompareTag("Death"))
            {
                isDead = true;
                LevelManager.instance.Respawn();
                isDead = false;
            }
        }
    }

    /*
     * Whenever the player enters a trigger with the "Death" tag the player is deleted and a new prefab is created at the respawn point
     * Max makes a good point that it may be better to just teleport the player to the respawn point instead and I agree since deleting it and making a new prefab would reset the player's health completely -Bren
     * 
     * I did the teleport to respawn change, you're welcome -Nick
     */
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!isDead)
        {
            if (col.gameObject.CompareTag("Death"))
            {
                isDead = true;
                LevelManager.instance.Respawn();
                isDead = false;
            }
        }
    }
}
