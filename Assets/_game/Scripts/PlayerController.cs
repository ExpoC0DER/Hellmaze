using AYellowpaper;
using UnityEngine;
using UnityEngine.Serialization;

namespace _game.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [FormerlySerializedAs("walkSpeed")]
        [SerializeField] private float _walkSpeed = 6f;
        [FormerlySerializedAs("runSpeed")]
        [SerializeField] private float _runSpeed = 12f;
        [FormerlySerializedAs("jumpPower")]
        [SerializeField] private float _jumpPower = 7f;
        [FormerlySerializedAs("gravity")]
        [SerializeField] private float _gravity = 10f;

        [SerializeField] private Transform _camera;
        [SerializeField] private InterfaceReference<IGun> _gun;

        private Vector3 _moveDirection = Vector3.zero;
        private bool _canMove = true;
        private CharacterController _characterController;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            #region Handles Movment
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            // Press Left Shift to run
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = _canMove ? (isRunning ? _runSpeed : _walkSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = _canMove ? (isRunning ? _runSpeed : _walkSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = _moveDirection.y;
            _moveDirection = (forward * curSpeedX) + (right * curSpeedY);
            #endregion

            #region Handles Jumping
            if (Input.GetButton("Jump") && _canMove && _characterController.isGrounded)
            {
                _moveDirection.y = _jumpPower;
            }
            else
            {
                _moveDirection.y = movementDirectionY;
            }

            if (!_characterController.isGrounded)
            {
                _moveDirection.y -= _gravity * Time.deltaTime;
            }
            #endregion

            #region Handles Rotation
            _characterController.Move(_moveDirection * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, _camera.rotation.eulerAngles.y, 0);
            #endregion

            #region Handles Shooting
            if (Input.GetMouseButtonDown(0))
                _gun.Value.Shoot();
            #endregion
        }
    }
}
