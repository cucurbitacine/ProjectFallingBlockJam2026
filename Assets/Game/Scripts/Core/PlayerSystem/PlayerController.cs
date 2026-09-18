using System;
using System.Collections;
using UnityEngine;

namespace Game.Scripts.Core.PlayerSystem
{
    [DisallowMultipleComponent]
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField] public PlayerTransform PlayerTransform { get; private set; } = new PlayerTransform();
        public PlayerCamera PlayerCamera { get; private set; } = new PlayerCamera();

        public virtual IEnumerator EnablePlayer(CameraController camera)
        {
            if (PlayerTransform.Get() == null)
            {
                PlayerTransform.Change(transform);
            }
            
            PlayerCamera.Change(camera);
            
            yield break;
        }

        public virtual void DisablePlayer()
        {
        }
    }

    [Serializable]
    public class PlayerTransform
    {
        [SerializeField] private Transform playerTransform;

        public event Action<Transform> TransformChanged;

        public Transform Get()
        {
            return playerTransform;
        }
        
        public void Change(Transform newPlayerTransform)
        {
            if (playerTransform == newPlayerTransform) return;

            playerTransform = newPlayerTransform;

            OnTransformChanged();
        }
        
        private void OnTransformChanged()
        {
            TransformChanged?.Invoke(playerTransform);
        }
    }
    
    [Serializable]
    public class PlayerCamera
    {
        [SerializeField] private CameraController playerCamera;

        public event Action<CameraController> CameraChanged;

        public CameraController Get()
        {
            return playerCamera;
        }
        
        public void Change(CameraController camera)
        {
            if (playerCamera == camera) return;

            playerCamera = camera;

            OnCameraChanged();
        }
        
        private void OnCameraChanged()
        {
            CameraChanged?.Invoke(playerCamera);
        }
    }
}