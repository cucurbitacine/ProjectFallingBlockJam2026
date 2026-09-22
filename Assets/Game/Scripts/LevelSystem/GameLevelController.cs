using System;
using System.Collections;
using Game.Scripts.Core.LevelSystem;
using Game.Scripts.PlayerSystem;
using Game.Scripts.WorldSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Game.Scripts.LevelSystem
{
    public class GameLevelController : LevelController
    {
        [Header("Game Level")]
        [SerializeField] private GameState gameState = GameState.Unknown;
        [SerializeField] private FigureType nextFigureType;
        [SerializeField] private FigureState nextFigureState;
        
        [Min(0)]
        [SerializeField] private int currentHeight = 0;
        [Min(0)]
        [SerializeField] private int figureSpawnHeight = 20;
        
        [Header("Settings")]
        [Min(0f)]
        [SerializeField] private float gameTick = 0.5f;
        [SerializeField] private float fastGameTick = 0.1f;
        [Min(0f)]
        [SerializeField] private float restBetweenFigures = 1f;
        [Min(0f)]
        [SerializeField] private float maxFallDistance = 2.5f;
        
        [Header("Required")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private LevelSceneAsset menuLevelScene;

        [Header("Input Build")]
        [SerializeField] private InputActionReference leftAction;
        [SerializeField] private InputActionReference rightAction;
        [SerializeField] private InputActionReference rotateAction;
        [SerializeField] private InputActionReference downAction;
        
        private bool isPlaying;
        private float tickTimeout = 0f;
        private float restTimeout = 0f;
        private bool isFastFalling = false;
        
        private PawnPlayerController pawnPlayer;
        private GridWorldController gridWorld;
        
        private FigureType[] figureTypes;
        private FigureState[] figureStates;
        
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

        public float GetGameTick()
        {
            return isFastFalling ? fastGameTick : gameTick;
        }
        
        public float GetCurrentHeight()
        {
            return currentHeight;
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
            pawnPlayer.PlayerTransform.Get().position = GetSpawnPoint();
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

        private void OnGameStateChanged(GameState exitState, GameState enterState)
        {
            if (exitState is GameState.Initializing)
            {
                SetPawnCallbacks();
            }
            else if (exitState is GameState.Falling)
            {
                RemoveBuildCallbacks();
            }

            if (enterState is GameState.Initializing)
            {
                figureTypes = (FigureType[])Enum.GetValues(typeof(FigureType));
                figureStates = (FigureState[])Enum.GetValues(typeof(FigureState));
                RandomizeNextFigure();
            }
            else if (enterState is GameState.Falling)
            {
                SetBuildCallbacks();
                tickTimeout = 0f;
                gridWorld.LaunchFigureAtHeight(nextFigureType, nextFigureState, currentHeight + figureSpawnHeight);
            }
            else if (enterState is GameState.Consequence)
            {
                restTimeout = 0f;
                gridWorld.MergeFigure();
                gridWorld.ClearFullLines();
                RandomizeNextFigure();
            }
            else if (enterState is GameState.Fail or GameState.Success)
            {
                RemovePawnCallbacks();
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
            if (restTimeout >= restBetweenFigures)
            {
                ChangeState(GameState.Falling);
            }
            else
            {
                restTimeout += deltaTime;
            }
        }
        
        private void UpdateFalling(float deltaTime)
        {
            var playerY = pawnPlayer.PlayerTransform.Get().position.y;
            var fallDistance = GetCurrentHeight() - playerY;
            if (fallDistance > maxFallDistance)
            {
                ChangeState(GameState.Fail);
                return;
            }
            
            if (tickTimeout >= GetGameTick())
            {
                var down = -1;
                if (gridWorld.IsPossibleMoveFigureY(down))
                {
                    gridWorld.MoveFigureY(down);
                    tickTimeout = 0f;

                    if (gridWorld.IsLowerThen(GetCurrentHeight() - maxFallDistance))
                    {
                        gridWorld.SkipFigure();
                        ChangeState(GameState.Consequence);
                    }
                }
                else
                {
                    ChangeState(GameState.Consequence);
                }
            }
            else
            {
                tickTimeout += deltaTime;
            }
        }
        
        private void RandomizeNextFigure()
        {
            nextFigureType = figureTypes[Random.Range(0, figureTypes.Length)];
            nextFigureState = figureStates[Random.Range(0, figureStates.Length)];
        }
        
        private Vector2 GetSpawnPoint()
        {
            return spawnPoint == null ? transform.position : spawnPoint.position;
        }

        private void SetPawnCallbacks()
        {
            pawnPlayer.GetPawn().Landed += OnPlayerPawnLanded;
        }

        private void RemovePawnCallbacks()
        {
            pawnPlayer.GetPawn().Landed -= OnPlayerPawnLanded;
        }
        
        private void SetBuildCallbacks()
        {
            leftAction.action.started += OnLeft;
            rightAction.action.started += OnRight;
            rotateAction.action.started += OnRotate;
            downAction.action.started += OnDown;
            downAction.action.canceled += OnDown;
        }
        
        private void RemoveBuildCallbacks()
        {
            isFastFalling = false;
            
            leftAction.action.started -= OnLeft;
            rightAction.action.started -= OnRight;
            rotateAction.action.started -= OnRotate;
            downAction.action.started -= OnDown;
            downAction.action.canceled -= OnDown;
        }

        private void OnLeft(InputAction.CallbackContext context)
        {
            if (GetGameState() != GameState.Falling) return;
            
            var left = -1;
            
            if (gridWorld.IsPossibleMoveFigureX(left))
            {
                gridWorld.MoveFigureX(left);
            }
        }

        private void OnRight(InputAction.CallbackContext context)
        {
            if (GetGameState() != GameState.Falling) return;
            
            var right = 1;
            
            if (gridWorld.IsPossibleMoveFigureX(right))
            {
                gridWorld.MoveFigureX(right);
            }
        }

        private void OnRotate(InputAction.CallbackContext context)
        {
            if (GetGameState() != GameState.Falling) return;
            
            if (gridWorld.IsPossibleRotateFigure())
            {
                gridWorld.RotateFigure();
            }
        }
        
        private void OnDown(InputAction.CallbackContext context)
        {
            if (GetGameState() != GameState.Falling) return;
            
            if (context.started)
            {
                isFastFalling = true;
                
                if (GetGameState() == GameState.Falling)
                {
                    var down = -1;
                    if (gridWorld.IsPossibleMoveFigureY(down))
                    {
                        gridWorld.MoveFigureY(down);
                        tickTimeout = 0f;
                    }
                }
            }
            else if (context.canceled)
            {
                isFastFalling = false;
                tickTimeout = 0f;
            }
        }
        
        private void OnPlayerPawnLanded()
        {
            var playerCenter = pawnPlayer.GetPawn().GetWorldCenter();
            var playerCell = gridWorld.GetWorldGrid().WorldToCell(playerCenter);

            if (playerCell.y > currentHeight)
            {
                currentHeight = playerCell.y;
            }
        }
        
        private void Update()
        {
            UpdateGame(GetGameState(), Time.deltaTime);
        }

        private void OnDisable()
        {
            RemovePawnCallbacks();
            RemoveBuildCallbacks();
        }

        private void OnDrawGizmos()
        {
            if (isPlaying)
            {
                var playerCenter = pawnPlayer.GetPawn().GetWorldCenter();
                var playerCell = gridWorld.GetWorldGrid().WorldToCell(playerCenter);
                var playerBlockCenter = gridWorld.GetWorldGrid().GetCellCenterWorld(playerCell);
                
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(playerBlockCenter, gridWorld.GetWorldGrid().cellSize);

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(Vector3.up * currentHeight + Vector3.up * 0.5f, new Vector3(10, 1));
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
