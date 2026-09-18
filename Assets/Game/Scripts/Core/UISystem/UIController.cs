using System.Collections;
using Game.Scripts.Core.PlayerSystem;
using Game.Scripts.Core.WorldSystem;
using UnityEngine;

namespace Game.Scripts.Core.UISystem
{
    [DisallowMultipleComponent]
    public class UIController : MonoBehaviour
    {
        public virtual IEnumerator EnableUI(PlayerController player, WorldController world)
        {
            yield break;
        }

        public virtual void DisableUI()
        {
        }
    }
}
