using Game.Scripts.CombatSystem;
using UnityEngine;

namespace Game.Scripts.FxSystem
{
    [RequireComponent(typeof(Health))]
    public class DamageHighlight : MonoBehaviour
    {
        private static readonly int HighlightEnabled = Shader.PropertyToID("_HighlightEnabled");
        private static readonly int HighlightColor = Shader.PropertyToID("_HighlightColor");

        [SerializeField] private float highlightDuration = 0.2f;
        [SerializeField] private Color highlightColor;
        [Space]
        [SerializeField] private SpriteRenderer sprite;

        private bool _isDamaged;
        private float _timeLastDamage = float.MinValue;
        private Material _spriteMaterial;
        private Health _health;
        
        private void OnDamageReceived(int damageAmount)
        {
            _isDamaged = true;

            _timeLastDamage = Time.time;
            
            Highlight(true);
        }
        
        private void UpdateHighlight()
        {
            if (!_isDamaged) return;
            
            var now = Time.time;
            var timeSinceLastDamage = now - _timeLastDamage;
            if (timeSinceLastDamage >= highlightDuration)
            {
                _isDamaged = false;
                Highlight(false);
            }
        }

        private void Highlight(bool isOn)
        {
            if (isOn)
            {
                _spriteMaterial.SetColor(HighlightColor, highlightColor);
            }
            
            _spriteMaterial.SetFloat(HighlightEnabled, isOn ? 1 : 0);
        }
        
        private void Awake()
        {
            _health = GetComponent<Health>();
            _spriteMaterial = sprite.material;
        }

        private void OnEnable()
        {
            _health.Damaged += OnDamageReceived;
        }

        private void OnDisable()
        {
            _health.Damaged -= OnDamageReceived;
        }

        private void Update()
        {
            UpdateHighlight();
        }
    }
}
