using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    [CreateAssetMenu(menuName = "Create FigureProfileAsset", fileName = "FigureProfileAsset", order = 0)]
    public class FigureProfileAsset : ScriptableObject
    {
        [SerializeField] private FigureAsset figureDefault;
        
        [SerializeField]
        private Dictionary<FigureState, FigureAsset> states =
            new Dictionary<FigureState, FigureAsset>(); 
        
        public FigureAsset GetFigureDefault()
        {
            return figureDefault;
        }
        
        public FigureAsset GetFigure(FigureState state = FigureState.Turn0)
        {
            return states.GetValueOrDefault(state, GetFigureDefault());
        }
    }

    public enum FigureState
    {
        Turn0 = 0,
        Turn90 = 1,
        Turn180 = 2,
        Turn270 = 3, 
    }
}