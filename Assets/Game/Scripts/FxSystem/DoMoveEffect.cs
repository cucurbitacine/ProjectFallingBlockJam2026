using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.FxSystem
{
    public class DoMoveEffect : DoEffect
    {
        [Header("DoMove")]
        [SerializeField] private MoveSettingsAsset moveSettings;
        [SerializeField] private bool useWorldSpace;
        
        public override Tweener CreateTweener()
        {
            var endValue = transform.TransformPoint(moveSettings.endValue);

            if (useWorldSpace)
            {
                endValue = transform.position + moveSettings.endValue;
            }

            return transform.DOMove(endValue, moveSettings.duration, moveSettings.snapping);
        }
    }
}