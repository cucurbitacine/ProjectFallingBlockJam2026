using Game.Scripts.PlayerSystem.PawnSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.CombatSystem
{
    [RequireComponent(typeof(Collider2D))]
    public class PawnTriggerZone : MonoBehaviour
    {
        [SerializeField] private bool triggerOnce;
        [Space]
        [SerializeField] private UnityEvent<PawnController> pawnEnter = new UnityEvent<PawnController>();
        
        private Collider2D shape;
        private bool triggered;

        private void Trigger(PawnController pawn)
        {
            pawnEnter.Invoke(pawn);
        }

        private void TryTrigger(PawnController pawn)
        {
            if (triggerOnce)
            {
                if (triggered) return;
                triggered = true;
            }

            Trigger(pawn);
        }
        
        private void Awake()
        {
            shape = GetComponent<Collider2D>();
            shape.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PawnController pawn))
            {
                TryTrigger(pawn);
            }
        }
    }
}