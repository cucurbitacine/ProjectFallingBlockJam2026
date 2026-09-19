using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    [DisallowMultipleComponent]
    public class FigureController : MonoBehaviour
    {
        [SerializeField] private Vector2Int localPosition;
        [SerializeField] private Vector2Int figureSize;
        [SerializeField] private Grid grid;
        
        [SerializeField] private Dictionary<Vector2Int, BlockController> blocks = new Dictionary<Vector2Int, BlockController>();
        
        public Vector2Int GetLocalPosition()
        {
            return localPosition;
        }

        public bool TryGetBlock(Vector2Int localPoint, out BlockController block)
        {
            return blocks.TryGetValue(localPoint, out block);
        }
    }

    public static class FigureExtension
    {
        public static Vector2Int EvaluatePoint(this FigureController figure, Vector2Int localPoint)
        {
            return localPoint + figure.GetLocalPosition();
        }
        
        public static Vector2Int EvaluatePoint(this FigureController figure, BlockController block)
        {
            return block.GetLocalPosition() + figure.GetLocalPosition();
        }
    }
}