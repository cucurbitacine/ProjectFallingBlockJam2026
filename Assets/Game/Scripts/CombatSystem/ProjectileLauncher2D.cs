using UnityEngine;

namespace Game.Scripts.CombatSystem
{
    public class ProjectileLauncher2D : MonoBehaviour
    {
        [SerializeField] private Projectile2D projectilePrefab;

        [SerializeField] private bool localDirection;
        [SerializeField] private Vector2 direction = Vector2.up;

        public Vector2 GetLaunchDirection()
        {
            if (Mathf.Approximately(direction.sqrMagnitude, 0f))
            {
                direction = Vector2.up;
            }

            return localDirection ? transform.TransformDirection(direction) : direction;
        }
        
        public Vector2 GetLaunchPosition() => transform.position;
        public Quaternion GetLaunchRotation() => Quaternion.LookRotation(Vector3.forward, GetLaunchDirection());
        
        public Projectile2D Launch()
        {
            var projectile = Instantiate(projectilePrefab, GetLaunchPosition(), GetLaunchRotation());

            return projectile;
        } 
        
        public void LaunchProjectile()
        {
            Launch();
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