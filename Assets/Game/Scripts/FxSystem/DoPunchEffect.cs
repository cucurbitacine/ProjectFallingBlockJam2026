using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.FxSystem
{
    public class DoPunchEffect : DoEffect
    {
        [Header("DoPunch")]
        [SerializeField] private PunchSettingsAsset punchSettings;
        [Space]
        [SerializeField] private PunchType punchType = PunchType.Scale;
        [SerializeField] private bool useWorldSpace;

        public override Tweener CreateTweener()
        {
            var punch = punchSettings.punch;

            if (useWorldSpace)
            {
                punch = transform.InverseTransformVector(punchSettings.punch);
            }

            return DoPunch(punch, punchSettings.duration, punchSettings.vibrato, punchSettings.elasticity);
        }
        
        private Tweener DoPunch(Vector3 punch, float duration, int vibrato, float elasticity)
        {
            return DoPunch(transform, punchType, punch, duration, vibrato, elasticity);
        }
        
        private static Tweener DoPunch(Transform origin, PunchType punchType, Vector3 punch, float duration, int vibrato, float elasticity)
        {
            switch (punchType)
            {
                case PunchType.Position: return origin.DOPunchPosition(punch, duration, vibrato, elasticity);
                case PunchType.Rotation: return origin.DOPunchRotation(punch, duration, vibrato, elasticity);
                case PunchType.Scale: return origin.DOPunchScale(punch, duration, vibrato, elasticity);
            }

            return null;
        }
    }
}
