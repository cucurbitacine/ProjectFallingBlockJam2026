using TMPro;
using UnityEngine;

namespace Game.Scripts.FxSystem
{
    public class TextPopup : MonoBehaviour
    {
        [SerializeField] private string message;
        [SerializeField] private TMP_Text label;
        
        public void ChangeText(string text)
        {
            message = text;

            label.text = message;
        }
    }
}
