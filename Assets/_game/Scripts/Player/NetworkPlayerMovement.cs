using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace _game.Scripts.Player
{
    public class NetworkPlayerMovement : NetworkBehaviour
    {
        [SerializeField] private CharacterController _cc;
        [SerializeField] private NetworkPlayerAnimations _playerAnimator;

        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _sprintSpeed = 15f;
        [SerializeField] private float _turnSpeed = 30f;
        [SerializeField] private float _gravity = -8.91f;
        [SerializeField] private float _jumpHeight = 2f;

        [SerializeField] private Transform _camSocket;
        [FormerlySerializedAs("_vcam")]
        [SerializeField] private GameObject _vCam;

        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private Color _color;

        private Transform _vCamTransform;

        private int _tick;
        private const float TICK_RATE = 1f / 128f;
        private float _tickDeltaTime;

        private const int BUFFER_SIZE = 1024;
        private readonly InputState[] _inputStates = new InputState[BUFFER_SIZE];
        private readonly TransformState[] _transformStates = new TransformState[BUFFER_SIZE];

        private readonly NetworkVariable<TransformState> _serverTransformState = new NetworkVariable<TransformState>();
        private TransformState _previousTransformState;

        private Vector3 _playerVelocity;

        private int _lastProcessedTick;
        

        private void OnEnable() { _serverTransformState.OnValueChanged += OnServerStateChanged; }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _vCamTransform = _vCam.transform;
        }

        private void OnServerStateChanged(TransformState previousState, TransformState serverState)
        {
            if (!IsLocalPlayer) return;

            if (_previousTransformState == null)
            {
                _previousTransformState = serverState;
            }

            TransformState calculatedState = _transformStates.First(localState => localState.Tick == serverState.Tick);
            if (calculatedState.Position != serverState.Position)
            {
                Debug.Log("Correcting client position");
                //Teleport the player to the server position
                TeleportPlayer(serverState);
                //Replay the inputs that happened after
                IEnumerable<InputState> inputs = _inputStates.Where(input => input.Tick > serverState.Tick);
                inputs = from input in inputs orderby input.Tick select input;

                foreach (InputState inputState in inputs)
                {
                    _playerVelocity = inputState.Velocity;
                    MovePlayer(inputState.MovementInput, inputState.HasJumped, inputState.HoldingSprint);
                    RotatePlayer(inputState.LookInput);

                    TransformState newTransformState = new TransformState
                    {
                        Tick = inputState.Tick,
                        Position = transform.position,
                        Rotation = transform.rotation,
                        HasStartedMoving = true
                    };

                    for(int i = 0; i < _transformStates.Length; i++)
                    {
                        if (_transformStates[i].Tick == inputState.Tick)
                        {
                            _transformStates[i] = newTransformState;
                            break;
                        }
                    }
                }
            }
        }

        public void TeleportPlayer(TransformState state)
        {
            _cc.enabled = false;
            transform.position = state.Position;
            transform.rotation = state.Rotation;
            _cc.enabled = true;

            for(int i = 0; i < _transformStates.Length; i++)
            {
                if (_transformStates[i] != null && _transformStates[i].Tick == state.Tick)
                {
                    _transformStates[i] = state;
                    break;
                }
            }
        }

        public void ProcessLocalPlayerMovement(Vector2 movementInput, Vector2 lookInput, bool jumpedThisFrame, bool sprintHeld)
        {
            _tickDeltaTime += Time.deltaTime;
            if (_tickDeltaTime > TICK_RATE)
            {
                int bufferIndex = _tick % BUFFER_SIZE;

                if (!IsServer)
                {
                    MovePlayerServerRpc(_tick, movementInput, lookInput, jumpedThisFrame, sprintHeld);
                    MovePlayer(movementInput, jumpedThisFrame, sprintHeld);
                    AnimateMovement(movementInput);
                    RotatePlayer(lookInput);
                    SaveState(movementInput, lookInput, _playerVelocity, jumpedThisFrame, sprintHeld, bufferIndex);
                }
                else
                {
                    MovePlayer(movementInput, jumpedThisFrame, sprintHeld);
                    AnimateMovement(movementInput);
                    RotatePlayer(lookInput);

                    TransformState state = new TransformState
                    {
                        Tick = _tick,
                        Position = transform.position,
                        Rotation = transform.rotation,
                        HasStartedMoving = true
                    };

                    SaveState(movementInput, lookInput, _playerVelocity, jumpedThisFrame, sprintHeld, bufferIndex);

                    _previousTransformState = _serverTransformState.Value;
                    _serverTransformState.Value = state;
                }

                _tickDeltaTime -= TICK_RATE;
                _tick++;
            }
        }

        private void AnimateMovement(Vector2 movementInput)
        {
            _playerAnimator.AnimateMovement(movementInput);
        }

        public void ProcessSimulatedPlayerMovement()
        {
            _tickDeltaTime += Time.deltaTime;
            if (_tickDeltaTime > TICK_RATE)
            {
                if (_serverTransformState.Value is { HasStartedMoving: true }) // (_serverTransformState.Value.HasStartedMoving)
                {
                    transform.position = _serverTransformState.Value.Position;
                    transform.rotation = _serverTransformState.Value.Rotation;
                }

                _tickDeltaTime -= TICK_RATE;
                _tick++;
            }
        }

        private void SaveState(Vector2 movementInput, Vector2 lookInput, Vector3 velocity, bool hasJumped, bool holdingSprint, int bufferIndex)
        {
            InputState inputState = new InputState
            {
                Tick = _tick,
                MovementInput = movementInput,
                LookInput = lookInput,
                Velocity = velocity,
                HasJumped = hasJumped,
                HoldingSprint = holdingSprint,
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
        }

        private void MovePlayer(Vector2 movementInput, bool hasJumped, bool holdingSprint)
        {
            Vector3 moveDirection = Vector3.zero;
            moveDirection.x = movementInput.x;
            moveDirection.z = movementInput.y;

            float finalSpeed = holdingSprint ? _sprintSpeed : _speed;

            _cc.Move(transform.TransformDirection(moveDirection) * (finalSpeed * TICK_RATE));

            Vector3 feetPos = transform.position - Vector3.up * 1f;
            bool isGrounded = Physics.Raycast(feetPos, -transform.up, 0.2f);
            Debug.DrawLine(feetPos, feetPos - transform.up * 0.2f);

            if (!isGrounded)
                _playerVelocity.y += _gravity * TICK_RATE;
            else
                _playerVelocity.y = Mathf.Max(_gravity, _playerVelocity.y);

            if (hasJumped && isGrounded)
                _playerVelocity.y = Mathf.Sqrt(_jumpHeight * -3f * _gravity);

            _cc.Move(_playerVelocity * TICK_RATE);
        }


        private void RotatePlayer(Vector2 lookInput)
        {
            RotateCameraClamped(_vCamTransform, lookInput, -80f, 80f, _turnSpeed * TICK_RATE);
            //_vCamTransform.RotateAround(_vCamTransform.position, _vCamTransform.right, -lookInput.y * _turnSpeed * TICK_RATE);
            transform.RotateAround(transform.position, transform.up, lookInput.x * _turnSpeed * TICK_RATE);
        }

        private static void RotateCameraClamped(Transform cameraTransform, Vector2 input, float minAngle, float maxAngle, float speed)
        {
            // Get current X rotation
            Vector3 angles = cameraTransform.localEulerAngles;
            angles.x += -input.y * speed;

            // Convert to range -180 to 180 for proper clamping
            if (angles.x > 180f) angles.x -= 360f;

            // Clamp rotation
            angles.x = Mathf.Clamp(angles.x, minAngle, maxAngle);

            // Apply clamped rotation
            cameraTransform.localEulerAngles = angles;
        }

        [ServerRpc]
        private void MovePlayerServerRpc(int tick, Vector2 movementInput, Vector2 lookInput, bool hasJumped, bool holdingSprint)
        {
            if (_lastProcessedTick + 1 != tick)
            {
                Debug.Log("I missed a tick");
                Debug.Log($"Received Tick {tick}");
            }

            _lastProcessedTick = tick;
            MovePlayer(movementInput, hasJumped, holdingSprint);
            RotatePlayer(lookInput);

            TransformState state = new TransformState
            {
                Tick = tick,
                Position = transform.position,
                Rotation = transform.rotation,
                HasStartedMoving = true
            };


            _previousTransformState = _serverTransformState.Value;
            _serverTransformState.Value = state;
        }

        private void OnDrawGizmos()
        {
            if (_serverTransformState.Value != null)
            {
                Gizmos.color = _color;
                Gizmos.DrawMesh(_meshFilter.mesh, _serverTransformState.Value.Position);
            }
        }
    }


    public class InputState
    {
        public int Tick;
        public Vector2 MovementInput;
        public Vector2 LookInput;
        public Vector3 Velocity;
        public bool HasJumped;
        public bool HoldingSprint;
    }

    public class TransformState : INetworkSerializable, IEquatable<TransformState>
    {
        public int Tick;
        public Vector3 Position;
        public Quaternion Rotation;
        public bool HasStartedMoving;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref Rotation);
            serializer.SerializeValue(ref HasStartedMoving);
        }
        public bool Equals(TransformState other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Tick == other.Tick && Position.Equals(other.Position) && Rotation.Equals(other.Rotation) && HasStartedMoving == other.HasStartedMoving;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((TransformState)obj);
        }
        public override int GetHashCode() { return HashCode.Combine(Tick, Position, Rotation, HasStartedMoving); }
    }
}
