using System;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.SoundSystem
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Sound/Sound Pack", fileName = "Sound Pack", order = 0)]
    public class SoundPack : ScriptableObject
    {
        [SerializeField] private Pack[] packs;
        
        [Serializable]
        public struct Pack
        {
            public int SoundType;
            public SoundFxPreset SoundFx;
        }

        public bool TryGetSoundFx(int soundType, out SoundFxPreset soundFx)
        {
            return soundFx = packs.FirstOrDefault(s => s.SoundType == soundType).SoundFx;
        }
    }
}