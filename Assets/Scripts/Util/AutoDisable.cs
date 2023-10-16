using UnityEngine;

namespace Util
{
    public class AutoDisable : MonoBehaviour
    {
        [SerializeField]
        private float liveTime = 3;

        private float time;

        private void OnEnable()
        {
            time = liveTime;
        }

        void Update()
        {
            time -= Time.deltaTime;
            if( time < 0 )
                gameObject.SetActive(false);
        }
    }
}
