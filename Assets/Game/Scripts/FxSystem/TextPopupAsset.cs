using UnityEngine;

namespace Game.Scripts.FxSystem
{
    [CreateAssetMenu(menuName = "Create TextPopupAsset", fileName = "TextPopupAsset", order = 0)]
    public class TextPopupAsset : ScriptableObject
    {
        [SerializeField] private TextPopup textPopupPrefab;

        public TextPopup Popup(Vector3 position)
        {
            var textPopup = Instantiate(GetPrefab(), position, Quaternion.identity);
            
            return textPopup;
        }
        
        public TextPopup Popup(string text, Vector3 position)
        {
            var textPopup = Popup(position);
            
            textPopup.ChangeText(text);
            
            return textPopup;
        }

        private TextPopup GetPrefab()
        {
            return textPopupPrefab;
        }
    }
}