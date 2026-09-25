using Game.Scripts.FxSystem;
using Game.Scripts.LevelSystem;
using Game.Scripts.PlayerSystem;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UISystem
{
    public class GameScoreUI : MonoBehaviour
    {
        [SerializeField] private TextPopupAsset scoredPopup;
        [SerializeField] private TMP_Text score;
        [SerializeField] private TMP_Text length;
        
        private PawnPlayerController _player;
        private GameLevelController _level;
        private Camera _camera;
        private Canvas _canvas;
        private RectTransform _selfRect;
        private RectTransform _canvasRect;
        private bool _isPlaying;
        
        public void SetCanvas(Canvas canvas)
        {
            _canvas = canvas;
            _canvasRect = _canvas.GetComponent<RectTransform>();
        }
        
        public void SetupUI(PawnPlayerController player, GameLevelController level, Camera camera)
        {
            _selfRect = GetComponent<RectTransform>();
            _player = player;
            _level = level;
            _camera = camera;

            OnGameScoreChanged(0, _level.GetScore());
        }

        public void EnableUI()
        {
            _level.ScoreChanged += OnGameScoreChanged;

            _isPlaying = true;
        }

        public void DisableUI()
        {
            _isPlaying = false;
            
            _level.ScoreChanged -= OnGameScoreChanged;
        }

        private void UpdateUI(float deltaTime)
        {
            var worldPosition = _player.GetPawn().GetWorldCenter();

            UpdatePosition(worldPosition, _camera);

            var spaghettiLength = _level.GetSpaghetti().GetLenght();
            length.text = $"{spaghettiLength:F1}m";
        }
        
        private void UpdatePosition(Vector3 worldPosition, Camera camera)
        {
            var screenPosition = camera.WorldToScreenPoint(worldPosition);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect,
                screenPosition,
                _canvas.worldCamera,
                out var localPosition
            );

            _selfRect.anchoredPosition = localPosition;
        }
        
        private void OnGameScoreChanged(int previous, int current)
        {
            if (!score.gameObject.activeInHierarchy) return;
            
            score.text = $"{current}";

            var delta = current - previous;
            if (delta > 0)
            {
                var worldPosition = (Vector2)_camera.ScreenToWorldPoint(score.transform.position);
                
                scoredPopup.Popup($"+{delta}", worldPosition);
            }
        }
        
        private void Update()
        {
            if (_isPlaying)
            {
                UpdateUI(Time.deltaTime);
            }
        }
    }
}