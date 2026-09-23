using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Game.Scripts.CombatSystem
{
    public class PeriodEventInvoker : MonoBehaviour
    {
        [SerializeField] private bool isPlaying;
        [SerializeField] private float actualTime;
        [SerializeField] private float actualPeriod;
        [Header("Settings")]
        [SerializeField] private bool playOnStart;
        [Min(0f)]
        [SerializeField] private float periodMin = 1f;
        [Min(0f)]
        [SerializeField] private float periodThreshold = 0f;

        [Header("Event")]
        [SerializeField] private UnityEvent periodEvent = new UnityEvent();
        
        private float periodMax => periodMin + periodThreshold;
        private float timeLeft => actualPeriod - actualTime;

        public bool IsPlaying() => isPlaying;
        
        public void Play()
        {
            RandomizePeriod();
            
            isPlaying = true;
        }
        
        public void Stop()
        {
            isPlaying = false;
        }

        public void AddListener(UnityAction call)
        {
            periodEvent.AddListener(call);
        }
        
        public void RemoveListener(UnityAction call)
        {
            periodEvent.RemoveListener(call);
        }
        
        private float GetRandomPeriod()
        {
            return Random.Range(periodMin, periodMax);
        }

        private void RandomizePeriod()
        {
            actualPeriod = GetRandomPeriod();
        }
        
        private bool IsReadyInvoke()
        {
            return timeLeft <= 0f;
        }

        private void Invoke()
        {
            periodEvent.Invoke();
        }

        private void ResetTime()
        {
            actualTime = 0f;
        }

        private void UpdateTimer(float deltaTime)
        {
            actualTime += deltaTime;
        }
        
        private void UpdateInvoker(float deltaTime)
        {
            UpdateTimer(deltaTime);

            if (IsReadyInvoke())
            {
                Invoke();
                
                ResetTime();

                RandomizePeriod();
            }
        }
        
        private void Start()
        {
            if (playOnStart) Play();
        }

        private void Update()
        {
            if (IsPlaying())
            {
                UpdateInvoker(Time.deltaTime);
            }
        }
    }
}