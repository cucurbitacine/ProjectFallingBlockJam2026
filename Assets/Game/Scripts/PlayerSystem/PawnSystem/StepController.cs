using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.PlayerSystem.PawnSystem
{
    public class StepController : MonoBehaviour
    {
        [SerializeField] private PawnController pawn;
        [Space]
        [SerializeField] private float stepDelay = 0.5f;
        [Space]
        [SerializeField] private UnityEvent onStep = new UnityEvent();

        private float stepTimer;
        private bool wasMoving;
        private bool started;
        private bool canceled;
        
        private void UpdateStep(bool isMoving, float deltaTime)
        {
            started = !wasMoving && isMoving;
            canceled = wasMoving && !isMoving;

            wasMoving = isMoving;

            if (started)
            {
                stepTimer = 0f;
                onStep?.Invoke();
            }
            else if (isMoving)
            {
                if (stepTimer >= stepDelay)
                {
                    stepTimer = 0f;
                    onStep?.Invoke();
                }
                else
                {
                    stepTimer += deltaTime;
                }
            }
            else if (canceled)
            {
                stepTimer = 0f;
            }
        }
        
        private void Update()
        {
            if (pawn)
            {
                UpdateStep(pawn.IsMoving && pawn.Grounded, Time.deltaTime);
            }
        }
    }
}
