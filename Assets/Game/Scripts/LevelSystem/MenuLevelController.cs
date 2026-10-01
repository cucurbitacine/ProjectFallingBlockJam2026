using System.Collections;
using CucuTools.LevelSystem;
using Game.Scripts.UISystem;
using UnityEngine;

namespace Game.Scripts.LevelSystem
{
    public class MenuLevelController : LevelController
    {
        [Header("Menu Level")]
        [SerializeField] private LevelSceneAsset playLevelScene;
        
        private MenuUIController menuUI;
        
        [ContextMenu(nameof(StartPlayLevel))]
        public void StartPlayLevel()
        {
            LevelManager.Instance.LoadSceneAsync(playLevelScene);
        }

        public override void Init(ContextContainer context)
        {
            base.Init(context);

            menuUI = FindAnyObjectByType<MenuUIController>();
        }

        protected override IEnumerator StartLevel()
        {
            yield return base.StartLevel();
            
            menuUI.EnableUI();
        }

        protected override void DestroyLevel()
        {
            menuUI.DisableUI();
            
            base.DestroyLevel();
        }
    }
}
