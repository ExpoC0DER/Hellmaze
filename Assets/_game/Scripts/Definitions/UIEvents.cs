namespace _game.Scripts.Definitions
{
    public static class UIEvents
    {
        public delegate void MusicIndexChanged(int index);
        public static MusicIndexChanged OnMusicIndexChanged;
        
        public delegate void PlayerKill(string killerPlayer, string deadPlayer, int weaponIndex);
        public static PlayerKill OnPlayerKill;
    }
}
