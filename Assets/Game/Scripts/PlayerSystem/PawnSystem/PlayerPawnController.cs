using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.PlayerSystem.PawnSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerPawnController : MonoBehaviour
    {
        [SerializeField] private bool grounded;
        
        [Header("Settings")]
        [Min(0f)]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpHeight = 1.2f;
        
        [Header("Input")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;

        [SerializeField] private Vector2 moveInput;
        private Rigidbody2D body;
        
        public IEnumerator Activate()
        {
            moveAction.action.performed += OnMove;
            moveAction.action.canceled += OnMove;
            jumpAction.action.performed += OnJump;
                
            yield break;
        }

        public void Deactivate()
        {
            moveAction.action.performed -= OnMove;
            moveAction.action.canceled -= OnMove;
            jumpAction.action.performed -= OnJump;
        }

        public void Move(Vector2 move)
        {
            moveInput = move;
        }
        
        public void Jump()
        {
            var gravity = body.gravityScale * Physics2D.gravity;
            var jumpVelocity = Mathf.Sqrt(-2f * gravity.y * jumpHeight);
            
            body.linearVelocityY = jumpVelocity;
        }
        
        private void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Move(context.ReadValue<Vector2>());
            }
            else if (context.canceled)
            {
                Move(Vector2.zero);
            }
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (!grounded) return;
            
            if (context.performed)
            {
                Jump();
            }
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            body.linearVelocityX = Mathf.Clamp(moveInput.x, -1f, 1f) * moveSpeed;
        }
    }
}