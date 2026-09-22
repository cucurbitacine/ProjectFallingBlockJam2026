using System.Collections;
using Game.Scripts.Core.LevelSystem;
using Game.Scripts.Core.PlayerSystem;
using Game.Scripts.Core.WorldSystem;
using Game.Scripts.LevelSystem;
using UnityEngine;

namespace Game.Scripts.CameraSystem
{
    public class TowerCameraController : CameraController
    {
        [SerializeField] private Transform followAnchor;

        private bool isPlaying;
        private GameLevelController gameLevel;
        
        public override IEnumerator EnableCamera(PlayerController player, WorldController world = null, LevelController level = null)
        {
            yield return base.EnableCamera(player, world, level);

            if (level && level is GameLevelController tLevel)
            {
                gameLevel = tLevel;

                UpdateFollowPosition();

                gameLevel.GameStateChanged += OnGameStateChanged;
                
                isPlaying = true;
            }
        }

        public override void DisableCamera()
        {
            isPlaying = false;
            gameLevel.GameStateChanged -= OnGameStateChanged;
            
            base.DisableCamera();
        }

        private void UpdateFollowPosition()
        {
            if (gameLevel.GetGameState() is GameState.Fail)
            {
                followAnchor.transform.position = gameLevel.GetPlayer().PlayerTransform.Get().position;
            }
            else
            {
                var y = gameLevel.GetCurrentHeight();
                followAnchor.transform.position = Vector3.up * y;
            }
        }
        
        private void OnGameStateChanged(GameState exit, GameState enter)
        {
        }
        
        private void LateUpdate()
        {
            if (!isPlaying) return;

            UpdateFollowPosition();
        }
    }
}