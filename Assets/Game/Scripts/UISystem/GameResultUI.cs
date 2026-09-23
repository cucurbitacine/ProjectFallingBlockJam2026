using System.Collections.Generic;
using Game.Scripts.LevelSystem;
using UnityEngine;

namespace Game.Scripts.UISystem
{
    public class GameResultUI : MonoBehaviour
    {
        [SerializeField] private GameObject winSplashScreen;
        [SerializeField]
        private Dictionary<FailReason, GameObject> failSplashScreens = new Dictionary<FailReason, GameObject>();
        
        private GameLevelController _level;
        
        public void SetupUI(GameLevelController level)
        {
            _level = level;
        }

        public void EnableUI()
        {
            _level.GameStateChanged += OnGameStateChanged;
            _level.GameFailed += OnGameFailed;
        }

        public void DisableUI()
        {
            _level.GameStateChanged -= OnGameStateChanged;
            _level.GameFailed -= OnGameFailed;
        }

        private void OnGameStateChanged(GameState exit, GameState enter)
        {
            if (enter is GameState.Fail)
            {
                
            }
            else if (enter is GameState.Win)
            {
                winSplashScreen.SetActive(true);
            }
        }
        
        private void OnGameFailed(FailReason failReason)
        {
            if (failSplashScreens.TryGetValue(failReason, out var splashScreen))
            {
                splashScreen.SetActive(true);
            }
        }
    }
}