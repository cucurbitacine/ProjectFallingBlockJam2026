using System.Collections;
using UnityEngine;

namespace Game.Scripts.Core.SavingSystem
{
    [DisallowMultipleComponent]
    public abstract class SavingController : MonoBehaviour
    {
        public abstract IEnumerator EnableSaving();
        public abstract void DisableSaving();
    }
}
