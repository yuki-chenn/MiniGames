using MiniGames.Utils;
using System;
using System.Collections.Generic;
using TMPro;

namespace MiniGames.GuessColor
{
    [Serializable]
    public class GuessSetting
    {
        public int colorNum;
        public int guessNum;
        public int attemptNum;
        public bool colorRepeat;

        public GuessSetting()
        {
            this.colorNum = Constants.MIN_COLOR_NUM;
            this.guessNum = Constants.MIN_GUESS_NUM;
            this.attemptNum = Constants.MAX_ATTEMPT_NUM;
            this.colorRepeat = false;
        }

    }

    [Serializable]
    public class GuessData
    {
        public GuessSetting setting;
        public AttemptData attemptData;
        public int[] ans;

        public int rewardGem
        {
            get
            {
                int attemptTime = setting.guessNum * 2 + 1 - setting.attemptNum;
                if (attemptTime < 1) attemptTime = 1;
                return 10 *
                    (setting.colorNum - 5) *
                    (setting.guessNum - 3) *
                    attemptTime *
                    (setting.colorRepeat ? 5 : 1);
            }
        }

        public GuessData(GuessSetting setting)
        {
            this.setting = setting;
            ans = new int[setting.guessNum];
            attemptData = new AttemptData(setting);
        }

        public void InitData(bool canRepeat)
        {
            var list = RandomUtil.GetRandomIndexList(setting.guessNum, 0, setting.colorNum, canRepeat);
            for (int i = 0; i < setting.guessNum; i++)
            {
                ans[i] = list[i];
            }
        }

        public void SelectColor(int colorIndex)
        {
            attemptData.attemps[attemptData.currentAttemptIndex, attemptData.currentGuessIndex] = colorIndex;
            int moveNext = attemptData.GetNextMoveIndex();
            attemptData.currentGuessIndex = moveNext;
        }

        public bool CanSubmitAttempt()
        {
            return attemptData.IsCurrentAllSelected();
        }

        public void SubmitAttempt()
        {
            int[] attempt = attemptData.GetCurrentAttempt();
            var res = CheckResult(attempt);
            attemptData.SetRes(res);
        }

        public void GoNextAttempt()
        {
            attemptData.currentAttemptIndex++;
            attemptData.currentGuessIndex = 0;
        }

        private int[] CheckResult(int[] attempt)
        {
            int n = setting.guessNum;
            int[] res = new int[n];
            

            // 判断ans和attempt的相同颜色的个数
            int sameColorNum = 0;
            bool[] vis = new bool[n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (!vis[j] && ans[i] == attempt[j])
                    {
                        vis[j] = true;
                        sameColorNum++;
                        break;
                    }
                }
            }

            // 判断ans和attempt的相同颜色相同位置的个数
            int sameColorAndPosNum = 0;
            for (int i = 0; i < n; i++)
            {
                if (ans[i] == attempt[i])
                {
                    sameColorAndPosNum++;
                }
            }

            // 计算结果
            for (int i = 0; i < n; i++)
            {
                if (i < sameColorNum) res[i]++;
                if (i < sameColorAndPosNum) res[i]++;
            }

            return res;
        }

    }

    [Serializable]
    public class AttemptData
    {
        public int guessNum;
        public int maxAttemptNum;
        // 尝试的颜色 [第几次尝试，第几个颜色]
        public int[,] attemps;
        // 结果 -1 表示还没提交，0表示全错，1表示有颜色对，2表示颜色和位置都对
        public int[,] res;
        public int currentAttemptIndex = 0;
        public int currentGuessIndex = 0;

        public AttemptData(GuessSetting setting)
        {
            guessNum = setting.guessNum;
            maxAttemptNum = setting.attemptNum;

            attemps = new int[setting.attemptNum, setting.guessNum];
            // 全部初始化为-1
            for (int i = 0; i < setting.attemptNum; i++) for (int j = 0; j < setting.guessNum; j++) attemps[i, j] = -1;
            
            res = new int[setting.attemptNum, setting.guessNum];
            for (int i = 0; i < setting.attemptNum; i++) for (int j = 0; j < setting.guessNum; j++) res[i, j] = -1;

            currentAttemptIndex = 0;
            currentGuessIndex = 0;
        }

        public int GetNextMoveIndex()
        {
            for(int i = 1;i <= guessNum; ++i)
            {
                if (attemps[currentAttemptIndex, (currentGuessIndex + i) % guessNum] == -1) return (currentGuessIndex + i) % guessNum;
            }
            return currentGuessIndex;
        }

        public int[] GetCurrentAttempt()
        {
            int[] ret = new int[guessNum];
            for (int i = 0; i < guessNum; i++)
            {
                ret[i] = attemps[currentAttemptIndex, i];
            }
            return ret;
        }

        public void SetRes(int[] res)
        {
            for (int i = 0; i < res.Length; i++)
            {
                this.res[currentAttemptIndex, i] = res[i];
            }
        }

        public bool IsCurrentAllSelected()
        {
            for (int i = 0; i < guessNum; i++)
            {
                if (attemps[currentAttemptIndex, i] == -1) return false;
            }
            return true;
        }

        // 0表示未结束，1表示胜利，-1表示失败
        public int IsGameEnd()
        {
            bool winFlag = true;
            for(int i=0;i<guessNum; i++)
            {
                if(res[currentAttemptIndex, i] != 2)
                {
                    winFlag = false;
                    break;
                }
            }

            if (winFlag) return 1;

            if(currentAttemptIndex >= maxAttemptNum - 1) return -1;
            
            return 0;
        }
    }
}
