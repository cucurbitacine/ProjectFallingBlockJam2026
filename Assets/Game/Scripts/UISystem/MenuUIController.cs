using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Core.LevelSystem;
using Game.Scripts.Core.UISystem;
using Game.Scripts.SoundSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UISystem
{
    public class MenuUIController : UIController
    {
        [SerializeField] private SoundSource soundSource;
        [SerializeField] private SoundFxPreset buttonHoverSfx;
        [SerializeField] private SoundFxPreset buttonClickSfx;
        
        private readonly List<Button> buttons = new List<Button>();
        private readonly List<HoverEvent> hovers = new List<HoverEvent>();
        
        public override IEnumerator EnableUI(LevelController level)
        {
            yield return base.EnableUI(level);

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

        public override void DisableUI()
        {
            base.DisableUI();
            
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
