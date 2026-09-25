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
        [Header("Game State")]
        [SerializeField] private GameState gameState = GameState.Unknown;
        [Min(0)]
        [SerializeField] private Vector3Int bestCell = Vector3Int.zero;
        
        [Header("Game Settings")]
        [Min(0f)]
        [SerializeField] private float regularGameTick = 1.0f;
        
        [Header("Win Settings")]
        [Min(1)]
        [SerializeField] private int scoreToWin = 40;
        [SerializeField] private float winImpulse = 10;
        [SerializeField] private float winTorqueImpulse = 10;
        
        [Header("Fail Settings")]
        [Min(0)]
        [SerializeField] private int maxSafeFallHeight = 2;
        [Min(0f)]
        [SerializeField] private float maxSafeFigureDisposition = 0.1f;
        [SerializeField] private float smashedImpulse = 10;
        
        [Header("Time Settings")]
        [Min(0f)]
        [SerializeField] private float maxTimeout = 15f;
        [SerializeField] private bool updateTimeoutDuringConsequence;
        
        [Header("Figures Settings")]
        [Min(0f)]
        [SerializeField] private float fallingGameTick = 0.1f;
        [SerializeField] private FigureType nextFigureType;
        [SerializeField] private FigureRotation nextFigureRotation;
        [Min(0)]
        [SerializeField] private int figureSpawnHeight = 10;
        [Min(0)]
        [SerializeField] private int minFallHeightForSkip = 3;
        [Min(0f)]
        [SerializeField] private float restBetweenFigures = 1f;
        [SerializeField] private LayerMask blocksLayer;
        
        [Header("Events Settings")]
        [SerializeField] private Transform eventsContainer;
        
        [Header("Build Input")]
        [SerializeField] private InputActionReference leftAction;
        [SerializeField] private InputActionReference rightAction;
        [SerializeField] private InputActionReference rotateAction;
        [SerializeField] private InputActionReference downAction;

        [Header("Required")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private LevelSceneAsset menuLevelScene;
        
        private bool isPlaying;
        private float tickTimeout = 0f;
        private float restTimeout = 0f;
        private float timeLeft = 0f;
        private bool isFastFalling = false;
        
        private PawnPlayerController pawnPlayer;
        private GridWorldController gridWorld;
        
        private FigureType[] figureTypes;
        private FigureRotation[] figureStates;

        #region Public API

        public event Action<GameState, GameState> GameStateChanged;
        public event Action<FailReason> GameFailed;
        public event Action<int, int> ScoreChanged;
        
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
            return isFastFalling ? fallingGameTick : regularGameTick;
        }
        
        public Vector3Int GetBestCell()
        {
            return bestCell;
        }

        public float GetTimeLeft()
        {
            return timeLeft;
        }

        public float GetTimeMax()
        {
            return maxTimeout;
        }

        public int GetScore()
        {
            return GetBestCell().y;
        }

        public void HealPlayer(int amount)
        {
            pawnPlayer.GetHealth().Heal(amount);
        }
        
        public void AddTime(float time)
        {
            timeLeft = Mathf.Min(timeLeft + time, maxTimeout);
        }
        
        #endregion

        #region Override API

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
            
            ChangeState(GameState.FigureFalling);
        }

        #endregion

        #region Game Flow API

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
            //Debug.Log($"{exitState} -> {enterState}");
            
            if (exitState is GameState.Initializing)
            {
                SetPawnCallbacks();

                ResetTime();
            }
            else if (exitState is GameState.FigureFalling)
            {
                RemoveBuildCallbacks();
            }

            if (enterState is GameState.Initializing)
            {
                EnterInitializing();
            }
            else if (enterState is GameState.FigureFalling)
            {
                EnterFalling();
            }
            else if (enterState is GameState.Consequence)
            {
                EnterConsequence();
            }
            else if (enterState is GameState.Fail)
            {
                EnterFail();
            }
            else if (enterState is GameState.Win)
            {
                EnterWin();
            }
        }

        private void EnterInitializing()
        {
            figureTypes = (FigureType[])Enum.GetValues(typeof(FigureType));
            figureStates = (FigureRotation[])Enum.GetValues(typeof(FigureRotation));
            RandomizeNextFigure();
        }

        private void EnterFalling()
        {
            tickTimeout = 0f;
            GetNextFigure(out var figureType, out var figureState);

            if (gridWorld.LaunchFigureAtHeight(figureType, figureState, GetBestCell().y + GetFigureSpawnHeight()))
            {
                SetBuildCallbacks();
            }
            else
            {
                Fail(FailReason.ReachedTowerLimit);
            }
        }
        
        private void EnterConsequence()
        {
            restTimeout = 0f;
            gridWorld.MergeFigure();

            if (gridWorld.DestroyFullLines() > 0)
            {
                AddTime(maxTimeout);
            }
            
            RandomizeNextFigure();
        }
        
        private void EnterFail()
        {
            RemovePawnCallbacks();
            
            if (pawnPlayer.GetPawn().TryGetComponent(out Collider2D shape))
            {
                //shape.enabled = false;
            }
            
            if (pawnPlayer.GetPawn().TryGetComponent(out Rigidbody2D body))
            {
                body.linearVelocity = Vector2.zero;
                
                body.excludeLayers = blocksLayer;
                
                body.AddForce(smashedImpulse * Vector2.up, ForceMode2D.Impulse);
            }
        }
        
        private void EnterWin()
        {
            RemovePawnCallbacks();
            
            if (pawnPlayer.GetPawn().TryGetComponent(out Collider2D shape))
            {
                shape.enabled = false;
            }
            
            if (pawnPlayer.GetPawn().TryGetComponent(out Rigidbody2D body))
            {
                body.linearVelocity = Vector2.zero;
                body.gravityScale = 0f;
                body.constraints = RigidbodyConstraints2D.None;
                body.AddForce(winImpulse * Vector2.up, ForceMode2D.Impulse);
                body.AddTorque(winTorqueImpulse, ForceMode2D.Impulse);
            }
        }
        
        private void UpdateGame(GameState state, float deltaTime)
        {
            if (state == GameState.FigureFalling)
            {
                UpdateFigureFalling(deltaTime);
            }
            else if (state == GameState.Consequence)
            {
                UpdateConsequence(deltaTime);
            }

            eventsContainer.position = Vector2.up * GetBestCell().y;
        }

        private bool IsPlayerFallDown()
        {
            var fallDistance = GetBestCell().y - GetPlayerCell().y;
            return fallDistance > maxSafeFallHeight;
        }

        private bool IsPlayerFailBecauseFell()
        {
            if (IsPlayerFallDown())
            {
                Fail(FailReason.FellToLow);
                return true;
            }

            return false;
        }
        
        private void UpdateFigureFalling(float deltaTime)
        {
            if (IsPlayerFailBecauseFell())
            {
                return;
            }
            
            if (tickTimeout >= GetGameTick())
            {
                var down = -1;
                if (gridWorld.IsPossibleMoveFigureY(down))
                {
                    gridWorld.MoveFigureY(down);
                    tickTimeout = 0f;

                    if (gridWorld.IsLowerThen(GetBestCell().y - minFallHeightForSkip))
                    {
                        gridWorld.SkipFigure();
                        ChangeState(GameState.Consequence);
                        return;
                    }
                }
                else
                {
                    ChangeState(GameState.Consequence);
                    return;
                }
            }
            else
            {
                tickTimeout += deltaTime;
            }

            UpdateTimeout(deltaTime);
        }
        
        private void UpdateConsequence(float deltaTime)
        {
            if (IsPlayerFailBecauseFell())
            {
                return;
            }
            
            if (restTimeout >= restBetweenFigures)
            {
                ChangeState(GameState.FigureFalling);
                return;
            }
            else
            {
                restTimeout += deltaTime;
            }

            if (updateTimeoutDuringConsequence)
            {
                UpdateTimeout(deltaTime);   
            }
        }
        
        private void Fail(FailReason reason)
        {
            Debug.LogWarning($"Fail: {reason}");
            
            ChangeState(GameState.Fail);
            
            GameFailed?.Invoke(reason);
        }
        
        #endregion

        #region Callbacks API

        private void SetPawnCallbacks()
        {
            pawnPlayer.GetPawn().Landed += OnPlayerPawnLanded;
            pawnPlayer.GetHealth().ValueChanged += OnPlayerHealthChanged;
            pawnPlayer.GetCollisionEvent().EnterEvent += OnPlayerCollisionEnter;
            pawnPlayer.GetTriggerEvent().EnterEvent += OnPlayerTriggerEnter;
            pawnPlayer.SetCallbacks();
        }

        private void RemovePawnCallbacks()
        {
            pawnPlayer.GetPawn().Landed -= OnPlayerPawnLanded;
            pawnPlayer.GetHealth().ValueChanged -= OnPlayerHealthChanged;
            pawnPlayer.GetCollisionEvent().EnterEvent -= OnPlayerCollisionEnter;
            pawnPlayer.GetTriggerEvent().EnterEvent -= OnPlayerTriggerEnter;
            pawnPlayer.RemoveCallbacks();
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
            if (GetGameState() != GameState.FigureFalling) return;
            
            var left = -1;
            
            if (gridWorld.IsPossibleMoveFigureX(left))
            {
                gridWorld.MoveFigureX(left);
            }
        }

        private void OnRight(InputAction.CallbackContext context)
        {
            if (GetGameState() != GameState.FigureFalling) return;
            
            var right = 1;
            
            if (gridWorld.IsPossibleMoveFigureX(right))
            {
                gridWorld.MoveFigureX(right);
            }
        }

        private void OnRotate(InputAction.CallbackContext context)
        {
            if (GetGameState() != GameState.FigureFalling) return;
            
            if (gridWorld.IsPossibleRotateFigure())
            {
                gridWorld.RotateFigure();
            }
        }
        
        private void OnDown(InputAction.CallbackContext context)
        {
            if (GetGameState() != GameState.FigureFalling) return;
            
            if (context.started)
            {
                isFastFalling = true;
                
                if (GetGameState() == GameState.FigureFalling)
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
            var playerCell = GetPlayerCell();

            if (!SetBestCell(playerCell)) return;
            
            if (GetBestCell().y >= scoreToWin)
            {
                ChangeState(GameState.Win);
                return;
            }
        }

        private void OnPlayerHealthChanged(int prev, int curr)
        {
            if (curr < prev && curr == 0)
            {
                Fail(FailReason.LostHealth);
            }
        }
        
        private void OnPlayerTriggerEnter(Collider2D other)
        {
            // TODO hit by projectiles
        }

        private void OnPlayerCollisionEnter(Collision2D other)
        {
            // Only during Falling 
            if (GetGameState() != GameState.FigureFalling) return;
            
            // Contact only with Block
            if (!other.collider.TryGetComponent(out BlockController block)) return;

            // Block is part of Falling figure
            var fallingFigure = gridWorld.GetFallingFigure();
            if (fallingFigure == null) return;
            if (!fallingFigure.Contains(block)) return;

            var firstContact = other.GetContact(0);
            // Contact with Bottom part of block
            if (firstContact.point.y >= block.GetWorldCenter().y) return;
            // Contact normal looks Up or Down
            if (!Mathf.Approximately(Vector2.Dot(firstContact.normal, Vector2.right), 0f)) return;

            // Block is moving
            var shouldBeAt = gridWorld.GetGrid().GetCellCenterWorld(block.GetCell());
            var actuallyAt = block.GetWorldCenter();
            var disposition = Vector2.Distance(shouldBeAt, actuallyAt);
            if (disposition <= maxSafeFigureDisposition) return;
            
            Fail(FailReason.SmashedByFallingFigure);
        }
        
        #endregion
        
        #region Private API
        
        private void ResetTime()
        {
            timeLeft = maxTimeout;
        }
        
        private void UpdateTimeout(float deltaTime)
        {
            if (timeLeft <= 0f)
            {
                Fail(FailReason.DidntGetHigher);
                return;
            }
            
            timeLeft -= deltaTime;
        }
        
        private bool SetBestCell(Vector3Int cell)
        {
            if (cell.y <= bestCell.y) return false;
            
            var previousCell = bestCell;
            bestCell = cell;

            OnBestCellChanged(previousCell, bestCell);
            
            return true;
        }

        private void OnBestCellChanged(Vector3Int previous, Vector3Int current)
        {
            if (current.y > previous.y)
            {
                ScoreChanged?.Invoke(previous.y, current.y);
            }
        }

        private Vector3Int GetPlayerCell()
        {
            var playerWorld = pawnPlayer.GetPawn().GetWorldCenter();
            var playerCell = gridWorld.GetGrid().WorldToCell(playerWorld);
            return playerCell;
        }
        
        private int GetFigureSpawnHeight()
        {
            return figureSpawnHeight;
        }
        
        private void GetNextFigure(out FigureType figureType, out FigureRotation figureRotation)
        {
            figureType = nextFigureType;
            figureRotation = nextFigureRotation;
        }
        
        private void RandomizeNextFigure()
        {
            nextFigureType = figureTypes[Random.Range(0, figureTypes.Length)];
            nextFigureRotation = figureStates[Random.Range(0, figureStates.Length)];
        }
        
        private Vector2 GetSpawnPoint()
        {
            return spawnPoint == null ? transform.position : spawnPoint.position;
        }

        #endregion

        #region MonoBehaviour API

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
                var playerCell = gridWorld.GetGrid().WorldToCell(playerCenter);
                var playerBlockCenter = gridWorld.GetGrid().GetCellCenterWorld(playerCell);
                
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(playerBlockCenter, gridWorld.GetGrid().cellSize);

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(Vector3.up * GetBestCell().y + Vector3.up * 0.5f, new Vector3(10, 1));
            }
        }

        #endregion
    }

    public enum GameState
    {
        Unknown,
        Initializing,
        FigureFalling,
        Consequence,
        Fail,
        Win,
    }

    public enum FailReason
    {
        SmashedByFallingFigure,
        FellToLow,
        ReachedTowerLimit,
        LostHealth,
        DidntGetHigher,
    }
}
