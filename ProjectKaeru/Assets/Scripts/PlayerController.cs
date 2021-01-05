using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
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

	public float dashSpeed; //What to set the velocity of the player to for the dash

	public float fallMultiplier = 2.5f; //Increases the gravity of the player when they're falling to give them a weightier feeling
	public float lowJumpMultiplier = 2f; // Increases the gravity of the player when they tap space, allowing them to shorthop

	public Transform wallGrabPoint; // The point at which the player checks if they are against a wall they can grab
	private bool canGrab, isGrabbing; // Whether or not the player can grab the wall infront of them and whether or not they're grabbing it
	public float gravityStore; // Since we freeze gravity on the player when they're grabbing a wall, this stores the value we want to return gravity to
	public float wallJumpTime = .1f; //How long the player loses control after walljumping, increasing this number causes the player to have a longer walljump that they can't cancel out of
	private float wallJumpCounter; // Used to disable player movement between walljumps

	public Animator animator;
	public float runSpeed = 100f; //How fast the player moves

	private float horizontalMove = 0f; // How fast the player is moving. This is -1, 0, or 1 on keyboard or -1 - 1 on controller
	private bool jump = false; //Whether or not the player has triggered a jump, this is only used to tell the move method to apply vertical force to the player, not whether or not the jump animation is playing
	private bool crouch = false; // Whether or not the player is crouching, also controls whether or not the crouch animation is playing
	private bool attacking = false; // Whether or not the attack animation is playing
	private bool dashing = false; // Whether or not the player is dashing

	private float dashTime; // How long it has been since the player has last dashed
	public float startDashTime; // How long a dash lasts(?)
	public float dashRate = 1f; // Works with startDashTime to determine how often the player can dash
	private float nextDashTime = 0f; // When the player can dash again

	public float walljumpVertical = 50f; // How much vertical force is applied to the player when they walljump
	public float walljumpHorizontal = 50f; // How much horizontal force is applied to the player when they walljump

	public bool busy = false;


	[SerializeField] private LayerMask m_WhatIsWall; // What is considered a wall the player can jump off of

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent; // Method called when the player lands, I probably don't need it anymore

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent; // Method called when the player crouches
	private bool m_wasCrouching = false; // Whether or not the player just finished crouching

	private bool isWalljumping = false;

	private void Start()
	{
		dashTime = startDashTime;
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

	// Update is called once per frame
	void Update()
	{

		/*
		 * Most of the code in this part is just checking for the player's input and setting the corresponding value accordingly if that action can be taken
		 */
		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

		if (Input.GetButtonDown("Jump") && !attacking)
		{
			jump = true;
			animator.SetBool("IsJumping", true);
		}
		if (Input.GetButtonDown("Crouch"))
		{
			crouch = true;
		}
		else if (Input.GetButtonUp("Crouch"))
		{
			crouch = false;
		}

		if (GetComponent<PlayerCombat>().isAttacking && !isGrabbing)
		{
			attacking = true;
		}
		else
		{
			attacking = false;
		}

		/*
		 * This code allows the player to dash if its not on cooldown
		 */
		if (Time.time >= nextDashTime)
		{
			if (Input.GetKeyDown(KeyCode.LeftShift) && !attacking)
			{
				dashing = true;
				nextDashTime = (Time.time + 1f / dashRate) + dashTime;
				animator.SetBool("IsJumping", false);
			}
		}

		/*
		 * This code handles the walljump
		 */
		canGrab = Physics2D.OverlapCircle(wallGrabPoint.position, .2f, m_WhatIsWall);
		isGrabbing = false;
		//If we're against a wall we can grab and not on the floor...
		if (canGrab && !m_Grounded)
		{
		    //...and we're holding a direction then grab the wall
			if (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Horizontal") < 0)
			{
				isGrabbing = true;
			}
		}
		if (isGrabbing == true)
		{
			//If we're grabbing the wall and we press jump then we preform a walljump
			//I though that this instantly calling the move method when we get here would fix the inconsistent walljumps I've been having but the code gets here and calls the move method everytime so idk -Bren
			if (Input.GetButtonDown("Jump"))
			{
				isWalljumping = true;
				isGrabbing = false;
				jump = true;
				animator.SetBool("IsJumping", true);
				Move(horizontalMove * Time.fixedDeltaTime, false, true, false, false, true);
			}

		}
		else
		{
			animator.SetBool("IsGrabbing", false);
		}

		/*
		 * This is where most of the values are passed to the animator if that applies
		 */
		animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
		animator.SetBool("IsCrouching", crouch);
		animator.SetBool("IsGrounded", m_Grounded);
		animator.SetBool("IsDashing", dashing);
		animator.SetBool("IsGrabbing", isGrabbing);
	}

	//I'm pretty sure this is a relic of when the two scripts were seperate and I can move this code into where I update m_IsGrounded but it works and I'm too lazy deal with the possibility that it doesn't work -Bren
	public void OnLanding()
	{
		animator.SetBool("IsJumping", false);
	}

	private void FixedUpdate()
	{
		//Applies most of the physics to the player
		if (!isWalljumping && !busy)
		{
			Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, attacking, dashing, isGrabbing);
		}

		//We set jump to false right after move so we don't continue applying the upward force
		jump = false;

		//Updates the time keeping values for dashing if that applies
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
		if (colliders.Length == 0)
		{
			m_Grounded = false;
		}
	}


	public void Move(float move, bool crouch, bool jump, bool attack, bool dash, bool grab)
	{
		// this statements prevents the player from moving during the first part of the walljump, its how a lott of games do it
		if (wallJumpCounter <= 0)
		{
			/*
			 * This applies all the physics of the walljump
			 */
			if (grab)
			{

				m_Rigidbody2D.gravityScale = 0f;
				m_Rigidbody2D.velocity = Vector2.zero;

				if (jump)
				{
					wallJumpCounter = wallJumpTime;
					if(move < 0)
						m_Rigidbody2D.velocity = new Vector2(walljumpHorizontal, walljumpVertical);
					else
						m_Rigidbody2D.velocity = new Vector2(-1 * walljumpHorizontal, walljumpVertical);
					m_Rigidbody2D.gravityScale = gravityStore;

					isWalljumping = false;
					//Flip();
				}
			}
			else
			{
				m_Rigidbody2D.gravityScale = gravityStore;
			}

			/*
			 * This part applies the dash force
			 */
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
			/*
			 * This part handles all the other movement in general
			 */
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

					//If we're on the ground we reduce our speed while attacking
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
					//m_Grounded = false;
					m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
				}

				//If the player is falling we add more gravity to them than normal because it feels better
				if (m_Rigidbody2D.velocity.y < 0)
				{
					m_Rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
				}
				//If the player just jumped but let go of the jump key we increase the gravity applied to them allowing them to control their jump height better
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
}
