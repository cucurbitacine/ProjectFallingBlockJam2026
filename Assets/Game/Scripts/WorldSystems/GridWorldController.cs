using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Core.WorldSystem;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.WorldSystems
{
    public class GridWorldController : WorldController
    {
        [Header("Grid World")]
        [SerializeField] private FigureController fallingFigure;
        [SerializeField] private Dictionary<int, List<BlockController>> tower = new Dictionary<int, List<BlockController>>();
        
        [Header("Settings")]
        [Min(0f)]
        [SerializeField] private float figureFallingSpeed = 10f;
        [SerializeField] private Vector2Int gridBoundX = new Vector2Int(-5, 4);
        [SerializeField] private Vector2Int gridBoundY = new Vector2Int(0, 1000);
        
        [Header("Required")]
        [SerializeField] private Transform figureContainer;
        [SerializeField] private Grid worldGrid;
        [SerializeField] private FigureProfileStorageAsset figureStorage;
        
        public Grid GetWorldGrid()
        {
            return worldGrid;
        }

        public bool LaunchFigureAtHeight(FigureType figureType, FigureState figureState, int height)
        {
            MergeFigure();
            
            var figureSpawnCenterCell = new Vector3Int(0, height);
            var figureSpawnCenter = worldGrid.CellToWorld(figureSpawnCenterCell);
            
            fallingFigure = SpawnFigure(figureType, figureState, figureSpawnCenter);
            fallingFigure.SetupFigure();
            UpdateFigureCell(fallingFigure);
            
            var figureCenter = fallingFigure.GetWorldCenter();
            var figureOffset = figureSpawnCenter - figureCenter;
            fallingFigure.transform.position += figureOffset;
            UpdateFigureCell(fallingFigure);

            if (!CheckFigureIntersectWithTower(fallingFigure)) return false;
            
            UpdateBlocksPosition(fallingFigure, 0f);
            return true;
        }
        
        public bool IsPossibleMoveFigureY(int offset)
        {
            // Check every block in Figure
            for (var i = 0; i < fallingFigure.BlocksCount; i++)
            {
                var block = fallingFigure.GetBlock(i);
                var blockCell = block.GetCell();

                // Check every Step
                for (var step = 1; step <= Mathf.Abs(offset); step++)
                {
                    var y = blockCell.y + (int)Mathf.Sign(offset) * step;

                    //Check Ground
                    if (y < gridBoundY.x) return false;
                    if (y > gridBoundY.y) return false;

                    if (tower.TryGetValue(y, out var line))
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

        public bool IsPossibleMoveFigureX(int offset)
        {
            // Check every block in Figure
            for (var i = 0; i < fallingFigure.BlocksCount; i++)
            {
                var block = fallingFigure.GetBlock(i);
                var blockCell = block.GetCell();

                // Check every Step
                for (var step = 1; step <= Mathf.Abs(offset); step++)
                {
                    var x = blockCell.x + (int)Mathf.Sign(offset) * step;
                    
                    //Check Ground
                    if (x < gridBoundX.x) return false;
                    if (x > gridBoundX.y) return false;
                    
                    if (tower.TryGetValue(blockCell.y, out var line))
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
            var rotation = Quaternion.Euler(0f, 0, 90f);
            //var centerOfRotation = worldGrid.GetCellCenterWorld(worldGrid.WorldToCell(fallingFigure.GetWorldCenter()));
            var centerOfRotation = fallingFigure.GetWorldCenter();
            for (var i = 0; i < fallingFigure.BlocksCount; i++)
            {
                var block = fallingFigure.GetBlock(i);
                var blockCell = block.GetCell();
                var blockCenter = worldGrid.GetCellCenterWorld(blockCell);
                var blockVector = blockCenter - centerOfRotation;
                var blockVectorRotated = rotation * blockVector;
                var blockCenterRotated = blockVectorRotated + centerOfRotation;
                var blockCellRotated = worldGrid.WorldToCell(blockCenterRotated);

                if (blockCellRotated.x < gridBoundX.x || blockCellRotated.x > gridBoundX.y) return false;
                if (blockCellRotated.y < gridBoundY.x || blockCellRotated.y > gridBoundY.y) return false;
                
                if (tower.TryGetValue(blockCellRotated.y, out var line))
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
            var rotation = Quaternion.Euler(0f, 0, 90f);
            //var centerOfRotation = worldGrid.GetCellCenterWorld(worldGrid.WorldToCell(fallingFigure.GetWorldCenter()));
            var centerOfRotation = fallingFigure.GetWorldCenter();
            for (var i = 0; i < fallingFigure.BlocksCount; i++)
            {
                var block = fallingFigure.GetBlock(i);
                var blockCell = block.GetCell();
                var blockCenter = worldGrid.GetCellCenterWorld(blockCell);
                var blockVector = blockCenter - centerOfRotation;
                var blockVectorRotated = rotation * blockVector;
                var blockCenterRotated = blockVectorRotated + centerOfRotation;
                var blockCellRotated = worldGrid.WorldToCell(blockCenterRotated);
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
        
        public void ClearFullLines()
        {
            foreach (var line in tower)
            {
                var fullLine = true;
                for (var x = gridBoundX.x; x <= gridBoundX.y; x++)
                {
                    if (line.Value.All(b => b.GetCell().x != x))
                    {
                        fullLine = false;
                    }

                    if (!fullLine) break;
                }
                
                if (fullLine)
                {
                    foreach (var block in line.Value)
                    {
                        Destroy(block.gameObject);
                    }
                    
                    line.Value.Clear();
                }
            }
        }
        
        private FigureController SpawnFigure(FigureType figureType, FigureState figureState, Vector2 spawnPosition)
        {
            var figureProfile = figureStorage.GetFigureProfile(figureType);
            var figureAsset = figureProfile.GetFigure(figureState);

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
            var blockCell = worldGrid.WorldToCell(blockCenter);
            block.SetCell(blockCell);
        }
        
        private void MergeFigure(FigureController figure)
        {
            for (var i = 0; i < figure.BlocksCount; i++)
            {
                var block = figure.GetBlock(i);
                var blockCell = block.GetCell();

                UpdateBlockPosition(block, 0f);
                
                if (!tower.TryGetValue(blockCell.y, out var row))
                {
                    row = new List<BlockController>();
                    tower[blockCell.y] = row;
                }

                if (!row.Contains(block))
                {
                    row.Add(block);
                }
            }
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
            var targetPosition = worldGrid.GetCellCenterWorld(block.GetCell());
            
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
                if (tower.TryGetValue(blockCell.y, out var line))
                {
                    if (line.Any(b => b.GetCell().x == blockCell.x))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
        private void Update()
        {
            if (fallingFigure)
            {
                UpdateBlocksPosition(fallingFigure, Time.deltaTime);
            }
        }

        private void OnDrawGizmos()
        {
            if (!worldGrid) return;
            Gizmos.color = Color.softRed;
            var min = worldGrid.CellToWorld(new Vector3Int(gridBoundX.x, gridBoundY.x));
            var max = worldGrid.CellToWorld(new Vector3Int(gridBoundX.y, gridBoundY.y)) + worldGrid.cellSize;
            var gridBoundCenter = (max + min) * 0.5f;
            var gridBoundSize = max - min;
            Gizmos.DrawWireCube(gridBoundCenter, gridBoundSize);
        }

        public bool IsLowerThen(float height)
        {
            for (var i = 0; i < fallingFigure.BlocksCount; i++)
            {
                var block = fallingFigure.GetBlock(i);
                var blockCell = block.GetCell();
                var blockWorldCenter = worldGrid.GetCellCenterWorld(blockCell);
                if (blockWorldCenter.y >= height) return false;
            }
            
            return true;
        }

        public void SkipFigure()
        {
            Destroy(fallingFigure.gameObject);
            fallingFigure = null;
        }
    }
}
