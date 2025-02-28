using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace _game.Scripts.Lobby
{
    public class LobbyPlayerData
    {
        public string Id { get; private set; }
        public string Gamertag { get; private set; }
        public bool IsReady { get; set; }

        public LobbyPlayerData(string id, string gamerTag)
        {
            Id = id;
            Gamertag = gamerTag;
        }

        public LobbyPlayerData(Dictionary<string, PlayerDataObject> playerData)
        {
            UpdateState(playerData);
        }

        public void UpdateState(Dictionary<string, PlayerDataObject> playerData)
        {
            if (playerData.TryGetValue("Id", out PlayerDataObject temp))
            {
                Id = temp.Value;
            }
            if (playerData.TryGetValue("GamerTag", out temp))
            {
                Gamertag = temp.Value;
            }
            if (playerData.TryGetValue("IsReady", out temp))
            {
                IsReady = temp.Value.Equals("True");
            }
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>()
            {
                { "Id", Id },
                { "GamerTag", Gamertag },
                { "IsReady", IsReady.ToString() }
            };
        }
    }
}
