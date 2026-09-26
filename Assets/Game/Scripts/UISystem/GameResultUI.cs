using System.Collections.Generic;
using Game.Scripts.LevelSystem;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UISystem
{
    public class GameResultUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text lengthResult;
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
            
            lengthResult.gameObject.SetActive(false);
        }

        public void DisableUI()
        {
            _level.GameStateChanged -= OnGameStateChanged;
            _level.GameFailed -= OnGameFailed;
        }

        private void ShowResult()
        {
            lengthResult.text = $"{_level.GetSpaghetti().GetLenght():F1}m";
            lengthResult.gameObject.SetActive(true);
        }
        
        private void OnGameStateChanged(GameState exit, GameState enter)
        {
            if (enter is GameState.Fail)
            {
                ShowResult();
            }
            else if (enter is GameState.Win)
            {
                ShowResult();
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