
using System.Collections.Generic;
using UnityEngine;
using MiniGames.Base;
using MiniGames.Utils;
using System;
using MiniGames.Main;

namespace MiniGames.DoodleJump
{
    public class DJGameManager : Singleton<DJGameManager>
    {
        public bool isStart = false;

        public float leftBorder = 0f;
        public float rightBorder = 0f;
        public float spawnHeight = 0f;

        public Vector2 spawnRange = Vector2.zero;

        public float jumpHeight = 0f;
        public float currentHeight = 0f;
        public float[] rate = { 100.0f, 0f, 0f, 0f };

        List<GameObject> platforms = new List<GameObject>();
        List<GameObject> brokenPlatforms = new List<GameObject>();
        List<GameObject> springs = new List<GameObject>();
        List<GameObject> rockets = new List<GameObject>();
        Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

        public GameObject player;

        public int score = 0;

        protected override void Awake()
        {
            base.Awake();

            leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
            rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
            spawnHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;

            InitObjectPool();
        }

        private void InitObjectPool()
        {
            Transform pool = GameObject.Find("ObjectPool").transform;
            foreach (Transform child in pool)
            {
                ObjectPool op = child.GetComponent<ObjectPool>();
                pools.Add(child.name, op);
            }
        }

        public void StartGame()
        {
            isStart = true;
            currentHeight = -8.5f;
            score = 0;
            jumpHeight = 0f;
            player.transform.position = Constants.PLAYER_INIT_POS;
            player.GetComponent<Rigidbody2D>().gravityScale = 4;
            // 初始化生成的范围
            spawnRange = new Vector2(Constants.MIN_SPAWN_HEIGHT, Math.Min(Constants.MIN_SPAWN_HEIGHT + 0.5f, Constants.MAX_SPAWN_HEIGHT));
            // 生成第一个平台
            var go = SpawnPlatform(PlatformType.NomalPlatform, Constants.FIRST_PLATFORM_POS);
            platforms.Add(go);
        }

        public void PauseGame()
        {
            if (!isStart) return;
            isStart = false;
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            if (isStart) return;
            isStart = true;
            Time.timeScale = 1;
        }

        public void Reset()
        {
            Camera.main.transform.position = new Vector3(0, 0, -10);
            Time.timeScale = 1;
            isStart = false;
            currentHeight = 0f;
            jumpHeight = 0f;
            spawnRange = Vector2.zero;
            rate = new float[] { 100.0f, 0f, 0f, 0f };
            // 清空平台
            foreach (var platform in platforms)
            {
                pools[platform.name].ReturnObject(platform);
            }
            platforms.Clear();
            foreach (var platform in brokenPlatforms)
            {
                pools[platform.name].ReturnObject(platform);
            }
            brokenPlatforms.Clear();
            // 重置速度
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.GetComponent<Rigidbody2D>().angularVelocity = 0;
            player.GetComponent<Rigidbody2D>().gravityScale = 0;
            // 将玩家放到初始位置
            player.transform.position = Constants.PLAYER_INIT_POS;
                
            
        }

        private void Update()
        {
            if (!isStart) return;
            int stop = 0;
            // 获取platforms的最后一个元素
            GameObject platform = platforms.Count > 0 ? platforms[platforms.Count - 1] : null;
            // 如果platform的最后一个高度小于相机y+spawnHeight,生成平台
            while (platform == null || platform.transform.position.y < currentHeight + 1.5f * spawnHeight)
            {
                if (stop > 1000) break;

                int platformIndex = RandomUtil.GetIndexByProbablity(rate);
                var go = SpawnPlatform((PlatformType)platformIndex,
                    new Vector2(UnityEngine.Random.Range(leftBorder + 1.0f, rightBorder - 1.0f),
                    platform.transform.position.y + UnityEngine.Random.Range(spawnRange.x, spawnRange.y)));
                platforms.Add(go);
                platform = platforms.Count > 0 ? platforms[platforms.Count - 1] : null;
                stop++;
                // 有一定概率生成破碎平台
                if (RandomUtil.IsProbablityMet(Constants.SPAWN_BROKENPLATFORM_RATE))
                {
                    var bgo = SpawnPlatform(PlatformType.BrokenPlatform,
                        new Vector2(UnityEngine.Random.Range(leftBorder + 1.0f, rightBorder - 1.0f),
                        platform.transform.position.y + UnityEngine.Random.Range(Constants.MIN_SPAWN_HEIGHT, Constants.MAX_SPAWN_HEIGHT)));
                    brokenPlatforms.Add(bgo);
                }

                // 如果是normal或move平台，有一定概率生成道具
                if(jumpHeight >= 10.0f &&( platformIndex == 0 || platformIndex == 1 ))
                {
                    if (RandomUtil.IsProbablityMet(Constants.SPAWN_SPRING_RATE))
                    {
                        var item = SpawnItem(go,"Spring");
                        springs.Add(item);
                    }
                    else if(RandomUtil.IsProbablityMet(Constants.SPAWN_ROCKET_RATE))
                    {
                        var item = SpawnItem(go, "Rocket");
                        rockets.Add(item);
                    }
                }

            }

            // 移除超出屏幕的平台
            float removeHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y - 0.5f;

            if (platforms.Count > 0 && platforms[0].transform.position.y < removeHeight)
            {
                pools[platforms[0].name].ReturnObject(platforms[0]);
                platforms.RemoveAt(0);
            }

            if (brokenPlatforms.Count > 0 && brokenPlatforms[0].transform.position.y < removeHeight)
            {
                pools[brokenPlatforms[0].name].ReturnObject(brokenPlatforms[0]);
                brokenPlatforms.RemoveAt(0);
            }

            if (springs.Count > 0 && springs[0].transform.position.y < removeHeight)
            {
                pools[springs[0].name].ReturnObject(springs[0]);
                springs.RemoveAt(0);
            }

            if (rockets.Count > 0 && rockets[0].transform.position.y < removeHeight)
            {
                pools[rockets[0].name].ReturnObject(rockets[0]);
                rockets.RemoveAt(0);
            }

            CheckGameOver();
        }

        private GameObject SpawnPlatform(PlatformType type, Vector2 pos)
        {
            var pool = pools[type.ToString()];
            GameObject go = pool.GetObject();
            go.transform.position = pos;
            go.name = type.ToString();

            switch (type)
            {
                case PlatformType.NomalPlatform:
                    break;
                case PlatformType.MovePlatform:
                    break;
                case PlatformType.OncePlatform:
                    go.GetComponent<OncePlatform>().Reset();
                    break;
                case PlatformType.BoomPlatform:
                    go.GetComponent<BoomPlatform>().Reset();
                    break;
                case PlatformType.BrokenPlatform:
                    go.GetComponent<BrokenPlatform>().Reset();
                    break;
                default:
                    break;
            }
            return go;
        }

        private GameObject SpawnItem(GameObject platform,string itemName)
        {
            var go = pools[itemName].GetObject();
            go.name = itemName;
            go.transform.parent = platform.transform;
            if(itemName == "Spring")
            {
                float xc = platform.transform.position.x;
                go.GetComponent<Spring>().Reset();
                go.transform.position = new Vector2(
                    UnityEngine.Random.Range(xc - 0.25f,xc + 0.4f), 
                    platform.transform.position.y + Constants.ITEM_Y_OFFSET);
            }
            else if (itemName == "Rocket")
            {
                go.transform.position = new Vector2(platform.transform.position.x, platform.transform.position.y + Constants.ITEM_Y_OFFSET);
            }
            return go;
        }


        public void UpdateDifficulty()
        {
            int t = (int)jumpHeight / 50;
            spawnRange = new Vector2(
                Math.Min(Constants.MIN_SPAWN_HEIGHT + 0.1f * t, Constants.MAX_SPAWN_HEIGHT - 1.0f),
                Math.Min(Constants.MIN_SPAWN_HEIGHT + 0.5f + 0.2f * t, Constants.MAX_SPAWN_HEIGHT));

            rate[0] = Math.Max(45, 100 - t * 5);
            rate[1] = Math.Min(5 * t, 35);
            //rate[2] = rate[3] = 0;
            rate[2] = Math.Min(3 * t, 15);
            rate[3] = Math.Min(2 * t, 5);

            // 特殊区段
            if (t > 5 && t % 5 == 1)
            {
                rate[0] = rate[1] = rate[3] = 0;
                rate[2] = 100;
            }
            if (t > 10 && t % 5 == 4)
            {
                rate[0] = rate[1] = rate[2] = 0;
                rate[3] = 100;
            }

        }

        public void UpdateHeight(float y)
        {
            jumpHeight += Math.Max(0, y - currentHeight);
            score += Math.Max(0, (int)(y - currentHeight) * 10);
            currentHeight = Math.Max(currentHeight, y);
            EventCenter.Broadcast(EventDefine.DJ_RefreshScore);
        }

        public void CheckGameOver()
        {
            if (player.transform.position.y < Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y - 0.5f)
            {
                GameOver();
            }

        }

        public void GameOver()
        {
            DJAudioManager.Instance.PlayLoseEffect();
            // 获得钻石
            if (GameManager.Instance != null) GameManager.Instance.ModifyGem(score / 10);
            UpdateBestScore();
            Reset();
            EventCenter.Broadcast(EventDefine.DJ_Gameover);
            
        }

        public void UpdateBestScore()
        {
            if (GameManager.Instance != null)
            {
                if (score > GameManager.Instance.gameData.djGameData.bestScore)
                {
                    GameManager.Instance.gameData.djGameData.bestScore = score;
                }
                GameManager.Instance.SaveGameData();
            }
        }
    }
}
