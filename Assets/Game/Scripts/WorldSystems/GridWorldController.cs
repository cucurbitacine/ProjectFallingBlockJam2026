using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        
        [Header("Required")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform figureContainer;
        [SerializeField] private Grid worldGrid;
        [SerializeField] private FigureProfileStorageAsset figureStorage;

        private FigureType[] figureTypes;
        private FigureState[] figureStates;
        
        public override IEnumerator EnableWorld()
        {
            yield return base.EnableWorld();

            figureTypes = (FigureType[])Enum.GetValues(typeof(FigureType));
            figureStates = (FigureState[])Enum.GetValues(typeof(FigureState));
        }
        
        public Vector2 GetSpawnPoint()
        {
            return spawnPoint == null ? transform.position : spawnPoint.position;
        }

        public Grid GetWorldGrid()
        {
            return worldGrid;
        }

        public void LaunchFallingFigure(int height)
        {
            MergeFallingFigure();
            
            var randomFigureType = figureTypes[Random.Range(0, figureTypes.Length)];
            var randomFigureState = figureStates[Random.Range(0, figureStates.Length)];

            fallingFigure = SpawnFigure(randomFigureType, randomFigureState);
            var figureCell = new Vector3Int(0, height);
            fallingFigure.SetCell(figureCell);
            fallingFigure.transform.position = worldGrid.CellToWorld(figureCell);
        }

        [SerializeField] private int minHeight = 0;

        private readonly List<BlockController> blocksReuse = new List<BlockController>();
        
        public bool IsPossibleMoveY(int offset)
        {
            // Check every block in Figure
            fallingFigure.GetBlocks(blocksReuse);
            foreach (var block in blocksReuse)
            {
                var blockCell = worldGrid.WorldToCell(block.GetWorldPosition());

                // Check every Step
                for (var i = 1; i <= Mathf.Abs(offset); i++)
                {
                    var y = blockCell.y + (int)Mathf.Sign(offset) * i;
                    
                    //Check Ground
                    if (y < minHeight) return false;
                    
                    if (tower.TryGetValue(y, out var line))
                    {
                        // Check every block in Line
                        foreach (var blockInLine in line)
                        {
                            var blockCellInLine = worldGrid.WorldToCell(blockInLine.GetWorldPosition());
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
        
        public void MoveY(int offset)
        {
            var cell = fallingFigure.GetCell() + Vector3Int.up * offset;

            fallingFigure.SetCell(cell);
        }

        public void MergeFallingFigure()
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
        
        private FigureController SpawnFigure(FigureType figureType, FigureState figureState)
        {
            var figureProfile = figureStorage.GetFigureProfile(figureType);
            var figureAsset = figureProfile.GetFigure(figureState);

            var figurePrefab = figureAsset.GetPrefab();

            return Instantiate(figurePrefab, figureContainer);
        }

        private void MergeFigure(FigureController figure)
        {
            var blocks = new List<BlockController>();
            figure.GetBlocks(blocks);
            foreach (var block in blocks)
            {
                var blockCell = worldGrid.WorldToCell(block.transform.position);
                var height = blockCell.y;

                if (!tower.TryGetValue(height, out var row))
                {
                    row = new List<BlockController>();
                    tower[height] = row;
                }
                
                row.Add(block);
            }
        }

        [Min(0f)]
        [SerializeField] private float figureSpeed = 5f;
        
        private void Update()
        {
            if (fallingFigure)
            {
                var targetPosition = worldGrid.CellToWorld(fallingFigure.GetCell());
                var finalPosition = Vector2.Lerp(fallingFigure.transform.position, targetPosition, Time.deltaTime * figureSpeed);
                fallingFigure.transform.position = finalPosition;
            }
        }
    }
}
