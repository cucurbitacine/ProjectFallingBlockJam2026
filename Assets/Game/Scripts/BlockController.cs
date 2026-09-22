using UnityEngine;

namespace Game.Scripts
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class BlockController : MonoBehaviour
    {
        [SerializeField] private Vector3Int localCell;
        [SerializeField] private SpriteRenderer sprite;
        
        private Collider2D _collider;

        public Vector3Int GetCell()
        {
            return localCell;
        }

        public void SetCell(Vector3Int cell)
        {
            localCell = cell;
        }
        
        public Collider2D GetCollider()
        {
            return _collider;
        }
        
        public SpriteRenderer GetSpriteRenderer()
        {
            return sprite;
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

        public Vector3 GetWorldCenter()
        {
            return transform.position;
        }
    }
}
