using System.Linq;
using System.Threading.Tasks;
using _game.Scripts.Utils;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace _game.Scripts.Controllers_Managers
{
    public class RelayManager : Singleton<RelayManager>
    {
        private string _joinCode;
        private string _ip;
        private int _port;
        private byte[] _key;
        private byte[] _connectionData;
        private byte[] _hostConnectionData;
        private System.Guid _allocationId;
        private byte[] _allocationIdBytes;
        public bool IsHost { get; private set; }

        public string GetAllocationId() { return _allocationId.ToString(); }
        public string GetConnectionData() { return _connectionData.ToString(); }

        public async Task<string> CreateRelay(int maxConnection)
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerEndpoint dtlsEndpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
            _ip = dtlsEndpoint.Host;
            _port = dtlsEndpoint.Port;

            _allocationId = allocation.AllocationId;
            _allocationIdBytes = allocation.AllocationIdBytes;
            _connectionData = allocation.ConnectionData;
            _key = allocation.Key;

            IsHost = true;

            return _joinCode;
        }

        public async Task<bool> JoinRelay(string joinCode)
        {
            _joinCode = joinCode;
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerEndpoint dtlsEndpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
            _ip = dtlsEndpoint.Host;
            _port = dtlsEndpoint.Port;

            _allocationId = allocation.AllocationId;
            _allocationIdBytes = allocation.AllocationIdBytes;
            _connectionData = allocation.ConnectionData;
            _key = allocation.Key;
            _hostConnectionData = allocation.HostConnectionData;

            return true;
        }

        public (byte[] allocationId, byte[] key, byte[] connectionData, string dtlsAddress, int dtlsPort) GetHostConnectionInfo() { return (_allocationIdBytes, _key, _connectionData, _ip, _port); }

        public (byte[] allocationId, byte[] key, byte[] connectionData, byte[] hostConnectionData, string dtlsAddress, int dtlsPort) GetClientConnectionInfo() { return (_allocationIdBytes, _key, _connectionData, _hostConnectionData, _ip, _port); }
    }
}
