namespace _game.Scripts.Definitions
{
    public static class LobbyEvents
    {
        public delegate void LobbyUpdated(Unity.Services.Lobbies.Models.Lobby lobby);

        public static LobbyUpdated OnLobbyUpdated;

        public delegate void LobbyDestroyed();
        public static LobbyDestroyed OnLobbyDestroyed;
    }
}
