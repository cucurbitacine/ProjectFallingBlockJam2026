using Game.Scripts.CombatSystem;
using Game.Scripts.Core.PlayerSystem;
using Game.Scripts.PlayerSystem.PawnSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.PlayerSystem
{
    public class PawnPlayerController : PlayerController
    {
        [Header("Pawn")]
        [SerializeField] private PawnController pawn;
        [SerializeField] private Health health;
        [SerializeField] private Collision2DEvent collision;
        [SerializeField] private Trigger2DEvent trigger;

        [Header("Input Pawn")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;

        public void SetCallbacks()
        {
            moveAction.action.performed += OnMove;
            moveAction.action.canceled += OnMove;
            jumpAction.action.performed += OnJump;
        }
        
        public void RemoveCallbacks()
        {
            moveAction.action.performed -= OnMove;
            moveAction.action.canceled -= OnMove;
            jumpAction.action.performed -= OnJump;
            
            pawn.Move(Vector2.zero);
        }
        
        public PawnController GetPawn()
        {
            return pawn;
        }
        
        public Health GetHealth()
        {
            return health;
        }
        
        public Collision2DEvent GetCollisionEvent()
        {
            return collision;
        }
        
        public Trigger2DEvent GetTriggerEvent()
        {
            return trigger;
        }
        
        private void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                pawn.Move(context.ReadValue<Vector2>());
            }
            else if (context.canceled)
            {
                pawn.Move(Vector2.zero);
            }
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (!pawn.Grounded) return;
            
            if (context.performed)
            {
                pawn.Jump();
            }
        }
    }
}
