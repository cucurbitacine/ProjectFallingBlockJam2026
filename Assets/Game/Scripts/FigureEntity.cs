using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    [DisallowMultipleComponent]
    public class FigureEntity : MonoBehaviour
    {
        [SerializeField] private Vector2Int localPosition;
        [SerializeField] private Grid grid;
        
        [SerializeField] private Dictionary<Vector2Int, BlockEntity> blocks = new Dictionary<Vector2Int, BlockEntity>();
        
        public Vector2Int GetLocalPosition()
        {
            return localPosition;
        }

        public bool TryGetBlock(Vector2Int localPoint, out BlockEntity block)
        {
            return blocks.TryGetValue(localPoint, out block);
        }
    }

    public static class FigureExtension
    {
        public static Vector2Int EvaluatePoint(this FigureEntity figure, Vector2Int localPoint)
        {
            return localPoint + figure.GetLocalPosition();
        }
        
        public static Vector2Int EvaluatePoint(this FigureEntity figure, BlockEntity block)
        {
            return block.GetLocalPosition() + figure.GetLocalPosition();
        }
    }
}