using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Figures
{
    [DisallowMultipleComponent]
    public class FigureController : MonoBehaviour
    {
        [SerializeField] private BlockController rotationBlock;
        
        private readonly List<BlockController> blocks = new List<BlockController>();

        public int BlocksCount => blocks.Count;
        
        public BlockController GetBlock(int index)
        {
            if (index < 0 || index >= BlocksCount) return null;
            return blocks[index];
        }
        
        public bool Contains(BlockController block)
        {
            return blocks.Contains(block);
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
            return GetSize(out _, out _);
        }
        
        public Vector3Int GetSize(out Vector3Int figureMin, out Vector3Int figureMax)
        {
            GetMinMax(out figureMin, out figureMax);

            return new Vector3Int(figureMax.x - figureMin.x + 1, figureMax.y - figureMin.y + 1);
        }
        
        public Vector3 GetWorldCenter(Grid grid)
        {
            if (BlocksCount == 0) return transform.position;
            
            var worldCenter = Vector3.zero;
            for (var i = 0; i < BlocksCount; i++)
            {
                worldCenter += grid.GetCellCenterWorld(GetBlock(i).GetCell());
            }
            worldCenter /= BlocksCount;
            
            return worldCenter;
        }
        
        public Vector3 GetWorldRotationCenter(Grid grid)
        {
            if (rotationBlock && blocks.Contains(rotationBlock))
            {
                return grid.GetCellCenterWorld(rotationBlock.GetCell());
            }
            
            return GetWorldCenter(grid);
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