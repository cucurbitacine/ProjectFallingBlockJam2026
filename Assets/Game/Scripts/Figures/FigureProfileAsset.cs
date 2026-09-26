using UnityEngine;

namespace Game.Scripts.Figures
{
    [CreateAssetMenu(menuName = "Create FigureProfileAsset", fileName = "FigureProfileAsset", order = 0)]
    public class FigureProfileAsset : ScriptableObject
    {
        [SerializeField] private FigureAsset figureDefault;

        public FigureAsset GetFigureAsset()
        {
            return figureDefault;
        }
    }
}