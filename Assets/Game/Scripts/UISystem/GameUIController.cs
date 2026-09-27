using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Core.LevelSystem;
using Game.Scripts.Core.UISystem;
using Game.Scripts.LevelSystem;
using Game.Scripts.PlayerSystem;
using Game.Scripts.SoundSystem;
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
        [SerializeField] private NextFigureUI nextFigure;
        [Space]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Button returnButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Image blackoutImage;
        
        [Space]
        [SerializeField] private SoundSource soundSource;
        [SerializeField] private SoundFxPreset buttonHoverSfx;
        [SerializeField] private SoundFxPreset buttonClickSfx;
        
        private GameLevelController gameLevel;
        private PawnPlayerController pawnPlayer;
        private Camera mainCamera;

        private readonly List<Button> buttons = new List<Button>();
        private readonly List<HoverEvent> hovers = new List<HoverEvent>();
        
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
            
            nextFigure.SetupUI(gameLevel);
            nextFigure.EnableUI();
            
            returnButton.onClick.AddListener(OnReturnButtonClick);
            restartButton.onClick.AddListener(OnRestartButtonClick);
            
            blackoutImage.enabled = false;
            
            GetComponentsInChildren<Button>(true, buttons);
            hovers.AddRange(buttons.Select(b => b.GetComponent<HoverEvent>()).Where(h => h != null));

            foreach (var button in buttons)
            {
                button.onClick.AddListener(OnButtonClick);
            }
            
            foreach (var hover in hovers)
            {
                hover.onEnter.AddListener(OnButtonHover);
            }
            
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
            nextFigure.DisableUI();
            
            returnButton.onClick.RemoveListener(OnReturnButtonClick);
            restartButton.onClick.RemoveListener(OnRestartButtonClick);

            foreach (var button in buttons)
            {
                button.onClick.RemoveListener(OnButtonClick);
            }
            
            foreach (var hover in hovers)
            {
                hover.onEnter.RemoveListener(OnButtonHover);
            }
            
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

        private void OnButtonClick()
        {
            soundSource.Play(buttonClickSfx);
        }
        
        private void OnButtonHover()
        {
            soundSource.Play(buttonHoverSfx);
        }
        
        private void Start()
        {
            blackoutImage.enabled = true;
        }

        private void Update()
        {
            if (!isPlaying) return;
            
            playerTimeLeft.UpdateUI(pawnPlayer.GetPawn().GetWorldCenter(), mainCamera, gameLevel.GetTimeLeft(), gameLevel.GetTimeMax());
        }
    }
}
