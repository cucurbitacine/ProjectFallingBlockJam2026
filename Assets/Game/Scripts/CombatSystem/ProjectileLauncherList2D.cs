using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.CombatSystem
{
    public class ProjectileLauncherList2D : MonoBehaviour
    {
        [SerializeField] private List<ProjectileLauncher2D> launchers = new List<ProjectileLauncher2D>();

        public int LauncherCount => launchers.Count;

        public Projectile2D Launch(int index)
        {
            if (index < 0 || LauncherCount <= index) return null;

            return launchers[index].Launch();
        }
        
        public Projectile2D LaunchRandom()
        {
            return Launch(Random.Range(0, LauncherCount));
        }

        public void LaunchRandomProjectile(int index)
        {
            Launch(index);
        }
        
        public void LaunchRandomProjectile()
        {
            LaunchRandom();
        }
    }
}