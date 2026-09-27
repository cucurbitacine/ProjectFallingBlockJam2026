using System;
using Game.Scripts.LevelSystem;
using UnityEngine;

namespace Game.Scripts.SoundSystem
{
    public class TimerVolumeController : MonoBehaviour
    {
        [SerializeField] private GameLevelController gameLevel;
        [SerializeField] private SoundSource soundSource;
        [SerializeField] private AnimationCurve timerVolume = AnimationCurve.Linear(0f, 1f, 1f, 0f);

        private bool isPlaying;
        
        private void StartTimer()
        {
            isPlaying = true;
            soundSource.Play();
        }

        private void StopTimer()
        {
            isPlaying = false;
            soundSource.Stop();
        }

        private void UpdateTimer(float deltaTime)
        {
            if (!isPlaying) return;

            if (gameLevel.GetGameState() == GameState.Consequence)
            {
                soundSource.AudioSource.volume = 0f;
            }
            else
            {
                var timeLeft = gameLevel.GetTimeLeft();
                var timeMax = gameLevel.GetTimeMax();
                var t = timeLeft / timeMax;
                var volume = timerVolume.Evaluate(t);
                soundSource.AudioSource.volume = volume;
            }
        }
        
        private void OnGameStateChanged(GameState exit, GameState enter)
        {
            if (exit is GameState.Initializing)
            {
                StartTimer();
            }

            if (enter is (GameState.Fail or GameState.Win))
            {
                StopTimer();
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

        private void Update()
        {
            UpdateTimer(Time.deltaTime);
        }
    }
}
