using System;
using UnityEngine;

namespace Game.Scripts.CombatSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class Trigger2DEvent : MonoBehaviour
    {
        public event Action<Collider2D> EnterEvent;

        private void OnTriggerEnter2D(Collider2D other)
        {
            EnterEvent?.Invoke(other);
        }
    }
}