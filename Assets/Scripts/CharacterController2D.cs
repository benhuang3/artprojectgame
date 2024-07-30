using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class CharacterController2D : MonoBehaviour
{

	[SerializeField] private float m_JumpForce;							// Amount of force added when the player jumps.
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	

	const float k_GroundedRadius = 0.5f; // Radius of the overlap circle to determine if grounded
	           // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	public bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Attacking")]
	[SerializeField] float timeBetweenAttack;
	float timeSinceAttack = 0;
	[SerializeField] Transform SideAttackTransform, UpAttackTransform,DownAttackTransform;
	[SerializeField] Vector2  SideAttackArea, UpAttackArea, DownAttackArea;
	[SerializeField] LayerMask AttackableLayer;
	[SerializeField] int damage;

	ContactFilter2D attackableFilter;

	
	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;



	private bool m_Grounded; 
	private float jumpTimer = 0;
	// private bool isJumping = false;
	private bool inControl = true;
	private bool onDashCooldown = false;
	private bool isDashing = false;

	private bool hasDashed = false; //no double midair jumps or dashes
	private bool hasJumped = false;

	public bool isRecoil = false;

	public PlayerMovement PlayerMovementscript;
	public HealthScript healthscript;
	public PlayerColliderScript PlayerColliderScript_reference;

		[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }



	private void Awake()
	{

		attackableFilter.useLayerMask = true;
		attackableFilter.layerMask = AttackableLayer;

		m_Rigidbody2D = GetComponent<Rigidbody2D>();


		if (m_Rigidbody2D == null)
        {
            Debug.LogError("Rigidbody2D component not found on " + gameObject.name);
        }

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

	}

	private void Start()
	{
		Debug.Log("starting");
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
				hasJumped = false;
				hasDashed = false;
				jumpTimer = 0;

				if (!wasGrounded)
					OnLandEvent.Invoke();
				// isJumping = false;
			}
		}
	}


	public void Move(float move, bool jump)
	{
		// max fall speed
		if (m_Rigidbody2D.velocity.y < -8f)
		{
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, -8f);
		}
		
		// no falling during dash
		if (isDashing)
		{
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0f);
		}


		// base movement
		if (inControl)
		{
			 m_Rigidbody2D.velocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);

			if (move > 0 && !m_FacingRight)
			{
				Flip();
			}
			else if (move < 0 && m_FacingRight)
			{
				Flip();
			}
		}

		// If the player should jump...
		if ( ! hasJumped)
		{
			if (m_Grounded && jump)
			{
				// Add a vertical force to the player.
				m_Grounded = false;
				// m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
				m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 12);
			}

			else if (jumpTimer < 0.5f && jump)
			{

				Vector3 targetVelocity = new Vector2(m_Rigidbody2D.velocity.x, 8);
				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

				// m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 8);
				jumpTimer += Time.deltaTime;
			}
		}
		

		// no double jumps
		if (! m_Grounded && !jump)
		{
			hasJumped = true;
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

	// private void OnDrawGizmos()
	// {
	// 	Gizmos.color = Color.red;
	// 	Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
	// 	Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
	// 	Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);

	// }

	public void Attack(bool attackBool, float xAxis, float yAxis)
	{
		timeSinceAttack += Time.deltaTime;
		if (inControl)
		{
			if (attackBool && timeSinceAttack  >= timeBetweenAttack)
			{
				timeSinceAttack = 0;

				//anim.SetTrigger("Atttacking");
				if (yAxis == 0 || yAxis < 0 && m_Grounded)
				{
					Hit(SideAttackTransform.GetComponent<PolygonCollider2D>(), "side");
					// Debug.Log("Side Attack");
				}
				else if (yAxis > 0)
				{
					Hit(UpAttackTransform.GetComponent<PolygonCollider2D>(), "up");
					// Debug.Log("Up Attack");
				}

				if (yAxis < 0 && !m_Grounded)
				{
					Hit(DownAttackTransform.GetComponent<PolygonCollider2D>(), "down");
					// Debug.Log("Down Attack");
				}
			}
		
		}
	}

	void Hit (Collider2D attackCollider, string attackDirection)
	{
		// Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(attackTransform.position, attackArea, 0, AttackableLayer);

		Collider2D[] objectsToHit = new Collider2D[10];
		int overlapCount = Physics2D.OverlapCollider(attackCollider, attackableFilter, objectsToHit);
		isRecoil = true;
		if (objectsToHit.Length > 0)
		{
			for (int i = 0; i < overlapCount; i++)
			{
				// Debug.Log(objectsToHit[i]);
				if (objectsToHit[i].GetComponent<Enemy>() != null)
				{
					objectsToHit[i].GetComponent<Enemy>().EnemyGetHit(damage, attackDirection);

					if (isRecoil)
					{
						Recoil(attackDirection);
						isRecoil = false;
					}
				}
			}
		}
	}

	public void Recoil(string attackDirection)
	
	{
		StartCoroutine(loseControl(0.05f));
		switch (attackDirection)
			{
				case "side":
					if (m_FacingRight)
					{
						m_Rigidbody2D.AddForce(new Vector2(-30.0f, -10.0f), ForceMode2D.Impulse);
					}
					else if (!m_FacingRight){
						m_Rigidbody2D.AddForce(new Vector2(30.0f, -10.0f), ForceMode2D.Impulse);
					}
					break;
				
				case "up":
					m_Rigidbody2D.velocity = Vector3.zero;
					m_Rigidbody2D.AddForce(new Vector2(0.0f, -300.0f));
					break;

				case "down":
					m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 10);
					// m_Rigidbody2D.AddForce(new Vector2(0.0f, 300.0f));
					Debug.Log("down attack");
					break;
					
				

			}
	}
	
	public void Dash(bool dashBool)
	{
		if (! hasDashed)
		{
			if (dashBool && !onDashCooldown)
			{
				StartCoroutine(dashControl());
			}

		}
		
		
	}

	IEnumerator dashControl()
    {	
		hasDashed = true;
		isDashing = true;
		onDashCooldown = true;
		changeInControl(false);

		if (m_FacingRight)
		{
			m_Rigidbody2D.velocity =  new Vector3(12, 0, 0);
		}
		else
		{
			m_Rigidbody2D.velocity =  new Vector3(-12, 0, 0);
		}


        yield return new WaitForSeconds(0.2f);

		isDashing = false;


		//if not in hitstun, return control
		if (! PlayerColliderScript_reference.returnInHitstun())
		{
			changeInControl(true);
		}
        
		yield return new WaitForSeconds(0.2f);
		onDashCooldown = false;


    }
	IEnumerator loseControl(float time)
	{
		changeInControl(false);
		yield return new WaitForSeconds(time);
		changeInControl(true);

	}

	public void changeInControl(bool b)
	{
		inControl = b;
	}
	public void playerAddForce(Vector2 v2)
	{
		m_Rigidbody2D.AddForce(v2, ForceMode2D.Impulse);
	}

	public void playerSetVelocity(Vector2 v2)
	{
		m_Rigidbody2D.velocity =  v2;
	}


	


}
