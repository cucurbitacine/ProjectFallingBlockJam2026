using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    [DisallowMultipleComponent]
    public class FigureController : MonoBehaviour
    {
        [SerializeField] private Vector3Int localCell;
        [Space]
        [SerializeField] private Vector3Int figureSize;
        [SerializeField] private Vector3Int figureMin;
        [SerializeField] private Vector3Int figureMax;
        [Space]
        [SerializeField] private Grid grid;
        
        [Space]
        [SerializeField] private Dictionary<Vector3Int, BlockController> blocks = new Dictionary<Vector3Int, BlockController>();
        
        public Vector3Int GetCell()
        {
            return localCell;
        }

        public void SetCell(Vector3Int cell)
        {
            localCell = cell;
        }
        
        public Vector3Int GetSize()
        {
            return figureSize;
        }
        
        public int GetBlocks(List<BlockController> result)
        {
            result.Clear();
            result.AddRange(blocks.Values);
            return result.Count;
        }
        
        private void SetupFigure()
        {
            SetupBlocks();
            SetupSize();
        }

        private void SetupBlocks()
        {
            blocks.Clear();

            var children = GetComponentsInChildren<BlockController>();
            foreach (var block in children)
            {
                var local = block.transform.localPosition;
                block.SetCell(grid.LocalToCell(local));
                local = grid.GetCellCenterLocal(block.GetCell());
                block.transform.localPosition = local;

                blocks[block.GetCell()] = block;
            }
        }
        
        private void SetupSize()
        {
            figureMin = new Vector3Int(int.MaxValue, int.MaxValue);
            figureMax = new Vector3Int(int.MinValue, int.MinValue);

            foreach (var cell in blocks.Keys)
            {
                figureMin.x = Mathf.Min(figureMin.x, cell.x);
                figureMin.y = Mathf.Min(figureMin.y, cell.y);
                
                figureMax.x = Mathf.Max(figureMax.x, cell.x);
                figureMax.y = Mathf.Max(figureMax.y, cell.y);
            }

            figureSize = new Vector3Int(figureMax.x - figureMin.x + 1, figureMax.y - figureMin.y + 1);
        }
        
        private void Awake()
        {
            SetupFigure();
        }

        private void OnDrawGizmos()
        {
            if (grid)
            {
                var min = grid.GetCellCenterWorld(figureMin);
                var max = grid.GetCellCenterWorld(figureMax);
                var center = (max + min) * 0.5f;
                Gizmos.DrawWireCube(center, Vector3.Scale(figureSize, grid.cellSize));
            }
        }

#if UNITY_EDITOR
        [ContextMenu(nameof(SetupFigure))]
        private void EditorSetupFigure()
        {
            SetupFigure();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

    public static class FigureExtension
    {
        
    }
}