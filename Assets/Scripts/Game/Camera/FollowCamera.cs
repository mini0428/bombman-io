using UnityEngine;

namespace Game.Camera
{
    [ExecuteInEditMode]
    public class FollowCamera : MonoBehaviour
    {
        public Transform target = null;
        public Vector3 rot = Vector3.zero;
        public float zLerpTime = 0.3f;
    
    
        void Update()
        {
            if( target == null )
                return;

            var targetPos = target.transform.position;
            var pos = transform.position;
            targetPos += rot;

            pos = Vector3.Lerp(pos, targetPos, Time.deltaTime * zLerpTime);
            transform.position = pos;
        }

        public void Setup()
        {
            if( target == null )
                return;

            var targetPos = target.transform.position;
            targetPos += rot;
        
            transform.position = targetPos;
        }
    }
}
