using UnityEngine;

namespace Game.Scripts.SoundSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundSource : MonoBehaviour
    {
        [field: SerializeField] public SoundFxPreset SoundFx { get; private set; }
        public AudioSource AudioSource { get; private set; }

        public void Play()
        {
            if (!SoundFx.PlayOneShot) Stop();
            
            AudioSource.volume = SoundFx.Volume;
            AudioSource.loop = SoundFx.Looped;
            AudioSource.outputAudioMixerGroup = SoundFx.AudioMixerGroup;
            AudioSource.spatialBlend = SoundFx.SpatialBlend;
            AudioSource.minDistance = SoundFx.MinDistance;
            AudioSource.maxDistance = SoundFx.MaxDistance;

            if (SoundFx.PlayOneShot)
            {
                AudioSource.PlayOneShot(SoundFx.GetAudioClip(), SoundFx.Volume);
            }
            else
            {
                AudioSource.resource = SoundFx.GetAudioClip();
                AudioSource.Play();
            }
        }
        
        public void Play(SoundFxPreset soundFx)
        {
            SoundFx = soundFx;
            
            Play();
        }
        
        public void Stop()
        {
            if (AudioSource.isPlaying)
            {
                AudioSource.Stop();
            }
        }
        
        private void InitAudio()
        {
            AudioSource = GetComponent<AudioSource>();
            AudioSource.playOnAwake = false; 
            
            if (SoundFx && SoundFx.PlayOnEnable)
            {
                AudioSource.playOnAwake = false;
                AudioSource.clip = SoundFx.GetAudioClip();
            }
        }
        
        private void Awake()
        {
            InitAudio();
        }

        private void OnEnable()
        {
            if (SoundFx && SoundFx.PlayOnEnable) Play();
        }
    }
}