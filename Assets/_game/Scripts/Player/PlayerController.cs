using System;
using System.Collections;
using AYellowpaper;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using _game.Scripts;
using _game.Scripts.System;
using FMODUnity;

namespace _game.Scripts
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] PlayerStats playerStats;
		[SerializeField] private float _walkSpeed = 6f;
		[SerializeField] private float _runSpeed = 12f;
		[SerializeField] private float _grappleSpeed = 20f;
		[SerializeField] private float _acceleration;
		[SerializeField] float accelerate_mp = 1f;
		[SerializeField] private Animator _animator;
		[SerializeField] private Transform _cameraHeadTarget;
		[SerializeField] private Rigidbody _cameraHeadTarget_rb;
		[SerializeField] private Collider _cameraHeadTarget_col;

		[SerializeField] private Transform _camera;
		[SerializeField] private Transform _model;
		
		[SerializeField] EventReference grapple_sfx;
		
		private bool isDead;
		
		private InterfaceReference<IGun, MonoBehaviour> _gun;
		private Vector3 _moveDirection = Vector3.zero;
		private bool _canMove = true;
		private CharacterController _characterController;
		private float _targetSpeed;
		private float _speed;
		private bool _isGrounded;
		private bool _isGrappling;
		private Vector2 _moveInput;
		private bool _grappleInput = false;
		private bool _jumpInput = false;
		float speedX;
		float speedY;

		[Header("Jump / Physics Settings")]
		public float jumpHeight = 2.0f;
		public float gravity = -9.81f;
		public float mass = 1f;
		public float drag = 0.1f;

		private Vector3 velocity;
		private Vector3 phys_velocity = Vector3.zero;
		private float phys_groundReactTime = 0.3f;
		private float phys_cd;
		

		private void Start()
		{
			_characterController = GetComponent<CharacterController>();
			originalHeight = _characterController.height;
			originalCenter = _characterController.center;
			originalCameraPos = _cameraHeadTarget.transform.localPosition;

			gravity = Physics.gravity.y;

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			
			playerStats.OnDeath += OnDeath;
			playerStats.OnRespawn += OnRespawn;
		}
		
		private void Update()
		{
			CheckGrounded();
			SimulatePhysics();
			if(isDead) return;
			
			HandleGrapple();
			if (!_isGrappling)
			{
				HandleMovement();
				HandleCrouchAndSlide();
				HandleJump();
				HandleRotation();
			}
		}

		private void OnEnable()
		{
			GameManager.Instance.playerControlls.Player.Sprint.started += x => isRunning = true;
			GameManager.Instance.playerControlls.Player.Sprint.canceled += x => isRunning = false;
			GameManager.Instance.playerControlls.Player.Crouch.started += x => _CrouchInput = true;
			GameManager.Instance.playerControlls.Player.Crouch.canceled += x => _CrouchInput = false;
			GameManager.Instance.playerControlls.Player.SecondaryAttack.started += x => _grappleInput = true;
			GameManager.Instance.playerControlls.Player.SecondaryAttack.canceled += x => _grappleInput = false;
			GameManager.Instance.playerControlls.Player.Jump.started += x => _jumpInput = true;
			GameManager.Instance.playerControlls.Player.Jump.canceled += x => _jumpInput = false;
		}

		// TODO: redo subscribing as you cannot unsubscribe anonymous delagate
		// private void OnDisable()
		// {
		// 	GameManager.Instance.playerControlls.Player.Sprint.started -= x => isRunning = true;
		// 	GameManager.Instance.playerControlls.Player.Sprint.canceled -= x => isRunning = false;
		// 	GameManager.Instance.playerControlls.Player.Crouch.started -= x => _CrouchInput = true;
		// 	GameManager.Instance.playerControlls.Player.Crouch.canceled -= x => _CrouchInput = false;
		// 	GameManager.Instance.playerControlls.Player.SecondaryAttack.started -= x => _grappleInput = true;
		// 	GameManager.Instance.playerControlls.Player.SecondaryAttack.canceled -= x => _grappleInput = false;
		// 	GameManager.Instance.playerControlls.Player.Jump.started -= x => _jumpInput = true;
		// 	GameManager.Instance.playerControlls.Player.Jump.canceled -= x => _jumpInput = false;
		// }
		
		void OnDeath()
		{
			SetDeathCamPhysics(true);
			isDead = true;
		}

		void OnRespawn()
		{
			SetDeathCamPhysics(false);
			isDead = false;
		}
		
		void SetDeathCamPhysics(bool enable)
		{
			_cameraHeadTarget_rb.isKinematic = !enable;
			_cameraHeadTarget_col.enabled = enable;
			if(!enable)
			{
				_cameraHeadTarget.localPosition = originalCameraPos;
			}
		}
		
		private Vector3 _hitPos;

		
		
		private void HandleGrapple()
		{
			
			if (_grappleInput && !_isGrappling) // && Input.GetMouseButtonDown(1)
			{
				if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, 30, Physics.AllLayers, QueryTriggerInteraction.Collide))
				{
					Debug.Log("trying to grapple, what did I hit " + hit.transform.name, hit.transform);
					
					if (hit.transform.TryGetComponent(out GrapplePoint grapplePoint))
					{
						Debug.Log("trying to grapple, COMPONENT FOUND");
						_hitPos = grapplePoint.ArrivalPos;
						_isGrappling = true;
						FMODHelper.PlayNewInstance(grapple_sfx, transform);
					}
				}
			}
			
			if(_isGrappling)
			{
				Debug.Log("trying to grapple, GRappling now");
				isCrouching = false;
				isSliding = false;
				isRunning = false;
				_characterController.Move((_hitPos - transform.position).normalized * (20 * Time.deltaTime));
				if (Vector3.Distance(transform.position, _hitPos) < 0.4f)
				{
					_isGrappling = false;
					_speed = _grappleSpeed;
				}
			}
		}

		private void CheckGrounded()
		{ 
			_isGrounded = Physics.Raycast(transform.position, -transform.up, 0.15f);
			/* if(_isGrounded)LG_tools.DrawRay(transform.position, -transform.up * 0.25f, Color.cyan);
			else LG_tools.DrawRay(transform.position, -transform.up * 0.25f, Color.red); */
		}

		private void HandleMovement()
		{
			if (_targetSpeed < _speed)
			{
				_speed -= Time.deltaTime * _acceleration;
			}
			if (_targetSpeed > _speed)
			{
				_speed += Time.deltaTime * _acceleration;
			}

			Vector3 forward = transform.TransformDirection(Vector3.forward);
			Vector3 right = transform.TransformDirection(Vector3.right);

			// Press Left Shift to run
			//isRunning = Input.GetKey(KeyCode.LeftShift);
			_animator.SetBool("IsSprinting", isRunning);
			if (_canMove)
			{
				if (isCrouching)
				{
					_targetSpeed = crouchSpeed;
				}
				else if (isSliding)
				{
					_targetSpeed = slideSpeed;
				}
				else if (isRunning)
				{
					_targetSpeed = _runSpeed;
				}
				else
				{
					_targetSpeed = _walkSpeed;
				}
			}
			else
			{
				_targetSpeed = 0;
			}

			_moveInput = GameManager.Instance.playerControlls.Player.Move.ReadValue<Vector2>();
			
			/* if(_moveInput.x == 0 && speedY != 0)
			{
			    //deaccelerate speedY
			    speedY = Mathf.Lerp(speedY, _moveInput.x, Time.deltaTime * deaccelerate_mp);
			}else if(_moveInput.x != 0 && speedY == 0)
			{
			    //accelerate speedY
			    speedY = Mathf.Lerp(speedY, _moveInput.x, Time.deltaTime * accelerate_mp);
			} */
			
			speedY = Mathf.Lerp(speedY, _moveInput.x, Time.deltaTime * accelerate_mp);
			speedX = Mathf.Lerp(speedX, _moveInput.y, Time.deltaTime * accelerate_mp);
			
			//speedX = _moveInput.y;//Input.GetAxis("Vertical");
			//speedY = _moveInput.x;//Input.GetAxis("Horizontal");
			
			_animator.SetFloat(SpeedX, speedY);
			_animator.SetFloat(SpeedY, speedX);
			//_animator.SetBool(IsWalking, Mathf.Abs(speedX) > 0.1f || Mathf.Abs(speedY) > 0.1f);

			_moveDirection = forward * (speedX * _speed) + right * (speedY * _speed);
			_characterController.Move(_moveDirection * Time.deltaTime);

			// Apply gravity
			if (!_isGrounded)
				velocity.y += gravity * Time.deltaTime;
			else
				velocity = Vector3.zero;

			_characterController.Move(velocity * Time.deltaTime);
		}

		private void HandleJump()
		{
			// Jump if grounded and jump button pressed
			if (_jumpInput && _isGrounded && !_isGrappling) // && Input.GetButtonDown("Jump")
			{
				_animator.SetTrigger("Jump");
				velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
				_characterController.Move(velocity * Time.deltaTime);
			}
		}
		
		public void ApplyForce(Vector3 force)
		{
			phys_velocity += force / mass;
			phys_cd = phys_groundReactTime;
		}

		private void HandleRotation() { transform.rotation = Quaternion.Euler(0, _camera.rotation.eulerAngles.y, 0); }

		
		void SimulatePhysics()
		{
			if(phys_cd >= 0)
			{
				if(phys_cd <= 0)
				{
					if(_isGrounded) phys_velocity = Vector3.zero;
				}
				phys_cd -= Time.deltaTime;
			}else
			{
				if(_isGrounded) phys_velocity = Vector3.zero;
			}
			
			phys_velocity.x = Mathf.Lerp(phys_velocity.x, 0, Time.deltaTime * drag);
			phys_velocity.z = Mathf.Lerp(phys_velocity.z, 0, Time.deltaTime * drag);
			phys_velocity.y = Mathf.Lerp(phys_velocity.y, 0, Time.deltaTime * drag);
			_characterController.Move(phys_velocity * Time.deltaTime);
		}
		

		private Vector3 originalCenter;
		private float originalHeight;
		private Vector3 originalCameraPos;

		[Header("Crouch Settings")]
		public float crouchHeight = 1.0f;
		public float crouchSpeed = 3.0f;

		[Header("Slide Settings")]
		public float slideSpeed = 10.0f;
		public float slideDuration = 1.0f;
		public float slideCooldown = 2.0f;
		private float lastSlideTime;

		private bool _CrouchInput = false;
		private bool isCrouching = false;
		private bool isSliding = false;
		private float slideTimer;
		private bool isRunning;
		private static readonly int IsCrouching = Animator.StringToHash("IsCrouching");
		private static readonly int IsWalking = Animator.StringToHash("IsWalking");
		private static readonly int SpeedX = Animator.StringToHash("SpeedX");
		private static readonly int SpeedY = Animator.StringToHash("SpeedY");
		private static readonly int IsRunning = Animator.StringToHash("IsRunning");

		
		private void HandleCrouchAndSlide()
		{
			if (_CrouchInput && _speed > (_walkSpeed + _runSpeed) / 2 && !isSliding && Time.time >= lastSlideTime + slideCooldown) //Input.GetKeyDown(KeyCode.LeftControl) && 
			{
				// Start slide
				isSliding = true;
				slideTimer = slideDuration;
				_characterController.height = crouchHeight / 2; // Shorten height for slide
				_characterController.center = new Vector3(0, crouchHeight / 4, 0);
				_cameraHeadTarget.transform.localPosition = new Vector3(0, crouchHeight * 0.2f, 0);
				_animator.SetFloat("SlideAnim", UnityEngine.Random.Range(0f,1f));
				//Debug.Log("started SLIDE");
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
					//Debug.Log("ended SLIDE");
					
				}
			}

			if (_CrouchInput && !isSliding && !isCrouching) //Input.GetKeyDown(KeyCode.LeftControl) && 
			{
				//Debug.Log("started Crouch");
				// Crouch
				isCrouching = true;
				_characterController.height = crouchHeight;
				_characterController.center = new Vector3(0, crouchHeight / 2, 0);
				_cameraHeadTarget.transform.localPosition = new Vector3(0, crouchHeight * 0.8f, 0);
			}
			else if (!_CrouchInput && isCrouching) //Input.GetKeyUp(KeyCode.LeftControl) && 
			{
				// End crouch
				isCrouching = false;
				ResetHeight();
				//Debug.Log("ended Crouch");
			}
			_animator.SetBool("IsSliding", isSliding);
			_animator.SetBool(IsCrouching, isCrouching);

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
			_characterController.height = originalHeight;
			_characterController.center = originalCenter;
			_cameraHeadTarget.transform.localPosition = originalCameraPos;
		}
		
		public void SetGibbed(bool isGibbed)
		{
			if(isGibbed)
			{
				_characterController.height = 0.3f;
				_characterController.center = new Vector3(0, 0.15f, 0);
			}else
			{
				ResetHeight();
			}
			
		}
		
		public void ResetCharacterControllerVelocity() => _characterController.Move(Vector3.zero);
	}
}


//set velocity when not grounded to have horizontal
//use this velocity to rocket jump or get blasted by explosives
