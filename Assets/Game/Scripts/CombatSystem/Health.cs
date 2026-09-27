using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.CombatSystem
{
    public class Health : MonoBehaviour
    {
        [Min(0)]
        [SerializeField] private int value;
        [Min(1)]
        [SerializeField] private int maxValue = 1;

        [Space]
        [SerializeField] private UnityEvent onDamage = new UnityEvent();
        
        public event Action<int, int> ValueChanged; 
        public event Action<int> Damaged; 
        public event Action<int> Healed;
        
        public int GetValue()
        {
            return value;
        }
        
        public int GetMaxValue()
        {
            return maxValue;
        }

        public void SetValue(int nextValue)
        {
            nextValue = Mathf.Clamp(nextValue, 0, GetMaxValue());
            if (value == nextValue) return;
            var prevValue = value;
            value = nextValue;
            ValueChanged?.Invoke(prevValue, nextValue);
        }
        
        public int Damage(int amount)
        {
            if (amount <= 0) return 0;

            if (amount > GetValue())
            {
                amount = GetValue();
            }
            
            if (amount <= 0) return 0;
            
            SetValue(GetValue() - amount);
            
            Damaged?.Invoke(amount);

            onDamage.Invoke();
            
            return amount;
        }
        
        public int Heal(int amount)
        {
            if (amount <= 0) return 0;
            
            if (GetValue() + amount > GetMaxValue())
            {
                amount = GetMaxValue() - GetValue();
            }
            
            if (amount <= 0) return 0;
            
            SetValue(GetValue() + amount);
            
            Healed?.Invoke(amount);

            return amount;
        }
    }
}
