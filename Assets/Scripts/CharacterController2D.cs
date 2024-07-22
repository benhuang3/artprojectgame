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

	const float k_GroundedRadius = 0.5f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
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

	
	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;



	private bool inControl = true;
	private bool onDashCooldown = false;

	public bool isRecoil = false;

	public PlayerMovement PlayerMovementscript;
	public HealthScript healthscript;
	public PlayerColliderScript PlayerColliderScript_reference;

		[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }


	private int i;

	private void Awake()
	{
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
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool jump)
	{

		//only control the player if grounded or airControl is turned on
		if (inControl)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);

			 m_Rigidbody2D.velocity = targetVelocity;

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
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

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
		Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
		Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);

	}

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
					Hit(SideAttackTransform, SideAttackArea, "side");
					// Debug.Log("Side Attack");
				}
				else if (yAxis > 0)
				{
					Hit(UpAttackTransform, UpAttackArea, "up");
					// Debug.Log("Up Attack");
				}

				if (yAxis < 0 && !m_Grounded)
				{
					Hit(DownAttackTransform, DownAttackArea, "down");
					// Debug.Log("Down Attack");
				}
			}
		
		}
	}

	void Hit (Transform attackTransform, Vector2 attackArea, string attackDirection)
	{
		Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(attackTransform.position, attackArea, 0, AttackableLayer);
		isRecoil = true;
		if (objectsToHit.Length > 0)
		{
			for (int i = 0; i < objectsToHit.Length; i++)
			{
				// Debug.Log(objectsToHit[i]);
				if (objectsToHit[i].GetComponent<Enemy>() != null)
				{
					objectsToHit[i].GetComponent<Enemy>().EnemyGetHit(damage, attackDirection);

					if (isRecoil)
					{
						recoil(attackDirection);
						isRecoil = false;
					}
				}
			}
		}
	}

	public void recoil(string attackDirection)
	
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
					m_Rigidbody2D.velocity = Vector3.zero;
					m_Rigidbody2D.AddForce(new Vector2(0.0f, 300.0f));
					break;
				

			}
	}
	
	public void Dash(bool dashBool)
	{
		if (dashBool && !onDashCooldown)
		{
			StartCoroutine(dashControl());
		}
		
	}

	IEnumerator dashControl()
    {	
		changeInControl(false);

		if (m_FacingRight)
		{
			m_Rigidbody2D.velocity =  new Vector3(10, 0, 0);
		}
		else
		{
			m_Rigidbody2D.velocity =  new Vector3(-10, 0, 0);
		}

        yield return new WaitForSeconds(0.3f);
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
