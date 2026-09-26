using System.Collections;
using Game.Scripts.Core.LevelSystem;
using Game.Scripts.Core.UISystem;
using Game.Scripts.LevelSystem;
using Game.Scripts.PlayerSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UISystem
{
    public class GameUIController : UIController
    {
        [SerializeField] private PlayerHealthUI playerHealth;
        [SerializeField] private PlayerTimeLeftUI playerTimeLeft;
        [SerializeField] private GameResultUI gameResult;
        [SerializeField] private GameScoreUI gameScore;
        [Space]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Button returnButton;
        [SerializeField] private Button restartButton;
        
        private GameLevelController gameLevel;
        private PawnPlayerController pawnPlayer;
        private Camera mainCamera;

        private bool isPlaying;
        
        public override IEnumerator EnableUI(LevelController level)
        {
            yield return base.EnableUI(level);

            gameLevel = level as GameLevelController;
            
            pawnPlayer = level.GetPlayer<PawnPlayerController>();
            pawnPlayer.GetHealth().ValueChanged += OnPlayerHealthChanged;
            OnPlayerHealthChanged(0, pawnPlayer.GetHealth().GetValue());

            mainCamera = level.GetCamera().CameraMain;
            
            playerTimeLeft.SetCanvas(canvas);
            playerTimeLeft.SetupUI(gameLevel, mainCamera);
            playerTimeLeft.EnableUI();
            
            playerHealth.SetCanvas(canvas);
            playerHealth.SetupUI(pawnPlayer, mainCamera);
            playerHealth.EnableUI();
            
            gameResult.SetupUI(gameLevel);
            gameResult.EnableUI();
            
            gameScore.SetCanvas(canvas);
            gameScore.SetupUI(pawnPlayer, gameLevel, mainCamera);
            gameScore.EnableUI();
            
            returnButton.onClick.AddListener(OnReturnButtonClick);
            restartButton.onClick.AddListener(OnRestartButtonClick);
            
            isPlaying = true;
        }
        
        public override void DisableUI()
        {
            base.DisableUI();
            
            pawnPlayer.GetHealth().ValueChanged -= OnPlayerHealthChanged;
            
            playerHealth.DisableUI();
            gameResult.DisableUI();
            gameScore.DisableUI();
            playerTimeLeft.DisableUI();
            
            returnButton.onClick.RemoveListener(OnReturnButtonClick);
            restartButton.onClick.RemoveListener(OnRestartButtonClick);
            
            isPlaying = false;
        }
        
        private void OnReturnButtonClick()
        {
            gameLevel.ReturnMenuLevel();
        }
        
        private void OnRestartButtonClick()
        {
            gameLevel.RestartLevel();
        }
        
        private void OnPlayerHealthChanged(int prev, int curr)
        {
        }

        private void Update()
        {
            if (!isPlaying) return;
            
            playerTimeLeft.UpdateUI(pawnPlayer.GetPawn().GetWorldCenter(), mainCamera, gameLevel.GetTimeLeft(), gameLevel.GetTimeMax());
        }
    }
}
