using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 40;
    public float attackRate = 1f;
    float nextAttackTime = 0f;
    public bool isAttacking = false;

    // Update is called once per frame
    /*
     * If the player's attack is not on cooldown then see if the player has pressed the attack button and switch to the attacking animation
     * This code needs to be updated to check if the player is already preforming an action such as dashing or wall grabbing and if so, it should not let them attack -Bren
     */
    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            isAttacking = false;
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                //Attack();
                nextAttackTime = Time.time + 1f / attackRate;
                isAttacking = true;
            }
            //Play animation
            animator.SetBool("IsAttacking", isAttacking);
        }
    }

    /*void Attack()
    {
 
        //Detect enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        //Damage enemies
        foreach (Collider2D enemy in hitEnemies)
        {
            //Debug.Log("enemy hit");
            enemy.transform.parent.GetComponent<Enemy>().TakeDamage(attackDamage, GetComponent<Rigidbody2D>());
        }
    }

    //This makes the hitbox appear in the scene editor
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }*/
}
