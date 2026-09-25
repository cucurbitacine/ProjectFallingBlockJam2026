using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Core.WorldSystem;
using Game.Scripts.Figures;
using UnityEngine;

namespace Game.Scripts.WorldSystems
{
    public class GridWorldController : WorldController
    {
        [Header("Grid World")]
        [SerializeField] private FigureController fallingFigure;
        //[SerializeField]
        private readonly Dictionary<int, List<BlockController>> tower = new Dictionary<int, List<BlockController>>();

        [SerializeField] private List<SpriteRenderer> hints = new List<SpriteRenderer>();
        
        [Header("Settings")]
        [Min(0f)]
        [SerializeField] private float figureFallingSpeed = 10f;
        [SerializeField] private Vector2Int gridBoundX = new Vector2Int(-5, 4);
        [SerializeField] private Vector2Int gridBoundY = new Vector2Int(0, 1000);
        
        [Header("Required")]
        [SerializeField] private Transform figureContainer;
        [SerializeField] private Grid worldGrid;
        [SerializeField] private FigureProfileStorageAsset figureStorage;

        #region Public API

        public event Action<FigureController, GridWorldController> FigureMerged;
        public event Action<int, GridWorldController> LineDestroyed;
        
        public Grid GetGrid()
        {
            return worldGrid;
        }

        public Dictionary<int, List<BlockController>> GetTower()
        {
            return tower;
        }
        
        public FigureController GetFallingFigure()
        {
            return fallingFigure;
        }
        
        public bool LaunchFigureAtHeight(FigureType figureType, FigureRotation figureRotation, int height)
        {
            var grid = GetGrid();
            
            // Merge previous Figure
            MergeFigure();
            
            // Where should be falling position
            var figureSpawnCenterCell = new Vector3Int(0, height);
            var figureSpawnCenter = grid.CellToWorld(figureSpawnCenterCell);
            
            // Spawn and initial cell setup
            fallingFigure = InstantiateFigure(figureType, figureSpawnCenter);
            fallingFigure.SetupFigure();
            UpdateFigureCell(fallingFigure);
            
            // Offset to falling position
            var figureCenter = fallingFigure.GetWorldCenter(grid);
            var figureOffset = figureSpawnCenter - figureCenter;
            fallingFigure.transform.position += figureOffset;
            UpdateFigureCell(fallingFigure);

            // Do random rotation
            for (var i = 0; i < (int)figureRotation; i++)
            {
                if (IsPossibleRotateFigure())
                {
                    RotateFigure();
                }
                else break;
            }
            
            // Check if it hits tower
            if (!CheckFigureIntersectWithTower(fallingFigure))
            {
                return false;
            }
            
            // Move all blocks
            UpdateBlocksPosition(fallingFigure, 0f);
            return true;
        }
        
        public bool IsPossibleMoveFigureY(int offset, out int maxOffset)
        {
            maxOffset = 0;
            
            // Check every Step
            for (var step = 1; step <= Mathf.Abs(offset); step++)
            {
                // Check every block in Figure
                for (var i = 0; i < fallingFigure.BlocksCount; i++)
                {
                    var block = fallingFigure.GetBlock(i);
                    var blockCell = block.GetCell();
                    var y = blockCell.y + (int)Mathf.Sign(offset) * step;

                    //Check Ground
                    if (y < gridBoundY.x) return false;
                    if (y > gridBoundY.y) return false;

                    if (GetTower().TryGetValue(y, out var line))
                    {
                        // Check every block in Line
                        foreach (var blockInLine in line)
                        {
                            var blockCellInLine = blockInLine.GetCell();
                            if (blockCellInLine.x == blockCell.x)
                            {
                                return false;
                            }
                        }
                    }
                }
                    
                maxOffset++;
            }

            return true;
        }
        
        public void MoveFigureY(int offset)
        {
            for (var i = 0; i < fallingFigure.BlocksCount; i++)
            {
                var block = fallingFigure.GetBlock(i);
                var blockCell = block.GetCell() + Vector3Int.up * offset;
                block.SetCell(blockCell);
            }
        }
        
        public bool IsPossibleMoveFigureX(int offset, out int maxOffset)
        {
            maxOffset = 0;
            
            // Check every Step
            for (var step = 1; step <= Mathf.Abs(offset); step++)
            {
                // Check every block in Figure
                for (var i = 0; i < fallingFigure.BlocksCount; i++)
                {
                    var block = fallingFigure.GetBlock(i);
                    var blockCell = block.GetCell();
                    var x = blockCell.x + (int)Mathf.Sign(offset) * step;
                    
                    //Check Ground
                    if (x < gridBoundX.x) return false;
                    if (x > gridBoundX.y) return false;
                    
                    if (GetTower().TryGetValue(blockCell.y, out var line))
                    {
                        // Check every block in Line
                        foreach (var blockInLine in line)
                        {
                            var blockCellInLine = blockInLine.GetCell();
                            if (blockCellInLine.x == x)
                            {
                                return false;
                            }
                        }
                    }
                }
                    
                maxOffset++;
            }
            
            return true;
        }
        
        public void MoveFigureX(int offset)
        {
            for (var i = 0; i < fallingFigure.BlocksCount; i++)
            {
                var block = fallingFigure.GetBlock(i);
                var blockCell = block.GetCell() + Vector3Int.right * offset;
                block.SetCell(blockCell);
            }
        }
        
        public bool IsPossibleRotateFigure()
        {
            var grid = GetGrid();
            
            var rotation = Quaternion.Euler(0f, 0, 90f);
            var centerOfRotation = fallingFigure.GetWorldRotationCenter(grid);
            for (var i = 0; i < fallingFigure.BlocksCount; i++)
            {
                var block = fallingFigure.GetBlock(i);
                var blockCell = block.GetCell();
                var blockCenter = grid.GetCellCenterWorld(blockCell);
                var blockVector = blockCenter - centerOfRotation;
                var blockVectorRotated = rotation * blockVector;
                var blockCenterRotated = blockVectorRotated + centerOfRotation;
                var blockCellRotated = grid.WorldToCell(blockCenterRotated);

                if (blockCellRotated.x < gridBoundX.x || blockCellRotated.x > gridBoundX.y) return false;
                if (blockCellRotated.y < gridBoundY.x || blockCellRotated.y > gridBoundY.y) return false;
                
                if (GetTower().TryGetValue(blockCellRotated.y, out var line))
                {
                    if (line.Any(b => b.GetCell().x == blockCellRotated.x))
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }

        public void RotateFigure()
        {
            var grid = GetGrid();
            
            var rotation = Quaternion.Euler(0f, 0, 90f);
            var centerOfRotation = fallingFigure.GetWorldRotationCenter(grid);
            for (var i = 0; i < fallingFigure.BlocksCount; i++)
            {
                var block = fallingFigure.GetBlock(i);
                var blockCell = block.GetCell();
                var blockCenter = grid.GetCellCenterWorld(blockCell);
                var blockVector = blockCenter - centerOfRotation;
                var blockVectorRotated = rotation * blockVector;
                var blockCenterRotated = blockVectorRotated + centerOfRotation;
                var blockCellRotated = grid.WorldToCell(blockCenterRotated);
                block.SetCell(blockCellRotated);
                UpdateBlockPosition(block, 0f);
            }
        }
        
        public void MergeFigure()
        {
            if (fallingFigure != null)
            {
                MergeFigure(fallingFigure);

                fallingFigure = null;
            }
        }
        
        public int DestroyFullLines()
        {
            var destroyedLineCount = 0;
            
            foreach (var (height, blocks) in GetTower())
            {
                var isFullLine = true;
                for (var x = gridBoundX.x; x <= gridBoundX.y; x++)
                {
                    if (blocks.Any(b => b.GetCell().x == x)) continue;
                    isFullLine = false;
                    break;
                }

                if (!isFullLine) continue;

                DestroyLine(height, blocks);
                destroyedLineCount++;
            }

            return destroyedLineCount;
        }

        public bool IsLowerThen(float height)
        {
            for (var i = 0; i < fallingFigure.BlocksCount; i++)
            {
                var block = fallingFigure.GetBlock(i);
                var blockCell = block.GetCell();
                var blockWorldCenter = GetGrid().GetCellCenterWorld(blockCell);
                if (blockWorldCenter.y >= height) return false;
            }
            
            return true;
        }

        public void SkipFigure()
        {
            Destroy(fallingFigure.gameObject);
            fallingFigure = null;
        }
        
        #endregion

        #region Private API

        private FigureController InstantiateFigure(FigureType figureType, Vector2 spawnPosition)
        {
            var figureProfile = figureStorage.GetFigureProfile(figureType);
            //var figureAsset = figureProfile.GetFigure(figureState);
            var figureAsset = figureProfile.GetFigureDefault();

            var figurePrefab = figureAsset.GetPrefab();

            return Instantiate(figurePrefab, spawnPosition, Quaternion.identity, figureContainer);
        }

        private void UpdateFigureCell(FigureController figure)
        {
            for (var i = 0; i < figure.BlocksCount; i++)
            {
                UpdateBlockCell(figure.GetBlock(i));
            }
        }
        
        private void UpdateBlockCell(BlockController block)
        {
            var blockCenter = block.GetWorldCenter();
            var blockCell = GetGrid().WorldToCell(blockCenter);
            block.SetCell(blockCell);
        }
        
        private void MergeFigure(FigureController figure)
        {
            var towerTarget = GetTower();
            
            for (var i = 0; i < figure.BlocksCount; i++)
            {
                var block = figure.GetBlock(i);
                var blockCell = block.GetCell();

                UpdateBlockPosition(block, 0f);
                
                if (!towerTarget.TryGetValue(blockCell.y, out var row))
                {
                    row = new List<BlockController>();
                    towerTarget[blockCell.y] = row;
                }

                if (!row.Contains(block))
                {
                    row.Add(block);
                }
            }
            
            FigureMerged?.Invoke(figure, this);
        }

        private void UpdateBlocksPosition(FigureController figure, float deltaTime)
        {
            for (var i = 0; i < figure.BlocksCount; i++)
            {
                var block = fallingFigure.GetBlock(i);
                UpdateBlockPosition(block, deltaTime);
            }
        }
        
        private void UpdateBlockPosition(BlockController block, float deltaTime)
        {
            var targetPosition = GetGrid().GetCellCenterWorld(block.GetCell());
            
            if (deltaTime > 0f)
            {
                targetPosition = Vector2.Lerp(block.transform.position, targetPosition, deltaTime * figureFallingSpeed);
            }
            
            block.transform.position = targetPosition;
        }
        
        private bool CheckFigureIntersectWithTower(FigureController figure)
        {
            for (var i = 0; i < figure.BlocksCount; i++)
            {
                var block = figure.GetBlock(i);
                var blockCell = block.GetCell();
                if (GetTower().TryGetValue(blockCell.y, out var line))
                {
                    if (line.Any(b => b.GetCell().x == blockCell.x))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void DestroyLine(int height, List<BlockController> blocks)
        {
            try
            {
                LineDestroyed?.Invoke(height, this);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                foreach (var block in blocks)
                {
                    Destroy(block.gameObject);
                }
                blocks.Clear();
            }
        }
        
        private void DestroyLine(int height)
        {
            if (GetTower().TryGetValue(height, out var blocks))
            {
                DestroyLine(height, blocks);
            }
        }

        private void UpdateHint(FigureController figure, float deltaTime)
        {
            if (figure == null)
            {
                foreach (var hint in hints)
                {
                    hint.enabled = false;
                }
                
                return;
            }
            
            var minCount = Mathf.Min(figure.BlocksCount, hints.Count);

            if (minCount == 0)
            {
                foreach (var hint in hints)
                {
                    hint.enabled = false;
                }
                
                return;
            }

            IsPossibleMoveFigureY(-1000, out var maxOffset);
            
            for (var i = 0; i < hints.Count; i++)
            {
                var hint = hints[i];
                
                if (i < minCount)
                {
                    var block = figure.GetBlock(i);
                    var fallCell = block.GetCell() + Vector3Int.down * maxOffset;
                    var fallPoint = GetGrid().GetCellCenterWorld(fallCell);
                    if (hint.enabled)
                    {
                        /*
                        if (deltaTime > 0f)
                        {
                            fallPoint = Vector2.Lerp(hint.transform.position, fallPoint,
                                deltaTime * figureFallingSpeed);
                        }
                        */
                        hint.transform.position = fallPoint;
                    }
                    else
                    {
                        var blockSprite = block.GetSpriteRenderer();
                        if (blockSprite)
                        {
                            hint.sprite = blockSprite.sprite;
                        }
                        
                        hint.transform.position = fallPoint;
                        hint.enabled = true;
                    }
                    
                }
                else
                {
                    hint.enabled = false;
                }
            }
        }
        
        #endregion
        
        private void Update()
        {
            if (fallingFigure)
            {
                UpdateBlocksPosition(fallingFigure, Time.deltaTime);
            }
            
            UpdateHint(fallingFigure, Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            var grid = GetGrid();
            if (!grid) return;
            Gizmos.color = Color.softRed;
            var min = grid.CellToWorld(new Vector3Int(gridBoundX.x, gridBoundY.x));
            var max = grid.CellToWorld(new Vector3Int(gridBoundX.y, gridBoundY.y)) + grid.cellSize;
            var gridBoundCenter = (max + min) * 0.5f;
            var gridBoundSize = max - min;
            Gizmos.DrawWireCube(gridBoundCenter, gridBoundSize);

            if (fallingFigure)
            {
                IsPossibleMoveFigureY(-1000, out var maxOffset);

                if (maxOffset > 0)
                {
                    for (var i = 0; i < fallingFigure.BlocksCount; i++)
                    {
                        var fallCell = fallingFigure.GetBlock(i).GetCell() + Vector3Int.down * maxOffset;
                        var fallPoint = GetGrid().GetCellCenterWorld(fallCell);
                        Gizmos.DrawWireSphere(fallPoint, 0.2f);
                    }
                }
            }
        }
    }

    public static class GridWorldExtension
    {
        public static bool IsPossibleMoveFigureY(this GridWorldController world, int offset)
        {
            return world.IsPossibleMoveFigureY(offset, out _);
        }

        public static bool IsPossibleMoveFigureX(this GridWorldController world, int offset)
        {
            return world.IsPossibleMoveFigureX(offset, out _);
        }
    }
}
