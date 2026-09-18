namespace Game.Scripts.Core
{
    public class GameManager
    {
        public static GameManager Instance { get; private set; }

        static GameManager()
        {
            Instance = new GameManager();
        }
    }
}