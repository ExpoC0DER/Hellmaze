using System;
using System.Linq;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using UnityEngine;
namespace _game.Scripts
{
    public class MppmConnect : MonoBehaviour
    {
        private void Start()
        {
            string[] mppmTag = CurrentPlayer.ReadOnlyTags();
            if (mppmTag.Contains("Server"))
            {
                NetworkManager.Singleton.StartServer();
            }
            else if (mppmTag.Contains("Host"))
            {
                NetworkManager.Singleton.StartHost();
            }
            else if (mppmTag.Contains("Client"))
            {
                NetworkManager.Singleton.StartClient();
            }
            
        }
    }
}
