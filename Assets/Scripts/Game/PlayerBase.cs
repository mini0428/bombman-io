using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PlayerBase : MonoBehaviour
    {
        public float moveSpeed = 5;
    
        public Animator animator;
        public Rigidbody rb;

        public List<GameObject> bombs = new List<GameObject>();
    
        public float dieForce = 1000;
        public float lineCountWaitTime = 5;
    
        public Transform uiFollowPosition;
    
        protected Tile currentTile = null;
    
        public float bombNextDropWaitTime = 1;
        public Bomb bombPrefab;

        protected string uiName = string.Empty;
        protected float addBombElapsed = -1;
        protected int bombCount = 0;
    
        [System.NonSerialized]
        public bool isLive = false;
    
        protected int lineCount = 1;

        private float lineCountElapsed = 0;

        public virtual void Setup(Vector3 position, string uiName)
        {
            this.uiName = uiName;
            transform.position = position;
            transform.localEulerAngles = new Vector3(0, 270, 0);
        
            addBombElapsed = 1;
            bombCount = 0;
            isLive = false;
            lineCount = 1;
        
            lineCountElapsed = lineCountWaitTime;
        }
    
        public virtual void StartGame()
        {
            isLive = true;
        }

        public virtual void OnChangeTile()
        {
        }
    
        public virtual void BaseUpdate()
        {
            var tile = GameManager.Instance.GetTile(transform.position);
            if (tile != null && currentTile != tile)
            {
                if( currentTile != null )
                {
                    currentTile.OnLeave(this);
                }

                currentTile = tile;
                {
                    currentTile.OnEnter(this);
                    OnChangeTile();
                }
            }
        
            if (bombCount < bombs.Count)
            {
                addBombElapsed -= Time.deltaTime;
                if (addBombElapsed <= 0)
                {
                    addBombElapsed = bombNextDropWaitTime;
                    bombs[bombCount].SetActive(true);
                    bombCount++;
                }
            }
        
            animator.SetFloat("has_bomb",bombCount > 0 ? 0 : 1);

            lineCountElapsed -= Time.deltaTime;
            if (lineCountElapsed <= 0)
            {
                lineCountElapsed = lineCountWaitTime;
                lineCount++;
            }
            
        }

        public virtual void Die()
        {
            if( !isLive )
                return;

            isLive = false;
            animator.Play("die", -1, 0);
            rb.AddForce(new Vector3(0, dieForce, 0));
        }
    
        protected void AddBomb()
        {
            if( bombCount == 0 || currentTile == null )
                return;
        
            var bomb = GameObject.Instantiate(bombPrefab, GameManager.Instance.bombRoot.transform);

            var pos = currentTile.transform.position;
            pos.y = 0;
            bomb.transform.position = pos;
            bomb.Setup(this, currentTile, lineCount);
        
            bombs[bombCount - 1].SetActive(false);
            bombCount--;
            animator.SetFloat("has_bomb",bombCount > 0 ? 0 : 1);
        }

        public void OnWin()
        {
            animator.Play("win", -1, 0);
            foreach (var bomb in bombs)
            {
                bomb.SetActive(false);
            }
        }

        public void LookAt(Vector3 pos)
        {
            pos.y = transform.position.y;
            transform.LookAt(pos);
        }
    }
}
