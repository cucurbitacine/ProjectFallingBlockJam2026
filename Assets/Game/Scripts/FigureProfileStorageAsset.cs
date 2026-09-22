using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    [CreateAssetMenu(menuName = "Create FigureProfileStorageAsset", fileName = "FigureProfileStorageAsset", order = 0)]
    public class FigureProfileStorageAsset : ScriptableObject
    {
        [SerializeField]
        private Dictionary<FigureType, FigureProfileAsset> figures =
            new Dictionary<FigureType, FigureProfileAsset>();

        public FigureProfileAsset GetFigureProfile(FigureType figureType)
        {
            return figures.GetValueOrDefault(figureType, null);
        }
    }

    public enum FigureType
    {
        Figure_I,
        Figure_O,
        Figure_J,
        Figure_L,
        Figure_S,
        Figure_Z,
        Figure_T,
    }
    
    public enum FigureRotation
    {
        Identity = 0,
        Rotated_90 = 1,
        Rotated_180 = 2,
        Rotated_270 = 3, 
    }
}