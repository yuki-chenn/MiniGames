using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniGames.Base;
using MiniGames.Utils;
using MiniGames.Enum;
using MiniGames.Main;

namespace MiniGames.GuessColor
{
    public class GCGameManager : Singleton<GCGameManager>
    {
        
        public GuessSetting setting;

        public GuessData guessData;

        protected override void Awake()
        {
            base.Awake();
            setting = new GuessSetting();
        }

        public void StartGame()
        {
            CreateGuessData();
        }

        public void CreateGuessData()
        {
            guessData = new GuessData(setting);
            guessData.InitData(setting.colorRepeat);
        }

        public void SelectColor(int colorIndex)
        {
            guessData.SelectColor(colorIndex);
        }

        public void ChangeCurrentGuessIndex(int index)
        {
            guessData.attemptData.currentGuessIndex = index;
        }

        public bool CheckSubmitAttempt()
        {
            return guessData.CanSubmitAttempt();
        }

        public int CheckGameEnd()
        {
            int result = guessData.attemptData.IsGameEnd();
            if(result == 1)
            {
                GameManager.Instance.ModifyGem(guessData.rewardGem);
            }
            return result;
        }

        public void SubmitAttempt()
        {
            guessData.SubmitAttempt();
        }

        public void GoNextAttempt()
        {
            guessData.GoNextAttempt();
        }




    }
}
