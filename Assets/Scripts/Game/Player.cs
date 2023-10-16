using DG.Tweening;
using Game.UI;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class Player : PlayerBase
    {
        public UIJoystick joystick;

        private bool isKeyDown = false;

        public override void Setup(Vector3 position, string uiName)
        {
            base.Setup(position, uiName);
        
            animator.Play("idle", -1, 0);
            animator.SetBool("move", false);
            animator.SetFloat("has_bomb",1);

            foreach (var bomb in bombs)
            {
                bomb.SetActive(false);
            }
        }

        private bool DoMove(Vector3 pos)
        {
            var current = transform.position;
            current += pos * moveSpeed * Time.deltaTime;
        
            NavMeshHit hit;
            if (NavMesh.SamplePosition(current, out hit, 0.1f, -1) )
            {
                transform.position = current;
                return true;
            }
        
            return false;
        }
    
        void Update()
        {
            if (!isLive || !GameManager.Instance.isLive)
                return;
        
            var mousePos = new Vector3(joystick.yAxis.value, 0, -joystick.xAxis.value);
            bool isMove = !mousePos.Equals(Vector3.zero);

            if (isMove)
            {
                var current = transform.position;
                current += mousePos * moveSpeed * Time.deltaTime;
                LookAt(current);
            
                if (!DoMove(mousePos))
                {
                    var adjustPos = mousePos;
                    adjustPos.x = 0;
                    if (!DoMove(adjustPos))
                    {
                        adjustPos = mousePos;
                        adjustPos.z = 0;
                        DoMove(adjustPos);
                    }
                }
            }
            animator.SetBool("move", isMove);

            bool key = Input.GetMouseButton(0);
            if (key != isKeyDown)
            {
                if (isKeyDown)
                {
                    AddBomb();
                }
            
                isKeyDown = key;
            }
        
            BaseUpdate();
        }

        public override void Die()
        {
            if( !isLive )
                return;
        
            base.Die();
            DOVirtual.DelayedCall(2.5f, () =>
            {
                GameManager.Instance.SetGameStep(GameManager.GameStep.Fail);
            });
        }
    }
}
