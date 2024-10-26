namespace _game.Scripts
{
    public interface IGun
    {
        public int Ammo { get; set; }
        public void Shoot(bool keyDown);
    }
}
