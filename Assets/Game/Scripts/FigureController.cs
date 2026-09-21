using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    [DisallowMultipleComponent]
    public class FigureController : MonoBehaviour
    {
        [Space]
        [SerializeField] private List<BlockController> blocks = new List<BlockController>();

        public int BlocksCount => blocks.Count;
        
        public BlockController GetBlock(int index)
        {
            if (index < 0 || index >= BlocksCount) return null;
            return blocks[index];
        }
        
        public void GetMinMax(out Vector3Int figureMin, out Vector3Int figureMax)
        {
            figureMin = new Vector3Int(int.MaxValue, int.MaxValue);
            figureMax = new Vector3Int(int.MinValue, int.MinValue);
            
            foreach (var block in blocks)
            {
                figureMin.x = Mathf.Min(figureMin.x, block.GetCell().x);
                figureMin.y = Mathf.Min(figureMin.y, block.GetCell().y);
                
                figureMax.x = Mathf.Max(figureMax.x, block.GetCell().x);
                figureMax.y = Mathf.Max(figureMax.y, block.GetCell().y);
            }
        }
        
        public Vector3Int GetSize()
        {
            GetMinMax(out var figureMin, out var figureMax);

            return new Vector3Int(figureMax.x - figureMin.x + 1, figureMax.y - figureMin.y + 1);
        }
        
        public Vector3 GetWorldCenter()
        {
            if (BlocksCount == 0) return transform.position;

            var worldCenter = Vector3.zero;

            for (var i = 0; i < BlocksCount; i++)
            {
                worldCenter += GetBlock(i).GetWorldCenter();
            }

            worldCenter /= BlocksCount;
            
            return worldCenter;
        }
        
        public void SetupFigure()
        {
            SetupBlocks();
        }

        private void SetupBlocks()
        {
            blocks.Clear();
            foreach (var block in GetComponentsInChildren<BlockController>(false))
            {
                blocks.Add(block);
            }
        }
        
        private void Awake()
        {
            SetupFigure();
        }
    }
}