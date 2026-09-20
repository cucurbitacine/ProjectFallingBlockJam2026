using System;
using System.Collections;
using Game.Scripts.Core.LevelSystem;
using Game.Scripts.PlayerSystem;
using Game.Scripts.WorldSystems;
using UnityEngine;

namespace Game.Scripts.LevelSystem
{
    public class GameLevelController : LevelController
    {
        [Header("Game Level")]
        [SerializeField] private GameState gameState = GameState.Unknown;

        [Min(0)]
        [SerializeField] private int currentHeight = 0;
        [Min(0)]
        [SerializeField] private int figureSpawnHeight = 20;
        
        
        [Header("Settings")]
        [Min(0f)]
        [SerializeField] private float gameTick = 0.5f;
        [Min(0f)]
        [SerializeField] private float restBetweenFigures = 1f;
        
        [Header("Required")]
        [SerializeField] private LevelSceneAsset menuLevelScene;

        private bool isPlaying;
        private float tickTimeout = 0f;
        private float restTimeout = 0f;
        
        private PawnPlayerController pawnPlayer;
        private GridWorldController gridWorld;
        
        public event Action<GameState, GameState> GameStateChanged; 
        
        [ContextMenu(nameof(ReturnMenuLevel))]
        public void ReturnMenuLevel()
        {
            LevelManager.Instance.LoadSceneAsync(menuLevelScene);
        }

        public GameState GetGameState()
        {
            return gameState;
        }
        
        protected override IEnumerator EnableWorld()
        {
            yield return base.EnableWorld();
            
            gridWorld = this.GetWorld<GridWorldController>();
        }

        protected override IEnumerator EnablePlayer()
        {
            yield return base.EnablePlayer();

            pawnPlayer = this.GetPlayer<PawnPlayerController>();
            pawnPlayer.PlayerTransform.Get().position = gridWorld.GetSpawnPoint();
        }

        protected override IEnumerator EnableLevel()
        {
            ChangeState(GameState.Initializing);
            
            yield return base.EnableLevel();
            
            isPlaying = true;
            
            ChangeState(GameState.Falling);
        }

        private void ChangeState(GameState nextState)
        {
            if (gameState == nextState) return;

            var prevState = gameState;
            gameState = nextState;

            OnGameStateChanged(prevState, nextState);
            
            GameStateChanged?.Invoke(prevState, nextState);
        }
        
        private void OnGameStateChanged(GameState prevState, GameState nextState)
        {
            if (nextState == GameState.Falling)
            {
                tickTimeout = gameTick;
                gridWorld.LaunchFallingFigure(currentHeight + figureSpawnHeight);
            }
            else if (nextState == GameState.Consequence)
            {
                restTimeout = restBetweenFigures;
                gridWorld.MergeFallingFigure();
                gridWorld.CheckFullLines();
            }
        }
        
        private void UpdateGame(GameState state, float deltaTime)
        {
            if (state == GameState.Falling)
            {
                UpdateFalling(deltaTime);
            }
            else if (state == GameState.Consequence)
            {
                UpdateConsequence(deltaTime);
            }
        }
        
        private void UpdateConsequence(float deltaTime)
        {
            if (restTimeout <= 0f)
            {
                ChangeState(GameState.Falling);
            }
            else
            {
                restTimeout -= deltaTime;
            }
        }

        private void UpdateFalling(float deltaTime)
        {
            if (tickTimeout <= 0f)
            {
                var offset = -1;
                if (gridWorld.IsPossibleMoveY(offset))
                {
                    gridWorld.MoveY(offset);
                    tickTimeout = gameTick;
                }
                else
                {
                    ChangeState(GameState.Consequence);
                }
            }
            else
            {
                tickTimeout -= deltaTime;
            }
        }
        
        private void Update()
        {
            UpdateGame(GetGameState(), Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            if (isPlaying)
            {
                var playerPosition = pawnPlayer.PlayerTransform.Get().position;
                var cell = gridWorld.GetWorldGrid().WorldToCell(playerPosition);
                var playerBlockCenter = gridWorld.GetWorldGrid().GetCellCenterWorld(cell);
                
                Gizmos.DrawWireCube(playerBlockCenter, gridWorld.GetWorldGrid().cellSize);
            }
        }
    }

    public enum GameState
    {
        Unknown,
        Initializing,
        Falling,
        Consequence,
        Fail,
        Success,
    }
}
