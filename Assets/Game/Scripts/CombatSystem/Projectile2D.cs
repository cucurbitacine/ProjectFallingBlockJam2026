using UnityEngine;

namespace Game.Scripts.CombatSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile2D : MonoBehaviour
    {
        [Min(0f)]
        [SerializeField] private float speedMax = 5f;
        [Min(0)]
        [SerializeField] private int damageAmount = 1;

        [SerializeField] private bool destroyAfterDamage;
        
        private Rigidbody2D body;
        private Collider2D shape;
        
        public Vector2 GetDirection()
        {
            return transform.up;
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            
            shape = GetComponent<Collider2D>();
            shape.isTrigger = true;
        }

        private void FixedUpdate()
        {
            body.linearVelocity = GetDirection() * speedMax;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (damageAmount <= 0) return;
            if (!other.TryGetComponent(out Health health)) return;
            if (health.Damage(damageAmount) <= 0) return;
            if (!destroyAfterDamage) return;
            Destroy(gameObject);
        }
    }
}