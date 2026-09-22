using System;
using UnityEngine;

namespace Game.Scripts.CombatSystem
{
    public class Health : MonoBehaviour
    {
        [Min(0)]
        [SerializeField] private int value;
        [Min(1)]
        [SerializeField] private int maxValue = 1;

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
        
        public void Damage(int amount)
        {
            if (amount >= 0) return;
            
            SetValue(GetValue() - amount);
            
            Damaged?.Invoke(amount);
        }
        
        public void Heal(int amount)
        {
            if (amount <= 0) return;
            
            SetValue(GetValue() + amount);
            
            Healed?.Invoke(amount);
        }
    }
}
