using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.CombatSystem
{
    public class ProjectileLauncherList2D : BaseProjectile2D
    {
        [SerializeField] private List<BaseProjectile2D> launchers = new List<BaseProjectile2D>();

        public int LauncherCount => launchers.Count;

        public Projectile2D Launch(int index)
        {
            if (index < 0 || LauncherCount <= index) return null;

            return launchers[index].Launch();
        }
        
        public override Projectile2D Launch()
        {
            return Launch(Random.Range(0, LauncherCount));
        }

        public void LaunchProjectile(int index)
        {
            Launch(index);
        }
    }
}