using UnityEngine;

namespace Game.Scripts
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class BlockController : MonoBehaviour
    {
        [SerializeField] private Vector2Int localPosition;
        
        private Collider2D _collider;

        public Vector2Int GetLocalPosition()
        {
            return localPosition;
        }
        
        public Collider2D GetCollider()
        {
            return _collider;
        }
        
        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        [ContextMenu(nameof(Rename))]
        private void Rename()
        {
            var x = (int)transform.position.x;
            var y = (int)transform.position.y;
            name = $"Block_{x}{y}";
        }
    }
}
