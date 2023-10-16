using TMPro;
using UnityEngine;
using Util;

namespace Game.UI
{
    public class UIFollowName : MonoBehaviour
    {
        public TextMeshProUGUI label = null;
        public PlayerBase player = null;
    
        void Start()
        {
        }

        public void Setup(PlayerBase player, string name)
        {
            this.player = player;
            label.text = name;
            Update();
        }

        void Update()
        {
            transform.position = UIUtil.WorldToUISpace(UICommon.MainCam, UICommon.UiCam, UICommon.Canvas, player.uiFollowPosition.position);
        }
    }
}
