using System;
using UnityEngine;

namespace Game.Scripts.CombatSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class Collision2DEvent : MonoBehaviour
    {
        public event Action<Collision2D> EnterEvent;
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            EnterEvent?.Invoke(other);
        }
    }
}