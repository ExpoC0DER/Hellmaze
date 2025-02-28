using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace _game.Scripts.Controllers_Managers
{
    public class GameManager : MonoBehaviour
    {
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
    }
}
