using System;
using UnityEngine;

namespace Game.Scripts.PlayerSystem.PawnSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PawnController : MonoBehaviour
    {
        [SerializeField] private bool grounded;
        [SerializeField] private Vector2 worldCenter = Vector2.up * 0.5f;

        [Header("Ground")]
        [Min(0f)]
        [SerializeField] private float groundCheckRadius = 0.1f;
        [SerializeField] private ContactFilter2D groundCheckFilter;
        
        [Header("Move & Jump")]
        [Min(0f)]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpHeight = 1.2f;
        
        private Vector2 moveInput;
        private Rigidbody2D body;
        private ColliderArray2D groundCheck;

        private Vector2 groundCheckPosition => transform.position;
        
        public Vector2 MoveInput => moveInput;
        public bool IsMoving => !Mathf.Approximately(MoveInput.x, 0f);
        public bool Grounded => grounded;

        public event Action Landed;
        
        public Vector3 GetWorldCenter()
        {
            return transform.TransformPoint(worldCenter);
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
            groundCheck = Physics2D.OverlapCircle(groundCheckPosition, groundCheckRadius, groundCheckFilter);

            var wasGrounded = grounded;
            grounded = groundCheck.Length > 0;

            if (!wasGrounded && grounded)
            {
                Landed?.Invoke();
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
            Gizmos.color = Color.yellowNice;
            Gizmos.DrawWireSphere(GetWorldCenter(), 0.1f);
            
            Gizmos.color = grounded ? Color.limeGreen : Color.softRed;
            Gizmos.DrawWireSphere(groundCheckPosition, groundCheckRadius);
        }
    }
}