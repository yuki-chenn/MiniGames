using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniGames.Base;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MiniGames.Main
{
    public class GameManager : PersistentSingleton<GameManager>
    {

        public GameData gameData;



        protected override void Awake()
        {
            base.Awake();
#if UNITY_ANDROID && !UNITY_EDITOR
        Application.targetFrameRate = 60;
#else
            Application.targetFrameRate = -1;
#endif

            LoadGameData();

        }

        void Start()
        {

        }

        public void ModifyGem(int value)
        {
            gameData.gemCount += value;
            SaveGameData();
        }



        #region 记录游戏数据
        public void SaveGameData()
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream file = File.Create(Application.persistentDataPath + "/GameData.dat"))
                {
                    bf.Serialize(file, gameData);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("SaveGameData Error: " + e.Message);
            }
        }

        public void LoadGameData()
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream file = File.Open(Application.persistentDataPath + "/GameData.dat", FileMode.Open))
                {
                    gameData = (GameData)bf.Deserialize(file);
                }
            }
            catch (System.Exception e)
            {
                gameData = new GameData();
                Debug.LogError("LoadGameData Error: " + e.Message);
            }
        }

        #endregion
    }
}

