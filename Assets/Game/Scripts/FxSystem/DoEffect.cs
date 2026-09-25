using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.FxSystem
{
    public abstract class DoEffect : MonoBehaviour
    {
        [SerializeField] private EffectEvent effectEvent = EffectEvent.Manual;
        
        private Tweener _tweener;
        
        public abstract Tweener CreateTweener();
        
        [ContextMenu(nameof(LaunchEffect))]
        public void LaunchEffect()
        {
            KillTweener();
            
            _tweener = CreateTweener();
        }
        
        public void KillTweener()
        {
            _tweener?.Kill(true);
        }
        
        protected virtual void OnEnable()
        {
            if (effectEvent is EffectEvent.OnEnable) LaunchEffect();
        }
        
        protected virtual void Start()
        {
            if (effectEvent is EffectEvent.Start) LaunchEffect();
        }
        
        protected virtual void OnDisable()
        {
            KillTweener();
        }
    }
    
    public enum EffectEvent
    {
        Manual,
        OnEnable,
        Start,
    }
}