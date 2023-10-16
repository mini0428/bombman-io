using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class UIPlayerCount : MonoBehaviour
    {
        public TextMeshProUGUI label;
        void Update()
        {
            label.text = GameManager.Instance.ToPlayerInfoString();
        }
    }
}
