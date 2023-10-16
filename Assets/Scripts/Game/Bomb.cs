using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Game
{
    public class Bomb : MonoBehaviour
    {
        public float bombWaitTime = 4;
        public GameObject explosionEffect;
    
        private List<Tile> tileLists = new List<Tile>();
        private Tile currentTile;
    
        private float bombCount = 0;
        void Update()
        {
            if( !GameManager.Instance.isLive)
                return;
        
            bombCount -= Time.deltaTime;
            if (bombCount <= 0)
            {
                Explosion();
            }
        }

        public void Explosion()
        {
            if( currentTile == null )
                return;

            var temp = currentTile;
            currentTile = null;
        
            temp.Explosion();
        
            foreach (var tile in tileLists)
            {
                tile.BombMark(false);
                var effect = GameObject.Instantiate(explosionEffect);
                effect.transform.position = tile.transform.position + new Vector3(0, 0.5f, 0);
                tile.Explosion();
            }
            
            var effectRoot = GameObject.Instantiate(explosionEffect);
            effectRoot.transform.position = transform.position + new Vector3(0, 0.5f, 0);
        
            gameObject.Destroy();
        }

        public void Setup(PlayerBase playerBase, Tile tile, int lineCount)
        {
            currentTile = tile;
            currentTile.AddBomb(this);
        
            tileLists = GameManager.Instance.GetTiles(tile.transform.position, lineCount);
            foreach (var list in tileLists)
            {
                list.BombMark(true);
            }
            bombCount = bombWaitTime;
        }
    }
}
