using UnityEngine;

namespace Game.Scripts
{
    [CreateAssetMenu(menuName = "Create FigureAsset", fileName = "FigureAsset", order = 0)]
    public class FigureAsset : ScriptableObject
    {
        [SerializeField] private Sprite figureIcon;
        [SerializeField] private FigureController figurePrefab;

        public Sprite GetIcon()
        {
            return figureIcon;
        }
        
        public FigureController GetPrefab()
        {
            return figurePrefab;
        }
    }
}