using System.Collections;
using UnityEngine;
using MiniGames.Base;
using MiniGames.Main;
using System;
using MiniGames.Utils;

namespace MiniGames.Collapse
{
    public class CPGameManager : Singleton<CPGameManager>
    {
        // 这一轮是否开始
        public bool isStart = false;
        public float d = 1f;

        public float riseTime = 1f;
        // 是否正在上升
        public bool isRising = false;
        public bool isCollapse = false;
        public float delta = 0.01f;
        public int cnt = 0;

        public int betAll = 0;
        public int betCur = 0;

        public bool isEnough = false;
        public float enoughTime = 1.0f;

        public int gem { get { return GameManager.Instance.gameData.gemCount; } }

        protected override void Awake()
        {
            base.Awake();
        }

        public void Reset()
        {
            isStart = false;
            isRising = false;
            isCollapse = false;
            isEnough = false;
            betAll = 0;
            betCur = 0;
            riseTime = 1f;
            delta = 0.01f;
            cnt = 0;
            enoughTime = 0;
        }

        public void AddBet(int add)
        {
            betCur += add;
            if(betCur > gem)
            {
                betCur = gem;
            }
        }

        public void MutiBet(int time)
        {
            betCur *= time;
            if (betCur > gem)
            {
                betCur = gem;
            }
        }

        public void RemoveBet()
        {
            betCur = 0;
        }

        public void BetAll()
        {
            betCur = gem;
        }

        public void BP()
        {
            if (isEnough) return;

            // 这一轮还没开始
            if (!isStart)
            {
                CPAudioManager.Instance.PlayBgm();
                // 下注并开始
                betAll += betCur;
                GameManager.Instance.ModifyGem(-betCur);
                betCur = 0;
                isStart = true;
                isRising = true;
                d = UnityEngine.Random.Range(0.025f, 0.05f);
                StartCoroutine(Rise());
            }
            else
            {
                if (isRising)
                {
                    // 暂停
                    isRising = false;
                    StopAllCoroutines();
                }
                else
                {
                    // 下注并继续
                    betAll += betCur;
                    GameManager.Instance.ModifyGem(-betCur);
                    betCur = 0;
                    isRising = true;
                    d = UnityEngine.Random.Range(0.01f, 0.1f);
                    StartCoroutine(Rise());
                }
            }
        }

        IEnumerator Rise()
        {
            while (isRising)
            {
                riseTime += delta;
                Collapse();
                if (isCollapse)
                {
                    GameOver();
                    break;
                }
                yield return new WaitForSeconds(isEnough ? 0.01f : 0.1f);
            }
        }

        public void Enough()
        {
            if (!isStart || !isRising || isEnough) return;
            isEnough = true;
            enoughTime = riseTime;
            int win = Mathf.RoundToInt(betAll * enoughTime);
            GameManager.Instance.ModifyGem(win);
            CPAudioManager.Instance.PlayEnoughEffect();
            EventCenter.Broadcast(EventDefine.CP_UpdateAll);
        }

        private void Collapse()
        {
            float collapseRate = UnityEngine.Random.Range(0, d);
            if (RandomUtil.IsProbablityMet(collapseRate))
            {
                isCollapse = true;
            }
            else
            {
                cnt++;
                if(cnt % 10 == 0)
                {
                    delta = Mathf.Min(1.0f, delta * 2);
                }
            }
        }

        public void GameOver()
        {
            CPAudioManager.Instance.PlayCollapseEffect();
            CPAudioManager.Instance.StopBgm();
            float rise = riseTime;
            Reset();
            EventCenter.Broadcast(EventDefine.CP_UpdateLast, rise);
            EventCenter.Broadcast(EventDefine.CP_UpdateAll);
        }

    }
}


