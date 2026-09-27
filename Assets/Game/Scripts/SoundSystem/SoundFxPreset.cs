using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Game.Scripts.SoundSystem
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Sound/SoundFx Preset", fileName = "SoundFxPreset", order = 0)]
    public class SoundFxPreset : ScriptableObject
    {
        [field: SerializeField] public AudioMixerGroup AudioMixerGroup { get; private set; }
        [field: Range(0f, 1f)]
        [field: SerializeField] public float Volume { get; private set; } = 1f;
        
        [field: Header("SFX")]
        [field: SerializeField] public bool PlayOneShot { get; private set; } = false;
        
        [field: Header("Ambient")]
        [field: SerializeField] public bool PlayOnEnable { get; private set; } = false;
        [field: SerializeField] public bool Looped { get; private set; } = false;
        
        [field: Header("Spatial")]
        [field: Tooltip("0.0f (2D) - 1.0f (3D)")]
        [field: Range(0f, 1f)]
        [field: SerializeField] public float SpatialBlend { get; private set; } = 0f;
        [field: Min(0f)]
        [field: SerializeField] public float MinDistance { get; private set; } = 1f;
        [field: Min(0f)]
        [field: SerializeField] public float MaxDistance { get; private set; } = 10f;
        
        [field: Header("Clips")]
        [SerializeField] private AudioClip[] clips;

        public AudioClip GetAudioClip()
        {
            return clips[Random.Range(0, clips.Length)];
        }
    }
}