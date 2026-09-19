using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.PlayerSystem.PawnSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PawnController : MonoBehaviour
    {
        [SerializeField] private bool grounded;

        [Header("Ground")]
        [Min(0f)]
        [SerializeField] private float groundCheckRadius = 0.1f;
        [SerializeField] private ContactFilter2D groundCheckFilter;
        
        [Header("Move & Jump")]
        [Min(0f)]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpHeight = 1.2f;
        
        [Header("Input")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;

        private Vector2 moveInput;
        private Rigidbody2D body;
        private ColliderArray2D groundCheck;

        private Vector2 groundCheckPosition => transform.position;
        
        public Vector2 MoveInput => moveInput;
        public bool IsMoving => !Mathf.Approximately(MoveInput.x, 0f);
        public bool Grounded => grounded;
        
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
        
        private void GroundCheck()
        {
            var groundCheckPosition = transform.position;
            groundCheck = Physics2D.OverlapCircle(groundCheckPosition, groundCheckRadius, groundCheckFilter);

            grounded = groundCheck.Length > 0;
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
            GroundCheck();
            
            body.linearVelocityX = Mathf.Clamp(moveInput.x, -1f, 1f) * moveSpeed;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = grounded ? Color.limeGreen : Color.softRed;
            Gizmos.DrawWireSphere(groundCheckPosition, groundCheckRadius);
        }
    }
}