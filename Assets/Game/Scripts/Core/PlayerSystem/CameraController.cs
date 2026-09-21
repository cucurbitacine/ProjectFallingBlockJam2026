using System.Collections;
using Game.Scripts.Core.LevelSystem;
using Game.Scripts.Core.WorldSystem;
using UnityEngine;

namespace Game.Scripts.Core.PlayerSystem
{
    public class CameraController : MonoBehaviour
    {
        public Camera CameraMain => Camera.main;

        public virtual IEnumerator EnableCamera(PlayerController player, WorldController world = null, LevelController level = null)
        {
            yield break;
        }

        public virtual void DisableCamera()
        {
        }
    }
}