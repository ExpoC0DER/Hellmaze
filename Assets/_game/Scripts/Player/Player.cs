using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace _game.Scripts.Player
{
    public class Player : NetworkBehaviour
    {
        [field: SerializeField] public CinemachineCamera Camera { get; private set; }
        [SerializeField, HideInInspector] private CinemachineCamera _cameraPrefab;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;

            if (!Camera)
            {
                Camera = Instantiate(_cameraPrefab);
                Camera.Follow = transform;
            }

            //Camera.transform.parent = null;

            if (!IsOwner)
            {
                Camera.Priority = 0;
            }
            else
            {
                Camera.Priority = 100;
            }
        }

        // Update is called once per frame
        private void Update() { }
    }
}
