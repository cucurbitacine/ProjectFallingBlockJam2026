using System.Collections;
using Game.Scripts.Core.LevelSystem;
using UnityEngine;

namespace Game.Scripts.Core.PlayerSystem
{
    public class CameraController : MonoBehaviour
    {
        public Camera CameraMain => Camera.main;

        public virtual IEnumerator EnableCamera(LevelController level)
        {
            yield break;
        }

        public virtual void DisableCamera()
        {
        }
    }
}