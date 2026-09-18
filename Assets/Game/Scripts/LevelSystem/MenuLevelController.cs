using Game.Scripts.Core.LevelSystem;
using UnityEngine;

namespace Game.Scripts.LevelSystem
{
    public class MenuLevelController : LevelController
    {
        [Header("Menu Level")]
        [SerializeField] private LevelSceneAsset playLevelScene;
        
        [ContextMenu(nameof(StartPlayLevel))]
        public void StartPlayLevel()
        {
            LevelManager.Instance.LoadSceneAsync(playLevelScene);
        }
    }
}
