using System;
using System.Collections;
using System.Runtime.InteropServices;
using _game.Scripts;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
	
	//bot difficulty idea: less likely they will flee, faster reaction time, better accuracy
	
	[SerializeField] public Animator animator;
	[SerializeField] EnemyVision enemyVision;
	[SerializeField] float fovAngle = 90;

	[SerializeField] private float health = 100;
	[SerializeField] private float maxHealth = 100;
	[SerializeField] ParticleSystem death_part;
	[SerializeField] public WeaponSlots weaponSlots {get; private set;}
	public NavMeshAgent navMeshAgent { get; private set; }
	
	public float navigationRadius {get; private set;} = 30;
	
	private bool _dead = false;
	public Transform target {get; private set;}
	public Vector3 lastSeenTargetPos {get; private set;}
	
	public static event Action OnDeath;

	Bot_State currentState;
	public Bot_State_Attacking state_Attacking = new Bot_State_Attacking();
	public Bot_State_Tracking state_Tracking = new Bot_State_Tracking();
	public Bot_State_Roam state_Roam = new Bot_State_Roam();
	
	[Header("Crouch Settings")]
	public float crouchHeight = 1.0f;
	public float crouchSpeed = 3.0f;
	public Vector2 crouchCooldown = new Vector2(10,20);
	public Vector2 jumpCooldown = new Vector2(10,20);
	float crouchCd;
	float JumpCd;
	float crouchCheckCd;
	bool crouchedTrigger = false;
	private Vector3 originalCenter;
	private float originalHeight;
	
	[Header("Slide Settings")]
	public float speedToSlide = 7;
	public float slideSpeed = 10.0f;
	public float slideDuration = 1.0f;
	public float slideCooldown = 2.0f;
	private float lastSlideTime;

	private bool isCrouching = false;
	private bool isSliding = false;
	private float slideTimer;
	private bool isRunning;
	
	[Header("Jump Settings")]
	public float jumpForce = 5f;
	//public float gravity = -9.81f;
	private bool _isGrounded;
	float jumpLeapTime;
	bool isJumping = false;
	bool isFalling = false;
	private CapsuleCollider col;
	[SerializeField] Transform groundCheckPos;
	[SerializeField] Rigidbody rb;


	 private void Start()
	{
		col = GetComponent<CapsuleCollider>();
		rb=GetComponent<Rigidbody>();
		navMeshAgent = GetComponent<NavMeshAgent>();
		weaponSlots = GetComponent<WeaponSlots>();
		currentState = state_Roam;
		currentState.EnterState(this);
		originalHeight = col.height;
		originalCenter = col.center;
	}

	void Update()
	{
		HandleMovement();
		HandleInteraction();
		currentState.UpdateState();
	}
	
	public void SwitchState(Bot_State state)
	{
		Debug.Log("switch state to: " + state.ToString());
		currentState.ExitState();
		currentState = state;
		currentState.EnterState(this);		
	}
	
	public void SetTarget(Transform target)
	{
		if(target == null)
		{
			lastSeenTargetPos = this.transform.position;
			SwitchState(state_Tracking);
			this.target = target;
		}else
		{
			this.target = target;
			SwitchState(state_Attacking);
		}
	}
	public void SeenHealthKit(Transform pickup)
	{
		//if health is low go to this position
	}
	public void SeenWeaponPickup(Transform pickup)
	{
		//if ammo is low go to this position
	}
	public void FindWeaponPickup()
	{
		// call this from weapon script when ammo is low
	}
	void FindHealthKit()
	{
		//if health is bellow 30 go to this position
		if(enemyVision.CheckClosestHealthKit(out Transform position))
		{
			
		} 
	}
	
	private void CheckGrounded() { _isGrounded = Physics.Raycast(groundCheckPos.position, Vector3.down, 0.3f); }
	
	void HandleMovement()
	{
		CheckGrounded();
		RandomCrouch();
		RandomJump();
		HandleJump();
		if(NeedCrouchCheck())
		{
			ForceCrouch();
		}
		/* if(navMeshAgent.velocity.magnitude < 1f && navMeshAgent.remainingDistance > 0.2f)
		{
			Crouch();
		}
		if(crouchedTrigger)
		{
			if(!Physics.Raycast(transform.position, Vector3.up, 2))
			{
				UnCrouch();
			}
		} */
		HandleCrouchAndSlide();
		//if direction to navmesh target raycast doesnt get blocked on distance of 4 -> high jump probability
		
		
		//if in navmesh target direction we see Grappling hook -> use it							
	}
	
	void RandomJump()
	{
		
		if(JumpCd >= 0)
		{
			JumpCd -= Time.deltaTime;
			jumpLeapTime += Time.deltaTime;
			if(isJumping && _isGrounded && jumpLeapTime > 0.5f)
			{
				JumpCd = 0;
			}
			if(JumpCd <= 0)
			{
				if(isJumping)
				{
					JumpCd = UnityEngine.Random.Range(jumpCooldown.x, jumpCooldown.y);
					isFalling= true;
					isJumping= false;
				}else
				{
					//Debug.Log("bot jumped");
					animator.SetTrigger("Jump");
					rb.isKinematic = false;
					rb.AddForce(navMeshAgent.desiredVelocity + Vector3.up * jumpForce, ForceMode.Impulse);
					JumpCd = UnityEngine.Random.Range(1, 1.5f);
					isJumping = true;
					navMeshAgent.updatePosition = false;
				}
				jumpLeapTime = 0;
			}
		}
	}
	
	bool NeedCrouchCheck()
	{
		bool needs = false;
		if(crouchCheckCd >= 0)
		{
			crouchCheckCd -= Time.deltaTime;
			if(crouchCheckCd <= 0)
			{
				//Debug.DrawRay(transform.position, navMeshAgent.velocity.normalized * 4, Color.blue, 5);
				//Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - 0.6f, transform.position.z), navMeshAgent.velocity.normalized * 4, Color.green, 5);
				if(Physics.Raycast(transform.position, navMeshAgent.velocity.normalized, 4))
				{
					if(!Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.6f, transform.position.z), navMeshAgent.velocity.normalized, 4))
					{
						needs = true;
					}
				}
				crouchCheckCd = 0.3f;
			}
		}
		return needs;
	}
	
	void RandomCrouch()
	{
		if(crouchCd >= 0)
		{
			crouchCd -= Time.deltaTime;
			if(crouchCd <= 0)
			{
				if(crouchedTrigger)
				{
					UnCrouch();
					crouchCd = UnityEngine.Random.Range(crouchCooldown.x, crouchCooldown.y);
				}else
				{
					Crouch();
					crouchCd = UnityEngine.Random.Range(1f,3f);
				}
			}
		}
		
	}
	
	void Crouch()
	{
		//Debug.Log("crouch");
		crouchedTrigger = true;
	}
	
	void UnCrouch()
	{
		//Debug.Log("uncrouch");
		crouchedTrigger = false;
	}

	void ForceCrouch()
	{
		crouchedTrigger = false;
		crouchCd = 0;
	}
	
	void HandleJump()
	{
		//Debug.DrawRay(transform.position, Vector3.down * 1f, Color.blue, 1);
		//Debug.Log("grounded "+ _isGrounded);
		/* if (isJumping)
		{
			//velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
			//characterController.Move(velocity * Time.deltaTime);
			//transform.position = new Vector3(transform.position.x, transform.position.y + velocity.y * Time.deltaTime, transform.position.z);
			
		} */
		/* if(!_isGrounded || isFalling)
		{
			//transform.position = new Vector3 (transform.position.x, transform.position.y + gravity * Time.deltaTime, transform.position.z);
		} */
		if(isFalling && _isGrounded)
		{
			navMeshAgent.nextPosition = transform.position;
			navMeshAgent.updatePosition = true;	
			rb.isKinematic = true;
			isFalling = false;
		}
		
	}
		

	private void HandleCrouchAndSlide()
	{
		if (crouchedTrigger && navMeshAgent.speed > 7 && !isSliding && Time.time >= lastSlideTime + slideCooldown)
		{
			// Start slide
			isSliding = true;
			slideTimer = slideDuration;
			col.height = crouchHeight / 2; // Shorten height for slide
			col.center = new Vector3(0, crouchHeight / 4, 0);
			animator.SetFloat("SlideAnim", UnityEngine.Random.Range(0f,1f));
			//Debug.Log("start slide");
		}

		if (isSliding)
		{
			slideTimer -= Time.deltaTime;
			if (slideTimer <= 0)
			{
				// End slide
				isSliding = false;
				lastSlideTime = Time.time;
				ResetHeight();
				//Debug.Log("end slide");
			}
		}

		if (crouchedTrigger && !isSliding && !isCrouching)
		{
			// Crouch
			isCrouching = true;
			col.height = crouchHeight;
			col.center = new Vector3(0, crouchHeight / 2, 0);
			//Debug.Log("start crouch");
		}
		else if (!crouchedTrigger && isCrouching)
		{
			// End crouch
			isCrouching = false;
			ResetHeight();
			//Debug.Log("end crouch");
		}
		animator.SetBool("IsSliding", isSliding);
		animator.SetBool("IsCrouching", isCrouching);

		// if (isCrouching || isSliding)
		// {
		//     _model.localPosition = new Vector3(0, -0.8f, 0);
		// }
		// else
		// {
		//     _model.localPosition = Vector3.zero;
		// }
	}

	private void ResetHeight()
	{
		// Reset height and center when crouching or sliding ends
		col.height = originalHeight;
		col.center = originalCenter;
	}	
	
	public void RotateToPosition(Vector3 position)
	{
		Vector3 direction = position - transform.position;
		if (direction.magnitude > 0.5f) // Avoid jitter when close
		{
			Quaternion lookRotation = Quaternion.LookRotation(direction);

			float cameraAngle = lookRotation.eulerAngles.x;
			if(cameraAngle > 90 && cameraAngle <= 360) cameraAngle -= 360;
			float camRot = Mathf.InverseLerp(90, -90f, cameraAngle);
			animator.SetFloat("SpineRotation", camRot);

			lookRotation.z = 0;
			lookRotation.x = 0;
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10); // Smooth rotation
		}else
		{
			animator.SetFloat("SpineRotation", Mathf.Lerp(animator.GetFloat("SpineRotation"), 0.5f, Time.deltaTime));
		}
	}
	
	void HandleInteraction()
	{
		//if seen interactable object (grappling point, explosive barrel) interact accordingly
	}
	
	public void TakeDamage(float damage, Vector3 sourcePos)
	{
		health -= damage;
		if(health<=0)
		{
			Respawn();
		}
		if(health < 30)
		{
			FindHealthKit();
		}
		RotateToPosition(sourcePos);
	}
	
	public void AddHealth(float amount)
	{
		health += amount;
		if(health > maxHealth) health = maxHealth;
		if(health < 30)
		{
			FindHealthKit();
		}
	}

	private void Respawn()
	{
		death_part.Play();
		OnDeath?.Invoke();
		
		//reset state
		health = maxHealth;
		target = null;
		currentState = state_Roam;
		currentState.EnterState(this);
		
		//reset position
		int side = Random.Range(0, 4);
		int cell = Random.Range(-5, 6);
		switch (side)
		{
			case 0:
				navMeshAgent.Warp(new Vector3(20, 0, cell * 4));
				break;
			case 1:
				navMeshAgent.Warp(new Vector3(-20, 0, cell * 4));
				break;
			case 2:
				navMeshAgent.Warp(new Vector3(cell * 4, 0, 20));
				break;
			case 3:
				navMeshAgent.Warp(new Vector3(cell * 4, 0, -20));
				break;
		}
	}
}

public abstract class Bot_State
{
	public abstract void EnterState(EnemyAI ai);
	public abstract void UpdateState();
	public abstract void ExitState();
}

public class Bot_State_Roam : Bot_State
{
	EnemyAI enemyAI;
	NavMeshAgent agent;
	public override void EnterState(EnemyAI ai)
	{
		enemyAI = ai;
		agent = enemyAI.navMeshAgent;
		SetRandomPosition();
	}
	public override void UpdateState()
	{
		if(agent.isOnNavMesh)
		{
			if(agent.remainingDistance < 0.3f) SetRandomPosition();
		}
			//jump or crouch randomly
			//crouch through small spaces
			//if seen interactable object (grappling point, explosive barrel) interact accordingly
		
	}
	public override void ExitState()
	{
		
	}
	
	void SetRandomPosition()
	{
		Vector3 pointInRadius = enemyAI.transform.position + Random.insideUnitSphere * enemyAI.navigationRadius;
		pointInRadius.y += 150;
		RaycastHit hit;

		Physics.Raycast(pointInRadius, Vector3.down, out hit);
		if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1, NavMesh.AllAreas))
		{
			agent.SetDestination(hit.point);
		}
	}
}

public class Bot_State_Tracking : Bot_State
{
	Vector3 lastSeenPos;
	EnemyAI enemyAI;
	NavMeshAgent agent;
	public override void EnterState(EnemyAI ai)
	{
		lastSeenPos = ai.lastSeenTargetPos;
		enemyAI = ai;
		agent = enemyAI.navMeshAgent;
		if (NavMesh.SamplePosition(lastSeenPos, out NavMeshHit navHit, 1, NavMesh.AllAreas))
		{
			agent.SetDestination(lastSeenPos);
		}else
		{
			enemyAI.SwitchState(enemyAI.state_Roam);
		}
	}
	public override void UpdateState()
	{
		if(agent.remainingDistance < 0.3f)
		{
			enemyAI.SwitchState(enemyAI.state_Roam);
		}
	}
	public override void ExitState()
	{
	
	}
}

public class Bot_State_Attacking : Bot_State
{
	NavMeshAgent agent;
	EnemyAI enemyAI;
	WeaponSlots weaponSlots;
	public override void EnterState(EnemyAI ai)
	{
		enemyAI = ai;
		agent = enemyAI.navMeshAgent;
		this.weaponSlots = enemyAI.weaponSlots;
		agent.updateRotation = false;
		SetRandomRoomPosition();
	}
	public override void UpdateState()
	{
		if(agent.remainingDistance < 0.3f)
		{
			SetRandomRoomPosition();
		}
		
		ShootTarget();
		//higher jump probability
		//
		//upper body is focused on target player (with some inaccuracy) and shoot him
	}
	public override void ExitState()
	{
		agent.updateRotation = true;
	}
	
	void ShootTarget()
	{
		//rotate bot and his spine
		/* Vector3 direction = new Vector3(enemyAI.target.position.x, enemyAI.target.position.y + 0.9f, enemyAI.target.position.z) - enemyAI.transform.position;
		if (direction.magnitude > 0.3f) // Avoid jitter when close
		{
			Quaternion lookRotation = Quaternion.LookRotation(direction);
			
			float cameraAngle = lookRotation.eulerAngles.x;
			if(cameraAngle > 90 && cameraAngle <= 360) cameraAngle -= 360;
			float camRot = Mathf.InverseLerp(90, -90f, cameraAngle);
			enemyAI.animator.SetFloat("SpineRotation", camRot);
			
			lookRotation.z = 0;
			lookRotation.x = 0;
			enemyAI.transform.rotation = Quaternion.Slerp(enemyAI.transform.rotation, lookRotation, Time.deltaTime * 5); // Smooth rotation
		}else
		{
			enemyAI.animator.SetFloat("SpineRotation", 0.5f);
		} */
		enemyAI.RotateToPosition(new Vector3(enemyAI.target.position.x, enemyAI.target.position.y + 0.9f, enemyAI.target.position.z));
		if(enemyAI.target != null )weaponSlots.Bot_Shooting(enemyAI.target);
	}
	
	void SetRandomRoomPosition()
	{
		bool pointInRoom = false;
		Vector3 pointInRadius = Vector3.zero;
		
		//check if random position is in the same room (bot wont leave fight)
		while(!pointInRoom)
		{
			pointInRadius = enemyAI.target.position + Random.insideUnitSphere * 4;
			pointInRadius.y += 3;
			if(!Physics.Raycast(enemyAI.transform.position, (pointInRadius - enemyAI.transform.position).normalized, Vector3.Distance(enemyAI.transform.position, pointInRadius)))
			{
				pointInRoom = true;
			}
		}
		
		RaycastHit hit;
		Physics.Raycast(pointInRadius, Vector3.down, out hit);
		if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1, NavMesh.AllAreas))
		{
			agent.SetDestination(hit.point);
		}
	}
}

public class Bot_State_FindPickup: Bot_State
{
	EnemyAI enemyAI;
	Transform target;
	bool lowHealth;
	public override void EnterState(EnemyAI ai)
	{
		enemyAI = ai;
	}
	public override void UpdateState()
	{
		//low health?
		//		pick last know healthkit location
		//no ammo?
		//		pick last know ammo location
		
		//check raycast if it is not blocked the target location (bot sees it)
		// if not blocked 
		//		check if it still exists in Vision array (not picked up)
		//			if not pick other location	(repeat)
		
		//if distance from target < 0.2f
		//		check if there is need for health / ammo
		//				if not -> free roam
	}
	public override void ExitState()
	{
		
	}
}
