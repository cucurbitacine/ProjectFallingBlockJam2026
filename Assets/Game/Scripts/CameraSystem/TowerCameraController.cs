using System.Collections;
using Game.Scripts.Core.LevelSystem;
using Game.Scripts.Core.PlayerSystem;
using Game.Scripts.LevelSystem;
using Game.Scripts.WorldSystems;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Scripts.CameraSystem
{
    public class TowerCameraController : CameraController
    {
        [SerializeField] private NoiseSettings mergeNoise;
        [Min(0f)]
        [SerializeField] private float mergeNoiseDuration = 0.5f;
        
        [Space]
        [SerializeField] private NoiseSettings destroyNoise;
        [Min(0f)]
        [SerializeField] private float destroyNoiseDuration = 0.5f;
        
        [SerializeField] private AnimationCurve noiseAmplitude = AnimationCurve.Constant(0f, 1f, 1f);
        
        [Space]
        [SerializeField] private Transform followAnchor;
        [SerializeField] private CinemachineCamera vCam;

        private Coroutine cameraShaking;
        
        private bool isPlaying;
        private GameLevelController gameLevel;
        private GridWorldController gridWorld;
        
        public override IEnumerator EnableCamera(LevelController level)
        {
            yield return base.EnableCamera(level);

            if (level && level is GameLevelController tLevel)
            {
                gameLevel = tLevel;
                gameLevel.GameStateChanged += OnGameStateChanged;
                
                UpdateFollowPosition();
            }

            if (level.GetWorld() is GridWorldController tWorld)
            {
                gridWorld = tWorld;
                gridWorld.FigureMerged += OnFigureMerged;
                gridWorld.LineDestroyed += OnLineDestroyed;
            }
            
            isPlaying = gameLevel && gridWorld;
        }

        public override void DisableCamera()
        {
            isPlaying = false;
            
            gameLevel.GameStateChanged -= OnGameStateChanged;
            gridWorld.FigureMerged -= OnFigureMerged;
            gridWorld.LineDestroyed -= OnLineDestroyed;
            
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
                var y = gameLevel.GetBestCell().y;
                followAnchor.transform.position = Vector3.up * y;
            }
        }
        
        private void OnGameStateChanged(GameState exit, GameState enter)
        {
        }
        
        private void OnFigureMerged(FigureController figure, GridWorldController world)
        {
            StartShake(mergeNoise, mergeNoiseDuration);
        }
        
        private void OnLineDestroyed(int height, GridWorldController world)
        {
            StartShake(destroyNoise, destroyNoiseDuration);
        }

        private void StartShake(NoiseSettings noise, float duration)
        {
            if (cameraShaking != null) StopCoroutine(cameraShaking);
            cameraShaking = StartCoroutine(CameraShaking(noise, duration));
        }

        private IEnumerator CameraShaking(NoiseSettings noise, float duration)
        {
            var perlin = vCam.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;

            if (!perlin) yield break;
            
            perlin.NoiseProfile = noise;
            perlin.enabled = true;

            var time = 0f;
            while (time < duration)
            {
                var t = time / duration;
                perlin.AmplitudeGain = noiseAmplitude.Evaluate(t);
                
                time += Time.deltaTime;
                yield return null;
            }
            
            perlin.enabled = false;
            perlin.NoiseProfile = null;
        }

        private void LateUpdate()
        {
            if (!isPlaying) return;

            UpdateFollowPosition();
        }
    }
}