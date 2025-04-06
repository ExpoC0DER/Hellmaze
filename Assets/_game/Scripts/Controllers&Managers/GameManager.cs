using EditorAttributes;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtils;

namespace _game.Scripts.Controllers_Managers
{
    public class GameManager : MonoBehaviour
    {
        private const string MENU_SCENE = "MainMenu";

        private void Start()
        {
            NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
            if (RelayManager.Instance.IsHost)
            {
                NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApproval;
                (byte[] allocationId, byte[] key, byte[] connectionData, string dtlsAddress, int dtlsPort) = RelayManager.Instance.GetHostConnectionInfo();
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(dtlsAddress, (ushort)dtlsPort, allocationId, key, connectionData, true);
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                (byte[] allocationId, byte[] key, byte[] connectionData, byte[] hostConnectionData, string dtlsAddress, int dtlsPort) = RelayManager.Instance.GetClientConnectionInfo();
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(dtlsAddress, (ushort)dtlsPort, allocationId, key, connectionData, hostConnectionData, true);
                NetworkManager.Singleton.StartClient();
            }
        }

        private void ConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            response.Approved = true;
            response.CreatePlayerObject = true;
            response.Pending = false;
        }

        private void Update()
        {
            if (NetworkManager.Singleton.ShutdownInProgress)
            {
                GameLobbyManager.Instance.GoBackToLobby(true);
            }
        }

        private void OnClientConnected(ulong obj) { print(obj + " connected"); }

        private void OnClientDisconnected(ulong clientId)
        {
            if (NetworkManager.Singleton.LocalClientId == clientId)
            {
                print(clientId + " disconnected");
                NetworkManager.Singleton.Shutdown();
                SceneManager.LoadSceneAsync(MENU_SCENE);
            }
        }

        public void DisconnectClient()
        {
            print(NetworkManager.Singleton.LocalClientId + " disconnected");
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadSceneAsync(MENU_SCENE);
        }

        private void OnEnable()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            #if UNITY_EDITOR
            NetworkManager.Singleton.LogLevel = LogLevel.Developer;
            NetworkManager.Singleton.NetworkConfig.EnableNetworkLogs = true;
            #endif
        }

        private void OnDisable()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}
