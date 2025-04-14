namespace _game.Scripts.Definitions
{
    public static class UIEvents
    {
        public delegate void MusicIndexChanged(int index);
        public static MusicIndexChanged OnMusicIndexChanged;
        
        public delegate void PlayerKill(string killerPlayerId, string deadPlayerId, int weaponIndex);
        public static PlayerKill OnPlayerKill;
    }
}
