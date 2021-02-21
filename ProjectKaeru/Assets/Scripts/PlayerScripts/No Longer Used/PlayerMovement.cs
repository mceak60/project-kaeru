 using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Test
public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    public float runSpeed = 100f;

    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;
    bool grounded = true;
    bool attacking = false;
    bool dashing = false;

    private float dashTime;
    public float startDashTime;
    public float dashRate = 1f;
    float nextDashTime = 0f;

    public Transform wallGrabPoint;
    private bool canGrab, isGrabbing;
    private float gravityStore;
    public float wallJumpTime = .1f;
    private float wallJumpCounter;

    [SerializeField] private LayerMask m_WhatIsWall;

    private bool wasGrabbing = false;


    private void Start()
    {
        dashTime = startDashTime;
    }

    // Update is called once per frame
    void Update()
    {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

            if (Input.GetButtonDown("Jump") && !attacking)
            {
                jump = true;
                animator.SetBool("IsJumping", true);
            }


            if (Input.GetButtonDown("Crouch"))
            {
                crouch = true;
                animator.SetBool("IsCrouching", true);
            }
            else if (Input.GetButtonUp("Crouch"))
            {
                crouch = false;
                animator.SetBool("IsCrouching", false);
            }

            if (!controller.getGrounded() && grounded == true)
            {
                grounded = false;
                animator.SetBool("IsGrounded", false);
            }
            else if (controller.getGrounded() && grounded != true)
            {
                grounded = true;
                animator.SetBool("IsGrounded", true);
            }

            if (GetComponent<PlayerCombat>().isAttacking)
            {
                attacking = true;
            }
            else
            {
                attacking = false;
            }

            if (Time.time >= nextDashTime)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift) && !attacking)
                {
                    dashing = true;
                    nextDashTime = (Time.time + 1f / dashRate) + dashTime;
                    animator.SetBool("IsDashing", true);
                }
            }


            canGrab = Physics2D.OverlapCircle(wallGrabPoint.position, .2f, m_WhatIsWall);
            isGrabbing = false;
            if (canGrab && !grounded)
            {
                if (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Horizontal") < 0)
                {
                    isGrabbing = true;
                    animator.SetBool("IsGrabbing", true);
                }
            }
            if (isGrabbing == true)
            {
                wasGrabbing = true;
                if (Input.GetButtonDown("Jump"))
                {
                    jump = true;
                    wallJumpCounter = wallJumpTime;
                    isGrabbing = false;
                    animator.SetBool("IsGrabbing", false);
                    animator.SetBool("IsJumping", true);
                }
                else 
                {
                wasGrabbing = false;
                }

            }
            else
            {
                if (wasGrabbing)
                {
                    animator.SetBool("IsJumping", false);
                    wasGrabbing = false;
                }
                animator.SetBool("IsGrabbing", false);
            }

    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }


    void FixedUpdate()
    {
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, attacking, dashing, isGrabbing);
            jump = false;
            if (dashing)
            {
                if (dashTime <= 0)
                {
                    dashing = false;
                    animator.SetBool("IsDashing", false);
                    dashTime = startDashTime;
                }
                else
                {
                    dashTime -= Time.deltaTime;
                }
            }
    }
}
