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
        [SerializeField] private float _health;
        [SerializeField] private float _walkSpeed = 6f;
        [SerializeField] private float _runSpeed = 12f;
        [SerializeField] private float _acceleration;
        [SerializeField] private Animator _animator;

        [SerializeField] private Transform _camera;
        [SerializeField] private Transform _model;
        [SerializeField] private InterfaceReference<IGun, MonoBehaviour> _gun1;
        [SerializeField] private InterfaceReference<IGun, MonoBehaviour> _gun2;
        [SerializeField] private GameObject[] _guns;

        private float Health
        {
            get { return _health; }
            set
            {
                _health = value;
                OnHealthChange?.Invoke(_health);
            }
        }

        public event Action<float> OnHealthChange;
        public event Action<int> OnAmmoChange;

        private InterfaceReference<IGun, MonoBehaviour> _gun;
        private Vector3 _moveDirection = Vector3.zero;
        private bool _canMove = true;
        private CharacterController _characterController;
        private float _targetSpeed;
        private float _speed;
        private bool _isGrounded;

        [Header("Jump Settings")]
        public float jumpHeight = 2.0f;
        public float gravity = -9.81f;

        private Vector3 velocity;

        [Button("Take Damage")]
        private void TakeDmg() { Health -= 10; }


        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            originalHeight = _characterController.height;
            originalCenter = _characterController.center;

            _gun = _gun1;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _guns[0].SetActive(true);
                _guns[1].SetActive(false);
                _gun = _gun1;
                OnAmmoChange?.Invoke(_gun.Value.Ammo);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _guns[0].SetActive(false);
                _guns[1].SetActive(true);
                _gun = _gun2;
                OnAmmoChange?.Invoke(_gun.Value.Ammo);
            }

            CheckGrounded();
            HandleMovement();
            HandleCrouchAndSlide();
            HandleJump();
            HandleRotation();
            HandleShooting();
        }

        private void CheckGrounded() { _isGrounded = Physics.Raycast(transform.position, -transform.up, 0.1f); }

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
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            _animator.SetBool(IsRunning, isRunning);

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
            _animator.SetBool(IsWalking, Mathf.Abs(speedX) > 0.1f || Mathf.Abs(speedY) > 0.1f);

            _moveDirection = forward * (speedX * _speed) + right * (speedY * _speed);
            _characterController.Move(_moveDirection * Time.deltaTime);

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
            _characterController.Move(velocity * Time.deltaTime);
        }

        private void HandleJump()
        {
            // Jump if grounded and jump button pressed
            if (_isGrounded && Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

        }

        private void HandleRotation() { transform.rotation = Quaternion.Euler(0, _camera.rotation.eulerAngles.y, 0); }

        private void HandleShooting()
        {
            bool mouseDown = Input.GetMouseButton(0);
            _gun.Value.Shoot(mouseDown);
            if (mouseDown)
                OnAmmoChange?.Invoke(_gun.Value.Ammo);
        }

        private Vector3 originalCenter;
        private float originalHeight;

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
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftControl) && !isSliding && !isCrouching)
            {
                // Crouch
                isCrouching = true;
                _characterController.height = crouchHeight;
                _characterController.center = new Vector3(0, crouchHeight / 2, 0);
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl) && isCrouching)
            {
                // End crouch
                isCrouching = false;
                ResetHeight();
            }

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
        }
    }
}
