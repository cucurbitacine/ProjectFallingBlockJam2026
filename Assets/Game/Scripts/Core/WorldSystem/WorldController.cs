using System.Collections;
using UnityEngine;

namespace Game.Scripts.Core.WorldSystem
{
    public class WorldController : MonoBehaviour
    {
        public virtual IEnumerator EnableWorld()
        {
            yield break;
        }

        public virtual void DisableWorld()
        {
        }
    }
}