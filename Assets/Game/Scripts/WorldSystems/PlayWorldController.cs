using Game.Scripts.Core.WorldSystem;
using UnityEngine;

namespace Game.Scripts.WorldSystems
{
    public class PlayWorldController : WorldController
    {
        [SerializeField] private Transform spawnPoint;
        
        public Vector2 GetSpawnPoint()
        {
            return spawnPoint.position;
        }
    }
}
