using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * I mean, its rough code to respwen the player. It works well enough
 * The player can grapple while dead tho oop -Brennan
 */
public class PlayerDeath : MonoBehaviour
{
    private bool isDead = false;
    public PlayerController playerController;
    public GrappleHook grapple;
    public Animator anim;
    public float dieTime = 0.5f;
    public float respawnTime = 0.7f;
    private Health health;

    private void Start()
    {
        health = GetComponent<Health>();
    }

    // Respawn the player when they make contact with an object with the death tag
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!isDead)
        {
            if (col.gameObject.CompareTag("Death"))
            {
                Die();
            }
        }
    }

    // Respawn the player when they make contact with an object with the death tag
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!isDead)
        {
            if (col.gameObject.CompareTag("Death"))
            {
                Die();
            }
        }
    }

    //Yeah we don't need this method
    public void Die()
    {
        StartCoroutine(DieAnim());
    }

    // Play death animation and respawn the player
    IEnumerator DieAnim()
    {
        isDead = true;

        //Take damage
        health.TakeDamage(1);

        //Make invincible
        health.MakeInvincible();

        //Stop movement
        playerController.dying = true;
        grapple.preventGrapple = true;
        grapple.cancelGrapple();
        //Play anim
        anim.SetBool("IsDying", true);
        //Wait for anim to finish
        yield return new WaitForSeconds(dieTime);
        //respawn
        LevelManager.instance.Respawn();
        anim.SetBool("IsDying", false);
        //play respawn anim
        anim.SetBool("IsRespawning", true);
        yield return new WaitForSeconds(respawnTime);
        anim.SetBool("IsRespawning", false);
        //Allow player to player the game again
        playerController.dying = false;
        grapple.preventGrapple = false;
        isDead = false;
    }
}
