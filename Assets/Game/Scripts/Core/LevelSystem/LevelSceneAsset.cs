using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Core.LevelSystem
{
    [CreateAssetMenu(menuName = "Create LevelSceneAsset", fileName = "LevelSceneAsset", order = 0)]
    public class LevelSceneAsset : ScriptableObject
    {
        [SerializeField] private int sceneBuildIndex;
        
        [Space]
        [SerializeField] private LoadSceneParameters loadSceneParameters;

        public int GetSceneBuildIndex()
        {
            return sceneBuildIndex;
        }

        public LoadSceneParameters GetLoadSceneParameters()
        {
            return loadSceneParameters;
        }
    }
}