using System.Collections;
using Game.Scripts.Core.LevelSystem;
using Game.Scripts.PlayerSystem;
using Game.Scripts.WorldSystems;
using UnityEngine;

namespace Game.Scripts.LevelSystem
{
    public class PlayLevelController : LevelController
    {
        [Header("Play Level")]
        [SerializeField] private LevelSceneAsset menuLevelScene;
        
        [ContextMenu(nameof(ReturnMenuLevel))]
        public void ReturnMenuLevel()
        {
            LevelManager.Instance.LoadSceneAsync(menuLevelScene);
        }

        protected override IEnumerator EnableLevel()
        {
            yield return base.EnableLevel();

            var world = this.GetWorld<PlayWorldController>();
            
            GetPlayer().PlayerTransform.Get().position = world.GetSpawnPoint();
        }
    }
}
