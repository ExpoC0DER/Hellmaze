using System;
using System.Collections.Generic;
using _game.Scripts.Utils;
using Unity.Cinemachine;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using Utilities;

namespace _game.Scripts.Player
{
    public class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private float _speed = 5;
        [SerializeField] private float _gravity = -8.91f;
        [SerializeField] private float _jumpHeight = 3;
        [SerializeField] private float _lookSensitivity = 20f;
        [SerializeField] private bool _isGrounded;

        private CinemachinePanTilt _cinemachinePanTilt;
        private CharacterController _characterController;
        private Player _player;
        private ClientNetworkTransform _clientNetworkTransform;

        private Vector3 _playerVelocity;
        private Vector2 _playerInput;
        private Vector2 _playerLookInput;

        // Netcode general
        private NetworkTimer _networkTimer;
        private const float ServerTickRate = 60f;
        private const int BufferSize = 1024;

        // Netcode client specific
        private CircularBuffer<StatePayload> _clientStateBuffer;
        private CircularBuffer<InputPayload> _clientInputBuffer;
        private StatePayload _lastServerState;
        private StatePayload _lastProcessedState;

        // Netcode server specific
        private CircularBuffer<StatePayload> _serverStateBuffer;
        private Queue<InputPayload> _serverInputQueue;

        [Header("Netcode")]
        [SerializeField] private float _reconciliationCooldownTime = 1f;
        [SerializeField] private float _reconciliationThreshold = 10f;
        [SerializeField] private float _extrapolationLimit = 0.5f; // 500 milliseconds
        [SerializeField] private float _extrapolationMultiplier = 1.2f;
        [SerializeField] private GameObject _serverCube;
        [SerializeField] private GameObject _clientCube;

        private CountdownTimer _reconciliationCooldown;
        private StatePayload _extrapolationState;
        private CountdownTimer _extrapolationCooldown;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _player = GetComponent<Player>();
            _clientNetworkTransform = GetComponent<ClientNetworkTransform>();

            _networkTimer = new NetworkTimer(ServerTickRate);
            _clientStateBuffer = new CircularBuffer<StatePayload>(BufferSize);
            _clientInputBuffer = new CircularBuffer<InputPayload>(BufferSize);
            _serverStateBuffer = new CircularBuffer<StatePayload>(BufferSize);
            _serverInputQueue = new Queue<InputPayload>();
            _reconciliationCooldown = new CountdownTimer(_reconciliationCooldownTime);
            // _clientCube.transform.parent = null;
            // _serverCube.transform.parent = null;
            _extrapolationCooldown = new CountdownTimer(_extrapolationLimit);

            _reconciliationCooldown.OnTimerStart += () =>
            {
                _extrapolationCooldown.Stop();
            };
            
            _extrapolationCooldown.OnTimerStart += () =>
            {
                _reconciliationCooldown.Stop();
                SwitchAuthorityMode(NetworkTransform.AuthorityModes.Server);
            };

            _extrapolationCooldown.OnTimerStop += () =>
            {
                _extrapolationState = default;
                SwitchAuthorityMode(NetworkTransform.AuthorityModes.Owner);
            };
        }

        private void SwitchAuthorityMode(NetworkTransform.AuthorityModes authorityMode)
        {
            _clientNetworkTransform.AuthorityMode = authorityMode;
            bool shouldSync = authorityMode == NetworkTransform.AuthorityModes.Owner;
            _clientNetworkTransform.SyncPositionX = shouldSync;
            _clientNetworkTransform.SyncPositionY = shouldSync;
            _clientNetworkTransform.SyncPositionZ = shouldSync;
        }

        private void Start() { _cinemachinePanTilt = _player.Camera.GetComponent<CinemachinePanTilt>(); }

        private void Update()
        {
            _networkTimer.Update(Time.deltaTime);
            _reconciliationCooldown.Tick(Time.deltaTime);
            _extrapolationCooldown.Tick(Time.deltaTime);
            //ProcessRotation(_cinemachinePanTilt.PanAxis.Value);
        }

        private void FixedUpdate()
        {
            _isGrounded = Physics.Raycast(transform.position, -transform.up, 1f);
            
            while (_networkTimer.ShouldTick())
            {
                HandleClientTick();
                HandleServerTick();
            }

            Extrapolate();
        }

        private void HandleServerTick()
        {
            if (!_player.IsServer) return;

            int bufferIndex = -1;
            InputPayload inputPayload = default;
            while (_serverInputQueue.Count > 0)
            {
                inputPayload = _serverInputQueue.Dequeue();

                bufferIndex = inputPayload.Tick % BufferSize;

                StatePayload statePayload = ProcessMovement(inputPayload);
                _serverStateBuffer.Add(statePayload, bufferIndex);
            }

            if (bufferIndex == -1) return;
            SendToClientRpc(_serverStateBuffer.Get(bufferIndex));
            HandleExtrapolation(_serverStateBuffer.Get(bufferIndex), CalculateLatencyInMillis(inputPayload));
        }

        private void Extrapolate()
        {
            if (_player.IsServer && _extrapolationCooldown.IsRunning)
            {
                transform.position = _extrapolationState.Position;
                transform.rotation = _extrapolationState.Rotation;
                _playerVelocity = _extrapolationState.Velocity;
            }
        }
        
        private void HandleExtrapolation(StatePayload latest, float latency)
        {
            if (ShouldExtrapolate(latency))
            {
                if (_extrapolationState.Position != default)
                {
                    latest = _extrapolationState;
                }

                _extrapolationState.Position = latest.Position + _extrapolationState.Rotation * _extrapolationState.Velocity * (latency * _extrapolationMultiplier);
                //_extrapolationState.Rotation = Quaternion.Euler(_extrapolationState.Rotation.eulerAngles + new Vector3(0, _lookSensitivity * latency * _extrapolationMultiplier, 0));
                _extrapolationState.Rotation = latest.Rotation;
                _extrapolationState.Velocity = latest.Velocity;
                _extrapolationCooldown.Start();
            }
            else
            {
                _extrapolationCooldown.Stop();
            }
        }

        private bool ShouldExtrapolate(float latency) { return latency < _extrapolationLimit && latency > Time.fixedTime; }

        private void HandleClientTick()
        {
            if (!_player.IsClient || !_player.IsOwner) return;

            int currentTick = _networkTimer.CurrentTick;
            int bufferIndex = currentTick % BufferSize;

            InputPayload inputPayload = new InputPayload
            {
                Tick = currentTick,
                TimeStamp = DateTime.Now,
                NetworkObjectId = _player.NetworkObjectId,
                InputVector = _playerInput,
                LookVector = _playerLookInput,
                Position = transform.position
            };

            _clientInputBuffer.Add(inputPayload, bufferIndex);
            SendToServerRpc(inputPayload);

            StatePayload statePayload = ProcessMovement(inputPayload);
            _clientStateBuffer.Add(statePayload, bufferIndex);

            HandleServerReconciliation();
        }

        private void HandleServerReconciliation()
        {
            if (!ShouldReconcile()) return;

            int bufferIndex = _lastServerState.Tick % BufferSize;
            if (bufferIndex - 1 < 0) return; // Not enough information to reconcile

            StatePayload rewindState = _player.IsHost ? _serverStateBuffer.Get(bufferIndex - 1) : _lastServerState; // Host RPCs execute immediately, so we can use last server state
            float positionError = Vector3.Distance(rewindState.Position, _clientStateBuffer.Get(bufferIndex).Position);

            if (positionError > _reconciliationThreshold)
            {
                ReconcileState(rewindState);
                _reconciliationCooldown.Start();
            }

            _lastProcessedState = _lastServerState;
        }
        private void ReconcileState(StatePayload rewindState)
        {
            transform.SetPositionAndRotation(rewindState.Position, rewindState.Rotation);
            _playerVelocity = rewindState.Velocity;

            if (!rewindState.Equals(_lastServerState)) return;

            _clientStateBuffer.Add(rewindState, rewindState.Tick);

            // Replay all inputs from the rewind state to current state
            int tickToReplay = _lastServerState.Tick;

            while (tickToReplay < _networkTimer.CurrentTick)
            {
                int bufferIndex = tickToReplay % BufferSize;
                StatePayload statePayload = ProcessMovement(_clientInputBuffer.Get(bufferIndex));
                _clientStateBuffer.Add(statePayload, bufferIndex);
                tickToReplay++;
            }
        }

        private bool ShouldReconcile()
        {
            bool isNewServerState = !_lastServerState.Equals(default);
            bool isLastStateUndefinedOrDifferent = _lastProcessedState.Equals(default) || !_lastProcessedState.Equals(_lastServerState);

            return isNewServerState && isLastStateUndefinedOrDifferent && !_reconciliationCooldown.IsRunning && !_extrapolationCooldown.IsRunning;
        }

        private static float CalculateLatencyInMillis(InputPayload inputPayload) { return (DateTime.Now - inputPayload.TimeStamp).Milliseconds / 1000f; }

        [ServerRpc]
        private void SendToServerRpc(InputPayload inputPayload)
        {
            _clientCube.transform.position = new Vector3(inputPayload.Position.x, 2, inputPayload.Position.z);
            _serverInputQueue.Enqueue(inputPayload);
        }

        [ClientRpc]
        private void SendToClientRpc(StatePayload statePayload)
        {
            if (!_player.IsOwner) return;

            _serverCube.transform.position = new Vector3(statePayload.Position.x, 2, statePayload.Position.z);
            _lastServerState = statePayload;
        }

        private StatePayload ProcessMovement(InputPayload inputPayload)
        {
            Move(inputPayload.InputVector);
            Rotate(inputPayload.LookVector);

            return new StatePayload
            {
                Tick = inputPayload.Tick,
                NetworkObjectId = inputPayload.NetworkObjectId,
                Position = transform.position,
                Rotation = transform.rotation,
                Velocity = _playerVelocity
            };
        }

        private void ProcessRotation(float rotationY)
        {
            Vector3 newRotation = Vector3.zero;
            newRotation.y = rotationY;
            transform.rotation = Quaternion.Euler(newRotation);
        }

        private void Move(Vector2 input)
        {
            Vector3 moveDirection = Vector3.zero;
            moveDirection.x = input.x;
            moveDirection.z = input.y;

            _characterController.Move(transform.TransformDirection(moveDirection) * (_speed * _networkTimer.MinTimeBetweenTicks));
            if (!_isGrounded)
                _playerVelocity.y += _gravity * _networkTimer.MinTimeBetweenTicks;
            else
            {
                _playerVelocity.y = Mathf.Max(_gravity, _playerVelocity.y);
            }
            _characterController.Move(_playerVelocity * _networkTimer.MinTimeBetweenTicks);
        }

        private void Rotate(Vector2 lookInput)
        {
            _player.Camera.transform.RotateAround(_player.Camera.transform.position, _player.Camera.transform.right, -lookInput.y * 20f * _networkTimer.MinTimeBetweenTicks);
            transform.RotateAround(transform.position, transform.up, lookInput.x * _lookSensitivity * _networkTimer.MinTimeBetweenTicks);
        }

        public void Jump()
        {
            transform.position += transform.forward * 30f;
            // if (_isGrounded)
            //     _playerVelocity.y = Mathf.Sqrt(_jumpHeight * -3f * _gravity);
        }

        public void SetPlayerInput(Vector2 input, Vector2 lookInput)
        {
            _playerInput = input;
            _playerLookInput = lookInput;
        }
    }
}
