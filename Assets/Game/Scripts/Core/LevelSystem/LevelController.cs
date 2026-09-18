using System;
using System.Collections;
using Game.Scripts.Core.PlayerSystem;
using Game.Scripts.Core.SavingSystem;
using Game.Scripts.Core.UISystem;
using Game.Scripts.Core.WorldSystem;
using UnityEngine;

namespace Game.Scripts.Core.LevelSystem
{
    [DisallowMultipleComponent]
    public class LevelController : MonoBehaviour
    {
        [Header("Required")]
        [SerializeField] private PlayerController player;
        [SerializeField] private CameraController camera;
        
        [Header("Optional")]
        [SerializeField] private SavingController saving;
        [SerializeField] private WorldController world;
        [SerializeField] private UIController ui;
     
        [Header("Parameters")]
        [SerializeField] private LevelParameters levelParameters;

        #region Public API

        public SavingController GetSaving() => saving;
        public WorldController GetWorld() => world;
        public PlayerController GetPlayer() => player;
        public CameraController GetCamera() => camera;
        public UIController GetUI() => ui;

        public LevelParameters GetParameters()
        {
            return levelParameters;
        }
        
        public void SetParameters(LevelParameters parameters)
        {
            levelParameters = parameters;
        }

        #endregion

        #region Virtual API

        protected virtual IEnumerator EnableLevel()
        {
            FindAll();

            yield return EnableSaving();
            yield return EnableWorld();
            yield return EnablePlayer();
            yield return EnableCamera();
            yield return EnableUI();
        }

        protected virtual void DisableLevel()
        {
            DisableUI();
            DisableCamera();
            DisablePlayer();  
            DisableWorld();
            DisableSaving();
        }

        protected virtual IEnumerator EnableSaving()
        {
            yield return saving?.EnableSaving();
        }
        
        protected virtual IEnumerator EnableWorld()
        {
            yield return world?.EnableWorld();
        }
        
        protected virtual IEnumerator EnablePlayer()
        {
            yield return player?.EnablePlayer(camera);
        }
        
        protected virtual IEnumerator EnableCamera()
        {
            yield return camera?.EnableCamera(player);
        }
        
        protected virtual IEnumerator EnableUI()
        {
            yield return ui?.EnableUI(player, world);
        }
        
        protected virtual void DisableSaving()
        {
            saving?.DisableSaving();
        }
        
        protected virtual void DisableWorld()
        {
            world?.DisableWorld();
        }
        
        protected virtual void DisablePlayer()
        {
            player?.DisablePlayer();
        }
        
        protected virtual void DisableCamera()
        {
            camera?.DisableCamera();
        }
        
        protected virtual void DisableUI()
        {
            ui?.DisableUI();
        }

        #endregion
        
        private void FindAll()
        {
            if (saving == null) saving = Find<SavingController>();
            if (world == null) world = Find<WorldController>();
            if (player == null) player = Find<PlayerController>();
            if (camera == null) camera = Find<CameraController>();
            if (ui == null) ui = Find<UIController>();
        }
        
        private static T Find<T>() where T : MonoBehaviour
        {
            return FindAnyObjectByType<T>();
        }
        
        private IEnumerator Start()
        {
            yield return EnableLevel();
        }

        private void OnDestroy()
        {
            DisableLevel();
        }
    }

    [Serializable]
    public struct LevelParameters
    {
        public int previousSceneId;
    }
    
    public static class LevelControllerExt
    {
        public static T GetSaving<T>(this LevelController level) where T : SavingController => level.GetSaving() is T t ? t : null;
        public static T GetWorld<T>(this LevelController level) where T : WorldController => level.GetWorld() is T t ? t : null;
        public static T GetPlayer<T>(this LevelController level) where T : PlayerController => level.GetPlayer() is T t ? t : null;
        public static T GetCamera<T>(this LevelController level) where T : CameraController => level.GetCamera() is T t ? t : null;
        public static T GetUI<T>(this LevelController level) where T : UIController => level.GetUI() is T t ? t : null;
    }
}
