using Game.Scripts.LevelSystem;
using UnityEngine;
using UnityEngine.U2D;

namespace Game.Scripts.FxSystem
{
    public class SpaghettiController : MonoBehaviour
    {
        [SerializeField] private SpriteShapeController spriteShape;
        [SerializeField] private GameLevelController gameLevel;
        [SerializeField] private float height  = 0.2f;

        public float GetLenght()
        {
            var length = 0f;
            for (var i = 1; i < spriteShape.spline.GetPointCount(); i++)
            {
                length += Vector3.Distance(spriteShape.spline.GetPosition(i), spriteShape.spline.GetPosition(i - 1));
            }
            return length;
        }
        
        private void SetupBeginPoint(Vector3 beginPoint)
        {
            var localPoint = transform.InverseTransformPoint(beginPoint);
            var count = spriteShape.spline.GetPointCount();
            var beginIndex = 0;
            if (count != 0)
            {
                spriteShape.spline.SetPosition(beginIndex, localPoint);
            }
            else
            {
                spriteShape.spline.InsertPointAt(beginIndex, localPoint);
            }
            
            spriteShape.spline.SetCorner(beginIndex, true);
            spriteShape.spline.SetHeight(beginIndex, height);
            spriteShape.spline.SetTangentMode(beginIndex, ShapeTangentMode.Linear);
        }

        private void SetupEndPoint(Vector3 endPoint)
        {
            var localPoint = transform.InverseTransformPoint(endPoint);
            var count = spriteShape.spline.GetPointCount();
            var endIndex = count;
            if (count == 1)
            {
                
                spriteShape.spline.InsertPointAt(endIndex, localPoint);
            }
            else
            {
                endIndex = count - 1;
                spriteShape.spline.SetPosition(endIndex, localPoint);
            }
            
            spriteShape.spline.SetCorner(endIndex, true);
            spriteShape.spline.SetHeight(endIndex, height);
            spriteShape.spline.SetTangentMode(endIndex, ShapeTangentMode.Linear);
        }
        
        private void SetupBeginEndPoints(Vector3 beginPoint, Vector3 endPoint)
        {
            spriteShape.spline.Clear();
            
            SetupBeginPoint(beginPoint);
            SetupEndPoint(endPoint);
            
            spriteShape.RefreshSpriteShape();
        }

        private void UpdateTangents(int index)
        {
            var prev = spriteShape.spline.GetPosition(index - 1);
            var point = spriteShape.spline.GetPosition(index);
            var next = spriteShape.spline.GetPosition(index + 1);
                
            var scale = Mathf.Min(
                (next - point).magnitude,
                (prev - point).magnitude
            ) * 0.33f;
                
            SplineUtility.CalculateTangents(point, prev, next, transform.forward, scale, out var right, out var left);
                
            spriteShape.spline.SetTangentMode(index, ShapeTangentMode.Continuous);
            spriteShape.spline.SetLeftTangent(index, left);
            spriteShape.spline.SetRightTangent(index, right);
        }
        
        private void UpdateEndPoint(Vector3 endPoint)
        {
            var localPoint = transform.InverseTransformPoint(endPoint);
            var endIndex = spriteShape.spline.GetPointCount() - 1;
            
            spriteShape.spline.SetPosition(endIndex, localPoint);

            if (endIndex > 1)
            {
                UpdateTangents(endIndex - 1);
            }
            
            spriteShape.RefreshSpriteShape();
        }
        
        private void AddPoint(Vector3 worldPoint)
        {
            var localPoint = transform.InverseTransformPoint(worldPoint);
            var count = spriteShape.spline.GetPointCount();
            var pointIndex = count - 1;
            
            spriteShape.spline.InsertPointAt(pointIndex, localPoint);
            spriteShape.spline.SetCorner(pointIndex, true);
            spriteShape.spline.SetHeight(pointIndex, height);
            spriteShape.spline.SetTangentMode(pointIndex, ShapeTangentMode.Continuous);

            UpdateTangents(pointIndex);
            
            spriteShape.RefreshSpriteShape();
        }
        
        private void OnScoreChanged(int oldScore, int score)
        {
            var bestCell = gameLevel.GetBestCell();
            var gridWorld = gameLevel.GetGridWorld();
            var worldPoint = gridWorld.GetGrid().GetCellCenterWorld(bestCell) + Vector3.down * 0.5f;
            AddPoint(worldPoint);
        }
        
        private void OnGameStateChanged(GameState exit, GameState enter)
        {
            if (exit is GameState.Initializing)
            {
                SetupBeginEndPoints(transform.position, gameLevel.GetPawnPlayer().GetPawn().GetWorldCenter());
            }
        }
        
        private void Start()
        {
            gameLevel.GameStateChanged += OnGameStateChanged;
            gameLevel.ScoreChanged += OnScoreChanged;
        }

        private void OnDestroy()
        {
            gameLevel.GameStateChanged -= OnGameStateChanged;
            gameLevel.ScoreChanged -= OnScoreChanged;
        }

        private void Update()
        {
            if (gameLevel.GetGameState() is (GameState.FigureFalling or GameState.Consequence or GameState.Win))
            {
                UpdateEndPoint(gameLevel.GetPawnPlayer().GetPawn().GetWorldCenter());
            }
        }
    }
}
