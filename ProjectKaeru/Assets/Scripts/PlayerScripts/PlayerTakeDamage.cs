using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
// This is kind of scuffed but this script needs to be here so it doesn't detect collisions on the player's sword hitboxes
public class PlayerTakeDamage : MonoBehaviour
{
    public PlayerHealth health;

    private void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log(col);
        handleCollision(col);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        handleCollision(col);
    }

    private void handleCollision(Collider2D col)
    {
        if (!health.GetInvincible())
        {
            if (col.gameObject.CompareTag("EnemyHitbox"))
            {
                health.SetCollision(col);
                health.MakeInvincible();
                health.SetKnockback(true);
                health.controller.knockback = true;
                health.TakeDamage(1);
                //Debug.Log("Invincible");
                health.playerSprite.color = Color.gray;
            }
        }
    } 
}
