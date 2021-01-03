using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    public GameObject hitbox;
    public BoxCollider2D collider;
    public Animator animator;
    public LayerMask enemyLayers;
    
    public float attackRange = 0.5f;
    public int attackDamage = 40;
    public float attackRate = 1f;
    float nextAttackTime = 0f;
    public bool isAttacking = false;

    // Update is called once per frame
    void Update()
    {
        hitbox = GameObject.Find("Hitbox"); //gets the hitbox component
        

        if (Time.time >= nextAttackTime)
        {
            isAttacking = false;
<<<<<<< Updated upstream
=======
            hitbox.GetComponent<BoxCollider2D>().enabled = false; //disables the hitbox until attack
            //Debug.Log(hitbox.GetComponent<BoxCollider2D>().enabled); //debug

>>>>>>> Stashed changes
            //animator.SetBool("IsAttacking", false);
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                
                //Attack();
                nextAttackTime = Time.time + 1f / attackRate;
                isAttacking = true;
                hitbox.GetComponent<BoxCollider2D>().enabled = true; //enables hitbox before animation
              //  Debug.Log(hitbox.GetComponent<BoxCollider2D>().enabled); //debug

            }
            //Play animation + enable hitbox
            
            animator.SetBool("IsAttacking", isAttacking);
        }
    }


    void OnDrawGizmosSelected() //this is probably useless now lol (delete later)
    {
        if (hitbox == null)
            return;
        Gizmos.DrawWireCube(hitbox.transform.position, collider.size);
    }
}
