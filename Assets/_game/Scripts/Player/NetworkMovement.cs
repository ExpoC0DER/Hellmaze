using System;
using JetBrains.Annotations;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace _game.Scripts.Player
{
    public class NetworkMovement : NetworkBehaviour
    {
        [SerializeField] private CharacterController _cc;

        [SerializeField] private float _speed;
        [SerializeField] private float _turnSpeed;

        [SerializeField] private Transform _camSocket;
        [SerializeField] private GameObject _vCam;

        private Transform _vCamTransform;

        private int _tick = 0;
        private float _tickRate = 1 / 60f;
        private float _tickDeltaTime = 0f;

        private const int BufferSize = 1024;
        private InputState[] _inputStates = new InputState[BufferSize];
        private TransformState[] _transformStates = new TransformState[BufferSize];

        public NetworkVariable<TransformState> ServerTransformState = new NetworkVariable<TransformState>();
        private TransformState _previousTransformState;

        private void OnEnable() { ServerTransformState.OnValueChanged += OnServerStateChanged; }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _vCamTransform = _vCam.transform;
        }

        // Handle Reconciliation
        private void OnServerStateChanged(TransformState previousValue, TransformState newValue) { _previousTransformState = previousValue; }

        public void ProcessLocalPlayerMovement(Vector2 movementInput, Vector2 lookInput)
        {
            _tickDeltaTime += Time.deltaTime;
            if (_tickDeltaTime > _tickRate)
            {
                int bufferIndex = _tick % BufferSize;
                if (!IsServer)
                {
                    MovePlayerServerRpc(_tick, movementInput, lookInput);
                    MovePlayer(movementInput);
                    RotatePlayer(lookInput);
                }
                else
                {
                    MovePlayer(movementInput);
                    RotatePlayer(lookInput);

                    TransformState state = new TransformState
                    {
                        Tick = _tick,
                        Position = transform.position,
                        Rotation = transform.rotation,
                        HasStartedMoving = true
                    };

                    _previousTransformState = ServerTransformState.Value;
                    ServerTransformState.Value = state;
                }

                InputState inputState = new InputState
                {
                    Tick = _tick,
                    MovementInput = movementInput,
                    LookInput = lookInput
                };

                TransformState transformState = new TransformState
                {
                    Tick = _tick,
                    Position = transform.position,
                    Rotation = transform.rotation,
                    HasStartedMoving = true
                };

                _inputStates[bufferIndex] = inputState;
                _transformStates[bufferIndex] = transformState;

                _tickDeltaTime -= _tickRate;
                _tick++;
            }
        }

        public void ProcessSimulatedPlayerMovement()
        {
            _tickDeltaTime += Time.deltaTime;
            if (_tickDeltaTime > _tickRate)
            {
                if (ServerTransformState.Value != null && ServerTransformState.Value.HasStartedMoving)
                {
                    transform.position = ServerTransformState.Value.Position;
                    transform.rotation = ServerTransformState.Value.Rotation;
                }

                _tickDeltaTime -= _tickRate;
                _tick++;
            }
        }

        private void MovePlayer(Vector2 movementInput)
        {
            Vector3 movement = movementInput.x * _vCamTransform.right + movementInput.y * _vCamTransform.forward;
            movementInput.y = 0;
            if (!_cc.isGrounded)
            {
                movementInput.y = -8.91f;
            }

            _cc.Move(movement * (_speed * _tickRate));
        }

        private void RotatePlayer(Vector2 lookInput)
        {
            _vCamTransform.RotateAround(_vCamTransform.position, _vCamTransform.right, -lookInput.y * _turnSpeed * _tickRate);
            transform.RotateAround(transform.position, transform.up, lookInput.x * _turnSpeed * _tickRate);
            //Camera.main.GetComponent<CinemachineBrain>().ManualUpdate();
        }

        [ServerRpc]
        private void MovePlayerServerRpc(int tick, Vector2 movementInput, Vector2 lookInput)
        {
            // // Handle lost input
            // if (_tick != _previousTransformState.Tick + 1)
            // {
            //     // Lost some messages
            // }

            MovePlayer(movementInput);
            RotatePlayer(lookInput);

            TransformState state = new TransformState
            {
                Tick = tick,
                Position = transform.position,
                Rotation = transform.rotation,
                HasStartedMoving = true
            };

            _previousTransformState = ServerTransformState.Value;
            ServerTransformState.Value = state;
        }
    }

    public class InputState
    {
        public int Tick;
        public Vector2 MovementInput;
        public Vector2 LookInput;
    }
}
