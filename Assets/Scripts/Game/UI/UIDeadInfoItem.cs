using TMPro;
using UnityEngine;
using Util;

namespace Game.UI
{
    public class UIDeadInfoItem : MonoBehaviour
    {
        public TextMeshProUGUI label = null;
        public float elaspedTime = 3;

        public void Setup(string uiName)
        {
            label.text = $"<color=red>{uiName}</color> is dead";
        }


        void Update()
        {
            elaspedTime -= Time.deltaTime;
            if( elaspedTime <= 0 )
                gameObject.Destroy();
        }
    }
}
