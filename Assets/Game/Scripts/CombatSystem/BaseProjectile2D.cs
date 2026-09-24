using UnityEngine;

namespace Game.Scripts.CombatSystem
{
    public abstract class BaseProjectile2D : MonoBehaviour
    {
        public abstract Projectile2D Launch();

        public void LaunchProjectile()
        {
            Launch();
        }
    }
}