﻿using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

	[Range(0, 1)] [SerializeField] private float m_AttackSpeed = .1f; //Speed of attacking

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	public float dashSpeed;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	public float fallMultiplier = 2.5f;
	public float lowJumpMultiplier = 2f;

	public Transform wallGrabPoint;
	private bool canGrab, isGrabbing;
	private float gravityStore;
	public float wallJumpTime = .1f;
	private float wallJumpCounter;
	//public Animator animator;

	private void Start()
	{
		gravityStore = m_Rigidbody2D.gravityScale;
	}

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void Update()
	{	

	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool crouch, bool jump, bool attack, bool dash, bool grab)
	{
		/*canGrab = Physics2D.OverlapCircle(wallGrabPoint.position, .2f, m_WhatIsGround);

		isGrabbing = false;
		if (canGrab && !m_Grounded)
		{
			if (m_FacingRight && move > 0 || !m_FacingRight && move < 0)
			{
				isGrabbing = true;
				animator.SetBool("IsGrabbing", true);
			}
		}
		if (isGrabbing == true)
		{

			m_Rigidbody2D.gravityScale = 0f;
			m_Rigidbody2D.velocity = Vector2.zero;

			if (jump)
			{
				wallJumpCounter = wallJumpTime;

				m_Rigidbody2D.velocity = new Vector2(move*-210f, m_JumpForce/50);
				m_Rigidbody2D.gravityScale = gravityStore;
				isGrabbing = false;
				animator.SetBool("IsGrabbing", false);
				animator.SetBool("IsJumping", true);
				animator.SetBool("IsGrounded", true);
				Flip();
			}
		}
		else
		{
			m_Rigidbody2D.gravityScale = gravityStore;
			animator.SetBool("IsGrabbing", false);
		}*/
		if (wallJumpCounter <= 0)
		{
			if (grab)
			{

				m_Rigidbody2D.gravityScale = 0f;
				m_Rigidbody2D.velocity = Vector2.zero;

				if (jump)
				{
					wallJumpCounter = wallJumpTime;

					m_Rigidbody2D.velocity = new Vector2(move * -110f, m_JumpForce / 50);
					m_Rigidbody2D.gravityScale = gravityStore;
					Flip();
				}
			}

			else
			{
				m_Rigidbody2D.gravityScale = gravityStore;
			}

			if (dash)
			{
				if (m_FacingRight)
				{
					m_Rigidbody2D.velocity = Vector2.right * dashSpeed;
				}
				else
				{
					m_Rigidbody2D.velocity = Vector2.left * dashSpeed;
				}
			}
			else
			{
				//only control the player if grounded or airControl is turned on
				if (m_Grounded || m_AirControl)
				{

					// If crouching
					if (crouch)
					{
						if (!m_wasCrouching)
						{
							m_wasCrouching = true;
							OnCrouchEvent.Invoke(true);
						}

						// Reduce the speed by the crouchSpeed multiplier
						move *= m_CrouchSpeed;

						// Disable one of the colliders when crouching
						if (m_CrouchDisableCollider != null)
							m_CrouchDisableCollider.enabled = false;
					}
					else
					{
						// Enable the collider when not crouching
						if (m_CrouchDisableCollider != null)
							m_CrouchDisableCollider.enabled = true;

						if (m_wasCrouching)
						{
							m_wasCrouching = false;
							OnCrouchEvent.Invoke(false);
						}
					}

					if (attack && m_Grounded)
					{
						move *= m_AttackSpeed;
					}

					// Move the character by finding the target velocity
					Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
					// And then smoothing it out and applying it to the character
					m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

					// If the input is moving the player right and the player is facing left...
					if (move > 0 && !m_FacingRight)
					{
						// ... flip the player.
						Flip();
					}
					// Otherwise if the input is moving the player left and the player is facing right...
					else if (move < 0 && m_FacingRight)
					{
						// ... flip the player.
						Flip();
					}
				}
				// If the player should jump...
				if (m_Grounded && jump)
				{
					// Add a vertical force to the player.
					m_Grounded = false;
					m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
				}


					if (m_Rigidbody2D.velocity.y < 0)
					{
						m_Rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
					}
					else if (m_Rigidbody2D.velocity.y > 0 && !Input.GetButton("Jump"))
					{
						m_Rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
					}
				
			}
		}
		else 
		{
			wallJumpCounter -= Time.deltaTime;
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public bool getGrounded()
	{
		return m_Grounded;
	}
}

