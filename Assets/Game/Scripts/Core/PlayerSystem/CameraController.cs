using System.Collections;
using UnityEngine;

namespace Game.Scripts.Core.PlayerSystem
{
    public class CameraController : MonoBehaviour
    {
        public Camera CameraMain => Camera.main;

        public virtual IEnumerator EnableCamera(PlayerController player)
        {
            yield break;
        }

        public virtual void DisableCamera()
        {
        }
    }
}