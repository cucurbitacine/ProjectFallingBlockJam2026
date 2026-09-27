using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.LevelSystem
{
    public class GameLevelStateEvent : MonoBehaviour
    {
        [SerializeField] private GameLevelController gameLevel;
        
        [Space]
        [SerializeField] private Dictionary<GameState, UnityEvent> onEnter = new Dictionary<GameState, UnityEvent>();
        [SerializeField] private Dictionary<GameState, UnityEvent> onExit = new Dictionary<GameState, UnityEvent>();

        private void OnGameStateChanged(GameState exit, GameState enter)
        {
            if (onExit.TryGetValue(exit, out var exitEvent))
            {
                exitEvent.Invoke();
            }
            
            if (onEnter.TryGetValue(enter, out var enterEvent))
            {
                enterEvent.Invoke();
            }
        }
        
        private void OnEnable()
        {
            gameLevel.GameStateChanged += OnGameStateChanged;
        }

        private void OnDisable()
        {
            gameLevel.GameStateChanged -= OnGameStateChanged;
        }
    }
}