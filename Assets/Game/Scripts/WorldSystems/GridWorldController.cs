using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Core.WorldSystem;
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

        public void LaunchFigureAtHeight(FigureType figureType, FigureState figureState, int height)
        {
            MergeFigure();
            
            var figureSpawnCenterCell = new Vector3Int(0, height);
            var figureSpawnCenter = worldGrid.CellToWorld(figureSpawnCenterCell);
            
            fallingFigure = SpawnFigure(figureType, figureState, figureSpawnCenter);
            UpdateBlockCells(fallingFigure);
            
            var figureCenter = fallingFigure.GetWorldCenter();
            var figureOffset = figureSpawnCenter - figureCenter;
            fallingFigure.transform.position += figureOffset;
            UpdateBlockCells(fallingFigure);
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
            return false;
        }

        public void RotateFigure()
        {
            
        }
        
        public void MergeFigure()
        {
            if (fallingFigure != null)
            {
                MergeFigure(fallingFigure);
            }
        }
        
        public int CheckFullLines()
        {
            var count = 0;

            foreach (var line in tower)
            {
                if (line.Value.Count >= 10)
                {
                    count++;
                    // TODO Destroy Line
                }
            }
            
            return count;
        }
        
        private FigureController SpawnFigure(FigureType figureType, FigureState figureState, Vector2 spawnPosition)
        {
            var figureProfile = figureStorage.GetFigureProfile(figureType);
            var figureAsset = figureProfile.GetFigure(figureState);

            var figurePrefab = figureAsset.GetPrefab();

            return Instantiate(figurePrefab, spawnPosition, Quaternion.identity, figureContainer);
        }

        private void UpdateBlockCells(FigureController figure)
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
                
                row.Add(block);
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
        
        private void Update()
        {
            if (fallingFigure)
            {
                for (var i = 0; i < fallingFigure.BlocksCount; i++)
                {
                    var block = fallingFigure.GetBlock(i);
                    UpdateBlockPosition(block, Time.deltaTime);
                }
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
    }
}
