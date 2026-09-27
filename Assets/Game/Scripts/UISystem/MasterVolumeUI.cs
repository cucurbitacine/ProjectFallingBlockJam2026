using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Game.Scripts.UISystem
{
    public class MasterVolumeUI : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup mixerGroup;
        [SerializeField] private Slider slider;

        private const float MinVolume = -80f;
        private const float MaxVolume = 20f;

        private const string MasterVolumeParam = "MasterVolume";
        
        private float GetMasterVolume()
        {
            mixerGroup.audioMixer.GetFloat(MasterVolumeParam, out var volume);
            
            return volume;
        }
        
        private void SetMasterVolume(float volume)
        {
            mixerGroup.audioMixer.SetFloat(MasterVolumeParam, volume);
        }
        
        private void OnSliderValueChanged(float value)
        {
            var volume = ConvertSliderToVolume(value);
            
            SetMasterVolume(volume);
        }

        private float ConvertSliderToVolume(float value)
        {
            var minValue = slider.minValue;
            var maxValue = slider.maxValue;
            var normalizedValue = (value - minValue) / (maxValue - minValue);
            return normalizedValue * (MaxVolume - MinVolume) + MinVolume;
        }
        
        private float ConvertVolumeToSlider(float volume)
        {
            var minValue = slider.minValue;
            var maxValue = slider.maxValue;
            var normalizedValue = (volume - MinVolume) / (MaxVolume - MinVolume);
            return normalizedValue * (maxValue - minValue) + minValue;
        }

        private void StartSlider()
        {
            var volume = GetMasterVolume();
            var value = ConvertVolumeToSlider(volume);
            slider.SetValueWithoutNotify(value);
            
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        
        private void StopSlider()
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
        
        private void Start()
        {
            StartSlider();
        }

        private void OnDestroy()
        {
            StopSlider();
        }
    }
}
