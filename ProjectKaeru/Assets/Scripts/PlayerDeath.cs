using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private bool isDead = false;

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Death"))
        {
            Destroy(gameObject);
            LevelManager.instance.Respawn();
        }
    }*/

    private void OnTriggerEnter2D(Collider2D col)
    {   if(isDead)
        {
            return;
        }
        isDead = true;
        if (col.gameObject.CompareTag("Death"))
        {
            Destroy(gameObject);
            LevelManager.instance.Respawn();
        }
    }
}
