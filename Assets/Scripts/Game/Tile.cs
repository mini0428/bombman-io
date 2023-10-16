using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Tile : MonoBehaviour
    {
        public GameObject bombRoot;

        public Renderer mainRenderer;
        public Color mainColor;
        public Color stepColor;

        private List<PlayerBase> playerBases = new List<PlayerBase>();
        private List<Bomb> bombs = new List<Bomb>();
    
        private int refCount = 0;

        private void Awake()
        {
            mainColor = mainRenderer.material.color;
        }

        void Start()
        {
            bombRoot.SetActive(false);
        }

        public void OnEnter(PlayerBase playerBase)
        {
            playerBases.Add(playerBase);
        
            mainRenderer.material.color = stepColor;
        }

        public void OnLeave(PlayerBase playerBase)
        {
            playerBases.Remove(playerBase);
        
            mainRenderer.material.color = mainColor;
        }

        public bool IsEmptyTile()
        {
            return bombs.Count == 0 && refCount <= 0;
        }

        public void BombMark(bool active)
        {
            if (active)
                refCount++;
            else
                refCount--;

            if (refCount < 0)
                refCount = 0;
        
            bombRoot.SetActive(refCount > 0);
        }

        public void Explosion()
        {
            foreach (var playerBase in playerBases)
            {
                playerBase.Die();
            }

            List<Bomb> temp = new List<Bomb>();
            temp.AddRange(bombs);
            bombs.Clear();
        
            foreach (var bomb in temp)
            {
                bomb.Explosion();
            }
        }

        public void AddBomb(Bomb bomb)
        {
            bombs.Add(bomb);
        }
    }
}
