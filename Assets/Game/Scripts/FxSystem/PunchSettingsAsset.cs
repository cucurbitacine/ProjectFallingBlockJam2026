using UnityEngine;

namespace Game.Scripts.FxSystem
{
    [CreateAssetMenu(menuName = "Create PunchSettingsAsset", fileName = "PunchSettingsAsset", order = 0)]
    public class PunchSettingsAsset : ScriptableObject
    {
        public Vector3 punch = Vector3.one;
        public float duration = 1f;
        public int vibrato = 10;
        public float elasticity = 1f;
    }
    
    public enum PunchType
    {
        Position,
        Rotation,
        Scale,
    }
}