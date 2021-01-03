using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;

    public int maxHealth = 100;
    int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    //Update the enemies damage, play a stagger animation, and kill them if they're health drops below 0
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //Plays the death animation and disables the enemies collider so the player can no longer be blocked by the enemies collider. 
    //However, doing this causes the enemy to fall through the floor -Bren
    void Die()
    {

        animator.SetBool("IsDead", true);

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
   private void OnTriggerEnter2D(Collider2D other) //registers player hit
    {
        if(other.gameObject.tag == "PlayerAttack") //registers player hitbox (otherwise would register ALL colliders)
        {
            Debug.Log("Hit!");
            this.TakeDamage(34); //set value, change later
        }
        
    }
}
