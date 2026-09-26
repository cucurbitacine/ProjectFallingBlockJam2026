using Game.Scripts.FxSystem;
using Game.Scripts.PlayerSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UISystem
{
    public class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField] private TextPopupAsset damagePopup;
        [SerializeField] private TextPopupAsset healPopup;
        [SerializeField] private RectTransform popupAnchor;
        
        [Space]
        [SerializeField] private Image[] health;
        
        private PawnPlayerController _player;
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
        
        public void SetupUI(PawnPlayerController player, Camera camera)
        {
            _selfRect = GetComponent<RectTransform>();
            _player = player;
            _camera = camera;
        }

        public void EnableUI()
        {
            _player.GetHealth().ValueChanged += OnPlayerHealthChanged;
            _player.GetHealth().Damaged += OnPlayerDamaged;
            _player.GetHealth().Healed += OnPlayerHealed;

            _isPlaying = true;
        }

        public void DisableUI()
        {
            _isPlaying = false;
            
            _player.GetHealth().ValueChanged -= OnPlayerHealthChanged;
            _player.GetHealth().Damaged -= OnPlayerDamaged;
            _player.GetHealth().Healed -= OnPlayerHealed;
        }

        private void UpdateUI(float deltaTime)
        {
            //var worldPosition = _player.GetPawn().GetWorldCenter();
            //UpdatePosition(worldPosition, _camera);
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
        
        private void OnPlayerDamaged(int amount)
        {
            var worldPosition = (Vector2) _camera.ScreenToWorldPoint(popupAnchor.transform.position);
            damagePopup.Popup($"-{amount}", worldPosition);
        }

        private void OnPlayerHealed(int amount)
        {
            var worldPosition = (Vector2) _camera.ScreenToWorldPoint(popupAnchor.transform.position);
            healPopup.Popup($"+{amount}", worldPosition);
        }
        
        private void OnPlayerHealthChanged(int previous, int current)
        {
            var minCount = Mathf.Min(health.Length, current);
            for (var i = 0; i < health.Length; i++)
            {
                //health[i].enabled = i < minCount;
                health[i].gameObject.SetActive(i < minCount);
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