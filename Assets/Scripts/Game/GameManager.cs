using System.Collections.Generic;
using Game.Camera;
using Game.Table;
using Game.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Util;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public enum GameStep
        {
            Ready,
            PlayGame,
            Fail,
            Success
        }
    
        public static GameManager Instance;
        public static UnityAction<GameStep> onChangeGameStep;

        public Tile tilePrefab;
        public Player player;

        public List<AIPlayer> aiPlayers = new List<AIPlayer>();
        public List<UIFollowName> followNames = new List<UIFollowName>();
    
        public GameObject bgRoot;
        public GameObject bombRoot;
        public FollowCameara followCameara;
        public NavMeshSurface navMeshSurface;

        public int mapGenSeed = 1;

        public int stageX = 20;
        public int stageY = 20;

        [System.NonSerialized] public bool isLive = false;

        private List<Tile> tileInstances = new List<Tile>();
        private int enemyDeathCount = 0;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            SetGameStep(GameStep.Ready);
        }

        public Vector3 GetTilePosition(int x, int y)
        {
            return new Vector3(x - stageX / 2f, 0, y - stageY / 2f);
        }

        public Tile GetTile(Vector3 position)
        {
            int pos = (int)(position.z + stageY/2f + 0.5f) * stageX + (int)(position.x+stageX/2f + 0.5f);
            if (pos < 0 || pos >= tileInstances.Count)
                return null;
        
            return tileInstances[pos];
        }
    
        public Tile GetTile(int x, int y)
        {
            if (x < 0 || x >= stageX || y < 0 || y >= stageY)
                return null;
        
            int pos = y * stageX + x;
            if (pos < 0 || pos >= tileInstances.Count)
                return null;
        
            return tileInstances[pos];
        }
    
        public List<Tile> GetTiles(Vector3 position, int lineCount)
        {
            int x = (int)(position.x + stageX / 2f + 0.5f);
            int y = (int)(position.z + stageY / 2f + 0.5f);

            List<Tile> result = new List<Tile>();

            bool[] blocks = new[] { true, true, true, true };

            for (int i = 0; i < lineCount; i++)
            {
                int index = i + 1;
                Tile tile = null;

                if (blocks[0])
                {
                    tile = GetTile(x + index, y);
                    if (tile != null)
                        result.Add(tile);
                    else
                        blocks[0] = false;
                }

                if (blocks[1])
                {
                    tile = GetTile(x - index, y);
                    if (tile != null)
                        result.Add(tile);
                    else
                        blocks[1] = false;
                }

                if (blocks[2])
                {
                    tile = GetTile(x, y + index);
                    if (tile != null)
                        result.Add(tile);
                    else
                        blocks[2] = false;
                }

                if (blocks[3])
                {
                    tile = GetTile(x, y - index);
                    if (tile != null)
                        result.Add(tile);
                    else
                        blocks[3] = false;
                }
            }

            return result;
        }

        public Vector3 GetRandomTilePosition()
        {
            Vector3 result;
            while (true)
            {
                var tile = tileInstances[UnityEngine.Random.Range(0, tileInstances.Count)];
                if (tile != null)
                {
                    result = tile.transform.position;
                    break;
                }
            }

            return result;
        }
    
        public Vector3 GetRandomEmptyTilePosition()
        {
            Vector3 result;
            while (true)
            {
                var tile = tileInstances[UnityEngine.Random.Range(0, tileInstances.Count)];
                if (tile != null && tile.IsEmptyTile())
                {
                    result = tile.transform.position;
                    break;
                }
            }

            return result;
        }

        // 스테이지 생성
        private void DoGenStage()
        {
            // Random Seed 생성
            System.Random rnd = new System.Random(mapGenSeed);
            // 기존 배경 모두 삭제
            bgRoot.DestroyAllChilds();
            // tileInstances 제거(GameObect는 위에서 삭제가 됨)
            tileInstances.Clear();

            // Tile Loop 시작
            for (int y = 0; y < stageY; y++)
            {            
                for (int x = 0; x < stageX; x++)
                {
                    // 1/6 확률로 빈 공간을 만듬
                    if (rnd.Next(0, 6) == 0)
                    {
                        tileInstances.Add(null);
                        continue;
                    }

                    // 아니면 정상 타일 생성
                    var tile = GameObject.Instantiate(tilePrefab, bgRoot.transform);
                    // 리스트에 집어 넣고
                    tileInstances.Add(tile);
                    // 타일 이름 설정 해 주고
                    tile.gameObject.name = $"{x}-{y}";
                    // 위치 값 생성
                    tile.transform.localPosition = GetTilePosition(x,y);
                }
            }

            // NavMesh가 동적으로 생성 되어야 되기 때문에 타일 생성 후 Rebuild
            navMeshSurface.BuildNavMesh();
        }

        public void SetGameStep(GameStep step)
        {
            if( step == GameStep.PlayGame)
            {
                player.StartGame();
                foreach (var aiPlayer in aiPlayers)
                    aiPlayer.StartGame();
            
                isLive = true;
            }
        
            if (step == GameStep.Ready)
            {
                DoGenStage();
            
                enemyDeathCount = 0;
                bombRoot.DestroyAllChilds();
                player.Setup(GetRandomTilePosition(), "You");

                var randomNames = NameTable.instance.ShuffleName();
                for( int i = 0; i < aiPlayers.Count; i++)
                {
                    aiPlayers[i].Setup(GetRandomTilePosition(), randomNames[i]);
                    followNames[i].Setup(aiPlayers[i], randomNames[i]);
                }

                followCameara.Setup();
            
                isLive = false;
            }
        
            if (step == GameStep.Fail)
            {
                isLive = false;
            }
        
            if (step == GameStep.Success)
            {
                isLive = false;
                player.OnWin();
            }
        
            onChangeGameStep?.Invoke(step);
        }

        public void OnDeathEnemy()
        {
            if( !player.isLive )
                return;
        
            enemyDeathCount++;

            if (enemyDeathCount >= aiPlayers.Count)
            {
                SetGameStep(GameStep.Success);
            }
        }

        public string ToPlayerInfoString()
        {
            return $"{aiPlayers.Count - enemyDeathCount} Players Left";
        }
    }
}
