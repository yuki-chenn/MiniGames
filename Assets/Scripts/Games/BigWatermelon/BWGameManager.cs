using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniGames.Base;
using MiniGames.Utils;
using MiniGames.Enum;
using MiniGames.Main;
using System;
using UnityEngine.EventSystems;
using UnityEditor;

namespace MiniGames.BigWatermelon
{
    public class BWGameManager : Singleton<BWGameManager>
    {
        public Transform readyPosition;
        public Transform fruitContanier;


        public bool isStart = false;

        private GameObject readyFruit;
        private bool isReady = true;

        public bool moveFruit = false;

        public int score;

        protected override void Awake()
        {
            base.Awake();
        }

        public void StartGame()
        {
            // 清空水果
            foreach (Transform child in fruitContanier)
            {
                Destroy(child.gameObject);
            }

            if (readyFruit != null)
            {
                Destroy(readyFruit);
                readyFruit = null;
            }

            isStart = true;
            isReady = true;
            score = 0;
            moveFruit = false;
        }

        private void Update()
        {
            if (!isStart) return;
            // 点击UI
            if(IsPointerOverUIObject(Input.mousePosition)) return;
            // 点击检测
            if (Input.GetMouseButtonDown(0))
            {
                OnPressDown();
            }

            if (Input.GetMouseButtonUp(0))
            {
                OnPressUp();
            }

            // 移动水果
            if (moveFruit && readyFruit != null && !readyFruit.GetComponent<Fruit>().released)
            {
                // 跟随手指或者鼠标位置移动
                var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // 根据宽度限制水果的移动范围
                float limit = readyFruit.GetComponent<Fruit>().limitX;
                mousePosition.x = Mathf.Clamp(mousePosition.x, -limit, limit);
                readyFruit.transform.position = new Vector3(mousePosition.x, readyFruit.transform.position.y, 0);
            }

            // readyFruit
            if (readyFruit == null && isReady)
            {
                isReady = false;
                int fruitIndex = RandomUtil.GetRandomInt(0, Constants.MAX_READY_FRUIT_INDEX);
                readyFruit = Instantiate(BWAssetManager.Instance.fruitPrefabs[fruitIndex], readyPosition.position, Quaternion.identity, fruitContanier);
            }
        }

        private bool IsPointerOverUIObject(Vector2 mousePos)
        {

            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = mousePos;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;

            //return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();     
        }

        private void OnPressDown()
        {
            if (readyFruit == null || readyFruit.GetComponent<Fruit>().released) return;
            //Debug.Log("手指按下");
            moveFruit = true;
            // 跟随手指或者鼠标位置移动
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // 根据宽度限制水果的移动范围
            float limit = readyFruit.GetComponent<Fruit>().limitX;
            mousePosition.x = Mathf.Clamp(mousePosition.x, -limit, limit);
            readyFruit.transform.position = new Vector3(mousePosition.x, readyFruit.transform.position.y, 0);
        }

        private void OnPressUp()
        {
            if (!moveFruit || readyFruit == null || readyFruit.GetComponent<Fruit>().released) return;
            BWAudioManager.Instance.PlayReleaseEffect();
            //Debug.Log("手指抬起");
            moveFruit = false;
            readyFruit.GetComponent<Fruit>().released = true;
            readyFruit.GetComponent<Rigidbody2D>().gravityScale = 1;
            // 过0.5s后设置为空
            Invoke("OnFruitRelease", 0.5f);
            RemoveBugFruit();
        }

        private void OnFruitRelease()
        {
            readyFruit = null;
            isReady = true;
        }

        public void MergeFruit(GameObject fruitA, GameObject fruitB)
        {
            int newFruitType = fruitA.GetComponent<Fruit>().fruitType + 1;
            Vector2 generatePos = (fruitA.transform.position + fruitB.transform.position) / 2;
            Destroy(fruitA);
            Destroy(fruitB);
            if(newFruitType >= Constants.FRUIT_COUNT)
            {
                BWAudioManager.Instance.PlayBonusEffect();
                // 奖励分数
                score += Constants.BONUS_SCORE;
            }
            else
            {
                if (newFruitType == Constants.FRUIT_COUNT - 1) BWAudioManager.Instance.PlayWatermelonEffect();
                else BWAudioManager.Instance.PlayMergeEffect();
                var newFruit = Instantiate(BWAssetManager.Instance.fruitPrefabs[newFruitType], generatePos, Quaternion.identity, fruitContanier);
                newFruit.GetComponent<Fruit>().released = true;
                newFruit.GetComponent<Rigidbody2D>().gravityScale = 1;
                newFruit.GetComponent<Fruit>().SetScale();
            }
            AddScore(newFruitType - 1);
        }

        public void AddScore(int fruitId)
        {
            score += Constants.FRUIT_SCORE[fruitId] * 2;
            EventCenter.Broadcast(EventDefine.BW_RefreshScore);
        }

        private void RemoveBugFruit()
        {
            foreach (Transform child in fruitContanier)
            {
                if(child.gameObject == readyFruit)
                {
                    continue;
                }
                // 漂浮的bug水果
                var fruit = child.GetComponent<Fruit>();
                if (fruit == null || !fruit.released)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        public void GameOver()
        {
            Debug.Log("游戏结束");
            // 获得钻石
            if(GameManager.Instance != null) GameManager.Instance.ModifyGem(score / 100);
            BWAudioManager.Instance.PlayLoseEffect();
            UpdateBestScore();
            isStart = false;
            Destroy(readyFruit);
            EventCenter.Broadcast(EventDefine.BW_ShowGameoverPanel);
        }

        public void UpdateBestScore()
        {
            if (GameManager.Instance != null)
            {
                if (score > GameManager.Instance.gameData.bwGameData.bestScore)
                {
                    GameManager.Instance.gameData.bwGameData.bestScore = score;
                }
                GameManager.Instance.SaveGameData();
            }
        }

    }
}
