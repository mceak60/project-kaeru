using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;
    public float attackRate = 1f;
    float nextAttackTime = 0f;
    public bool isAttacking = false;

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
                nextAttackTime = Time.time + 1f / attackRate;
                isAttacking = true;
            }
            //Play animation
            animator.SetBool("IsAttacking", isAttacking);
        }
    }
}
