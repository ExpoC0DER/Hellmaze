namespace _game.Scripts.Definitions
{
    public static class GameLobbyEvents
    {
        public delegate void GameLobbyUpdated();
        public static GameLobbyUpdated OnGameLobbyUpdated;
        
        public delegate void GameLobbyReady();
        public static GameLobbyReady OnGameLobbyReady;
    }
}
