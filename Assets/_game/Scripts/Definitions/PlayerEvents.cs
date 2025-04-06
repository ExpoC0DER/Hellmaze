namespace _game.Scripts.Definitions
{
    public static class PlayerEvents
    {
        public delegate void PlayerShot(int ammoLeft);
        public static PlayerShot OnPlayerShot;
    }
}
