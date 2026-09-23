using UnityEngine;

namespace Game.Scripts.CombatSystem
{
    [DisallowMultipleComponent]
    public class DestroyAfterTime : MonoBehaviour
    {
        [Min(0f)]
        [SerializeField] private float time = 5f;

        private void DestroyMe()
        {
            Destroy(gameObject);
        }
        
        private void Start()
        {
            Invoke(nameof(DestroyMe), time);
        }
    }
}