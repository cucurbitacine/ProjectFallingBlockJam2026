using UnityEngine;

namespace Game.Scripts.CombatSystem
{
    public class ProjectileLauncher2D : BaseProjectile2D
    {
        [SerializeField] private Projectile2D projectilePrefab;

        [Space]
        [SerializeField] private Vector2 offset = Vector2.zero;
        [SerializeField] private Vector2 direction = Vector2.up;
        [SerializeField] private bool localDirection;
        
        public Vector2 GetLaunchDirection()
        {
            if (Mathf.Approximately(direction.sqrMagnitude, 0f))
            {
                direction = Vector2.up;
            }

            return localDirection ? transform.TransformDirection(direction) : direction;
        }
        
        public Vector2 GetLaunchPosition()
        {
            return localDirection ? transform.TransformPoint(offset) : (Vector2)transform.position + offset;
        }

        public Quaternion GetLaunchRotation() => Quaternion.LookRotation(Vector3.forward, GetLaunchDirection());
        
        public override Projectile2D Launch()
        {
            var projectile = Instantiate(projectilePrefab, GetLaunchPosition(), GetLaunchRotation());

            return projectile;
        } 
        
        private void OnValidate()
        {
            if (Mathf.Approximately(direction.sqrMagnitude, 0f))
            {
                direction = Vector2.up;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.softRed;
            Gizmos.DrawWireCube(GetLaunchPosition(), Vector2.one * 0.1f);
            Gizmos.DrawRay(GetLaunchPosition(), GetLaunchDirection().normalized);
        }
    }
}