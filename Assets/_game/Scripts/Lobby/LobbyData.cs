using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace _game.Scripts.Lobby
{
    public class LobbyData
    {
        public int MapIndex { get; set; }
        public string RelayJoinCode { get; set; }
        public string SceneName { get; set; }

        public LobbyData(int mapIndex) { MapIndex = mapIndex; }

        public LobbyData(Dictionary<string, DataObject> lobbyData) { UpdateState(lobbyData); }

        public void UpdateState(Dictionary<string, DataObject> lobbyData)
        {
            if (lobbyData.TryGetValue("MapIndex", out DataObject dataObject))
            {
                if (dataObject != null)
                    MapIndex = int.Parse(dataObject.Value);
            }

            if (lobbyData.TryGetValue("RelayJoinCode", out dataObject))
            {
                if (dataObject != null)
                    RelayJoinCode = dataObject.Value;
            }
            
            if (lobbyData.TryGetValue("SceneName", out dataObject))
            {
                if (dataObject != null)
                    SceneName = dataObject.Value;
            }
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>
            {
                { "MapIndex", MapIndex.ToString() },
                { "RelayJoinCode", RelayJoinCode },
                { "SceneName", SceneName }
            };
        }
    }
}
