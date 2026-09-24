using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.LevelSystem
{
    public class GameScoreEvents : MonoBehaviour
    {
        [SerializeField] private UnityEvent scoreChanged = new UnityEvent();
        [SerializeField] private Dictionary<int, UnityEvent> events = new Dictionary<int, UnityEvent>();

        private GameLevelController gameLevel;

        private void OnGameScoreChanged(int previous, int score)
        {
            if (score > previous)
            {
                scoreChanged.Invoke(); 
            }
            
            if (events.TryGetValue(score, out var unityEvent))
            {
                unityEvent.Invoke();
            }
        }
        
        private void Awake()
        {
            gameLevel = GetComponentInParent<GameLevelController>();
        }

        private void Start()
        {
            gameLevel.ScoreChanged += OnGameScoreChanged;
        }
        
        private void OnDestroy()
        {
            gameLevel.ScoreChanged -= OnGameScoreChanged;
        }
    }
}