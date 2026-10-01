using System.Collections.Generic;
using System.Linq;
using Game.Scripts.SoundSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UISystem
{
    public class MenuUIController : MonoBehaviour
    {
        [SerializeField] private SoundSource soundSource;
        [SerializeField] private SoundFxPreset buttonHoverSfx;
        [SerializeField] private SoundFxPreset buttonClickSfx;
        
        private readonly List<Button> buttons = new List<Button>();
        private readonly List<HoverEvent> hovers = new List<HoverEvent>();
        
        public void EnableUI()
        {
            GetComponentsInChildren<Button>(true, buttons);
            hovers.AddRange(buttons.Select(b => b.GetComponent<HoverEvent>()).Where(h => h != null));

            foreach (var button in buttons)
            {
                button.onClick.AddListener(OnButtonClick);
            }
            
            foreach (var hover in hovers)
            {
                hover.onEnter.AddListener(OnButtonHover);
            }
        }

        public void DisableUI()
        {
            foreach (var button in buttons)
            {
                button.onClick.RemoveListener(OnButtonClick);
            }
            
            foreach (var hover in hovers)
            {
                hover.onEnter.RemoveListener(OnButtonHover);
            }
        }

        private void OnButtonClick()
        {
            soundSource.Play(buttonClickSfx);
        }
        
        private void OnButtonHover()
        {
            soundSource.Play(buttonHoverSfx);
        }
    }
}
