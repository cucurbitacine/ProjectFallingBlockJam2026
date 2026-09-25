using Game.Scripts.FxSystem;
using Game.Scripts.LevelSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UISystem
{
    public class PlayerTimeLeftUI : MonoBehaviour
    {
        [SerializeField] private TextPopupAsset timeAddedPopup;
        [Space]
        [SerializeField] private RectTransform rect;
        [SerializeField] private Slider slider;
        [SerializeField] private Image fill;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Gradient colorGradient;

        private GameLevelController _gameLevel;
        private Camera _camera;
        private Canvas _canvas;
        private RectTransform _canvasRect;

        public void SetCanvas(Canvas canvas)
        {
            _canvas = canvas;
            _canvasRect = _canvas.GetComponent<RectTransform>();
        }

        public void SetupUI(GameLevelController level, Camera camera)
        {
            _gameLevel = level;
            _camera = camera;
        }
        
        public void UpdateUI(Vector3 worldPositon, Camera camera, float timeLeft, float timeTotal)
        {
            UpdatePosition(worldPositon, camera);
            UpdateTime(timeLeft, timeTotal);
        }

        public void EnableUI()
        {
            _gameLevel.TimeAdded += OnTimeAdded;
        }

        public void DisableUI()
        {
            _gameLevel.TimeAdded -= OnTimeAdded;
        }
        
        private void OnTimeAdded(float deltaTime)
        {
            var worldPosition = (Vector2) _camera.ScreenToWorldPoint(label.transform.position);
            timeAddedPopup.Popup($"+{deltaTime:F1}s", worldPosition);
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

            rect.anchoredPosition = localPosition;
        }
        
        private void UpdateTime(float timeLeft, float timeTotal)
        {
            slider.value = timeLeft / timeTotal;

            fill.color = colorGradient.Evaluate(slider.value);
            
            label.text = $"{timeLeft:F1}s";
        }
    }
}