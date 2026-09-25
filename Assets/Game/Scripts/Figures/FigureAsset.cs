using UnityEngine;

namespace Game.Scripts.Figures
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