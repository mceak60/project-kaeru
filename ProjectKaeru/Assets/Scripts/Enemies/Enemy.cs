using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;

    public int maxHealth = 100;
    public int currentHealth;

    //public float startKnockbackTime; // Amount of time the player takes knockabck
    //private float knockbackTime; // Used for counting knockbackTime
    public float knockbackAmount = 15; // The force we use to knock back the player
    //private bool knockback = false; // Whether the player is currently taking knockback

    private Rigidbody2D playerRB;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    //Update the enemies damage, play a stagger animation, and kill them if their health drops below 0
    public void TakeDamage(int damage, Rigidbody2D rb)
    {
        currentHealth -= damage;

        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }

        playerRB = rb;
        ApplyKnockback();
    }

    //Plays the death animation and disables the enemies collider so the player can no longer be blocked by the enemies collider. 
    //However, doing this causes the enemy to fall through the floor -Bren
    void Die()
    {

        animator.SetBool("IsDead", true);

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    private void ApplyKnockback()
    {
        Rigidbody2D enemyRB = GetComponent<Rigidbody2D>();
        Vector2 moveDirection = enemyRB.transform.position - playerRB.transform.position;
        if (moveDirection.x <= 0)
            enemyRB.velocity = new Vector2(-1, 0.35f) * knockbackAmount;
        else
            enemyRB.velocity = new Vector2(1, 0.35f) * knockbackAmount;
    }
}
