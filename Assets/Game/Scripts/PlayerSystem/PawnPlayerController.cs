using System.Collections;
using Game.Scripts.Core.PlayerSystem;
using Game.Scripts.PlayerSystem.BuilderSystem;
using Game.Scripts.PlayerSystem.PawnSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.PlayerSystem
{
    public class PawnPlayerController : PlayerController
    {
        [Header("Pawn")]
        [SerializeField] private PawnController pawn;
        [SerializeField] private PlayerBuilderController builder;

        [Header("Input Pawn")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;
        
        public override IEnumerator EnablePlayer(CameraController camera)
        {
            yield return base.EnablePlayer(camera);

            moveAction.action.performed += OnMove;
            moveAction.action.canceled += OnMove;
            jumpAction.action.performed += OnJump;
            
            yield return builder.Activate();
        }

        public override void DisablePlayer()
        {
            moveAction.action.performed -= OnMove;
            moveAction.action.canceled -= OnMove;
            jumpAction.action.performed -= OnJump;
            
            builder.Deactivate();
            
            base.DisablePlayer();
        }

        public PawnController GetPawn()
        {
            return pawn;
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
