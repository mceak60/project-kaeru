using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public int attackDamage;
    public bool isAttacking = false;
    public float attackRate;

    private float nextAttackTime = 0f;

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

    private void OnTriggerEnter2D(Collider2D col)
    { 
        if (col.gameObject.CompareTag("Enemy"))
                col.transform.GetComponent<Enemy>().TakeDamage(attackDamage, transform.parent.GetComponent<Rigidbody2D>());
    }
}
