using System.Collections;
using Game.Scripts.Core.LevelSystem;
using Game.Scripts.Core.PlayerSystem;
using Game.Scripts.Core.WorldSystem;
using UnityEngine;

namespace Game.Scripts.Core.UISystem
{
    [DisallowMultipleComponent]
    public class UIController : MonoBehaviour
    {
        public virtual IEnumerator EnableUI(LevelController level)
        {
            yield break;
        }

        public virtual void DisableUI()
        {
        }
    }
}
