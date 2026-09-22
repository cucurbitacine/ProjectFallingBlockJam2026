using UnityEngine;

namespace Game.Scripts
{
    [CreateAssetMenu(menuName = "Create FigureProfileAsset", fileName = "FigureProfileAsset", order = 0)]
    public class FigureProfileAsset : ScriptableObject
    {
        [SerializeField] private FigureAsset figureDefault;

        public FigureAsset GetFigureDefault()
        {
            return figureDefault;
        }
    }
}