using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
    public class UITouchToStart : MonoBehaviour, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            GameManager.Instance.SetGameStep(GameManager.GameStep.PlayGame);
        }
    }
}