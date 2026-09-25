using UnityEngine;

namespace Game.Scripts.FxSystem
{
    [CreateAssetMenu(menuName = "Create MoveSettingsAsset", fileName = "MoveSettingsAsset", order = 0)]
    public class MoveSettingsAsset : ScriptableObject
    {
        public Vector3 endValue = Vector3.up;
        public float duration = 1f;
        public bool snapping = false;
    }
}