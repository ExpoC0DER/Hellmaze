using Unity.Netcode;
using UnityEngine;

namespace _game.Scripts.Player
{
    [RequireComponent(typeof(PlayerMotor))]
    public class InputManager : NetworkBehaviour
    {
        private PlayerInput _playerInput;
        private PlayerInput.OnFootActions _onFoot;
        private PlayerMotor _playerMotor;
        private NetworkMovement _networkMovement;

        private void Awake()
        {
            _playerInput = new PlayerInput();
            _onFoot = _playerInput.OnFoot;
            _playerMotor = GetComponent<PlayerMotor>();
            _networkMovement = GetComponent<NetworkMovement>();

            _onFoot.Jump.performed += ctx => _playerMotor.Jump();
        }

        // private void Update()
        // {
        //     print(_onFoot.Look.ReadValue<Vector2>());
        // }

        private void Update()
        {
            _playerMotor.SetPlayerInput(_onFoot.Movement.ReadValue<Vector2>(), _onFoot.Look.ReadValue<Vector2>());
            // if (IsClient && IsLocalPlayer)
            // {
            //     _networkMovement.ProcessLocalPlayerMovement(_onFoot.Movement.ReadValue<Vector2>(), _onFoot.Look.ReadValue<Vector2>());
            // }
            // else
            // {
            //     _networkMovement.ProcessSimulatedPlayerMovement();
            // }
        }

        private void OnEnable() { _onFoot.Enable(); }

        private void OnDisable() { _onFoot.Disable(); }
    }
}
