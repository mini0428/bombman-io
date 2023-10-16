using System.Collections.Generic;
using DG.Tweening;
using Game.UI;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class AIPlayer : PlayerBase
    {
        // 이동 경로 저장
        private List<Vector3> movePath = new List<Vector3>();

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
        
            movePath.Clear();
        
            isLive = false;
            gameObject.SetActive(true);
        }

        
        private void PathFind()
        {
            var navMeshPath = new NavMeshPath();
            
            // 빈 타일을 찾아서 랜덤 하게 이동
            if (NavMesh.CalculatePath(transform.position, GameManager.Instance.GetRandomEmptyTilePosition(), -1,
                    navMeshPath))
            {
                // Path가 있으면 해당 위치 저장
                movePath.Clear();
                movePath.AddRange(navMeshPath.corners);
                
                // 첫 번 째 Path는 내 위치로 찍히기 때문에 제거 해줌
                if( movePath.Count > 1 )
                    movePath.RemoveAt(0);
            }
        }
    
        private void Update()
        {
            // 게임이 시작 되어야 Update 가능
            if (!isLive || !GameManager.Instance.isLive)
                return;
        
            BaseUpdate();

            // 이동할 Path가 없으면 다시 길을 찾음
            if (movePath.Count == 0)
            {
                PathFind();
            
                if( bombCount > 0 )
                    AddBomb();
            }

            // 이동할 Path가 있으면 해당 위치로 이동 시킨
            if (movePath.Count > 0)
            {
                // 목적지 정보를 얻어 옴
                var target = movePath[0];
                // 내 위치
                var pos = transform.position;
                // 방향 값 얻어 옴
                var dir = (target - pos);

                // 남은 거리
                var remainDistance = dir.magnitude;

                // 이동 가능한 거리
                float needMoveDistance = moveSpeed * Time.deltaTime;

                // 이동 가능한 거리가 남은 거리 보다 작거나 같으면 해당 위치에 도착
                // 이렇게 계산 하면 랙이 걸리거나, moveSpeed가 아무리 큰 값이 나와도 원하는 위치에 정확하게 도착
                if (remainDistance <= needMoveDistance)
                {   // 도착!!
                    // 현재 위치를 목적지 위치로 설정
                    transform.position = target;
                    // 이동 경로 하나 삭제
                    movePath.RemoveAt(0);
                }
                else
                {   // 아직 도착 하지 않았기 때문에 이동 가능한 거리로 이동
                    pos += dir.normalized * needMoveDistance;
                    // 대상 위치를 바라본다
                    LookAt(pos);
                    // 이동
                    transform.position = pos;
                }
            }

            // 이동 애니메이션 설정 
            animator.SetBool("move", IsMoveAgent());
        }

        private bool IsMoveAgent()
        {
            return movePath.Count > 0;
        }

        public override void Die()
        {
            if( !isLive || !GameManager.Instance.isLive)
                return;
        
            base.Die();
        
            UIMain.Instance.OnDeadMessage(uiName);
            GameManager.Instance.OnDeathEnemy();
        
            DOVirtual.DelayedCall(2f, () =>
            {
                gameObject.SetActive(false);
                if( currentTile != null )
                    currentTile.OnLeave(this);
            });
        }

        public override void OnChangeTile()
        {
            if (!currentTile.IsEmptyTile())
            {
                PathFind();
            }
        }
    }
}
