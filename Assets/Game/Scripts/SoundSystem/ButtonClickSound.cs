using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.SoundSystem
{
    [RequireComponent(typeof(SoundSource))]
    public class ButtonClickSound : MonoBehaviour
    {
        [SerializeField] private Button buttonObject;
        private SoundSource soundSource;
        
        private void OnButtonClick()
        {
            soundSource.Play();
        }
        
        private void Awake()
        {
            soundSource = GetComponent<SoundSource>();
        }
        
        private void OnEnable()
        {
            buttonObject.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            buttonObject.onClick.RemoveListener(OnButtonClick);
        }
    }
}
