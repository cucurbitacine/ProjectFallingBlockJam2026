using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.Scripts.UISystem
{
    public class HoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private UnityEvent pointerEnter = new UnityEvent();
        [SerializeField] private UnityEvent pointerExit = new UnityEvent();
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            pointerEnter.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            pointerExit.Invoke();
        }
    }
}
