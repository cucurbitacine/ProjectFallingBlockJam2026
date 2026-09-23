using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UISystem
{
    public class PlayerTimeLeftUI : MonoBehaviour
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private Slider slider;
        [SerializeField] private Image fill;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Gradient colorGradient;

        private Canvas _canvas;
        private RectTransform _canvasRect;

        public void SetCanvas(Canvas canvas)
        {
            _canvas = canvas;
            _canvasRect = _canvas.GetComponent<RectTransform>();
        }
        
        public void UpdateUI(Vector3 worldPositon, Camera camera, float timeLeft, float timeTotal)
        {
            UpdatePosition(worldPositon, camera);
            UpdateTime(timeLeft, timeTotal);
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