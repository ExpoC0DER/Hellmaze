using System;
using AYellowpaper;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Serialization;

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
		[SerializeField] private Animator _animator;
		[SerializeField] private Transform _cameraHeadTarget;
		[SerializeField] private Rigidbody _cameraHeadTarget_rb;
		[SerializeField] private Collider _cameraHeadTarget_col;

		[SerializeField] private Transform _camera;
		[SerializeField] private Transform _model;
		
		private bool isDead;
		
		private InterfaceReference<IGun, MonoBehaviour> _gun;
		private Vector3 _moveDirection = Vector3.zero;
		private bool _canMove = true;
		private CharacterController _characterController;
		private float _targetSpeed;
		private float _speed;
		private bool _isGrounded;
		private bool _isGrappling;

		[Header("Jump Settings")]
		public float jumpHeight = 2.0f;
		public float gravity = -9.81f;

		private Vector3 velocity;

		private void Start()
		{
			_characterController = GetComponent<CharacterController>();
			originalHeight = _characterController.height;
			originalCenter = _characterController.center;
			originalCameraPos = _cameraHeadTarget.transform.localPosition;

			//_gun = _gun1;

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			
			playerStats.OnDeath += OnDeath;
			playerStats.OnRespawn += OnRespawn;
		}

		private void Update()
		{
			CheckGrounded();
			
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
			if (!_isGrappling && Input.GetMouseButtonDown(1))
			{
				if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit))
				{
					if (hit.transform.TryGetComponent(out GrapplePoint grapplePoint))
					{
						_hitPos = grapplePoint.ArrivalPos;
						_isGrappling = true;
					}
				}
			}

			if (_isGrappling)
			{
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

		private void CheckGrounded() { _isGrounded = Physics.Raycast(transform.position, -transform.up, 0.25f); }

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
			isRunning = Input.GetKey(KeyCode.LeftShift);
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

			float speedX = Input.GetAxis("Vertical");
			float speedY = Input.GetAxis("Horizontal");
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
			if (_isGrounded && Input.GetButtonDown("Jump"))
			{
				_animator.SetTrigger("Jump");
				velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
				_characterController.Move(velocity * Time.deltaTime);
			}
		}

		private void HandleRotation() { transform.rotation = Quaternion.Euler(0, _camera.rotation.eulerAngles.y, 0); }

		/* private void HandleShooting()
		{
			bool mouseDown = Input.GetMouseButton(0);
			_gun.Value.Shoot(mouseDown);
			if (mouseDown)
				OnAmmoChange?.Invoke(_gun.Value.Ammo);
			_animator.SetBool("Shooting", mouseDown);
		} */

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
			if (Input.GetKeyDown(KeyCode.LeftControl) && _speed > (_walkSpeed + _runSpeed) / 2 && !isSliding && Time.time >= lastSlideTime + slideCooldown)
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

			if (Input.GetKeyDown(KeyCode.LeftControl) && !isSliding && !isCrouching)
			{
				//Debug.Log("started Crouch");
				// Crouch
				isCrouching = true;
				_characterController.height = crouchHeight;
				_characterController.center = new Vector3(0, crouchHeight / 2, 0);
				_cameraHeadTarget.transform.localPosition = new Vector3(0, crouchHeight * 0.8f, 0);
			}
			else if (Input.GetKeyUp(KeyCode.LeftControl) && isCrouching)
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
