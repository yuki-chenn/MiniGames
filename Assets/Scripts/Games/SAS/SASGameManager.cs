using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniGames.Base;
using MiniGames.Utils;
using MiniGames.Enum;
using MiniGames.Main;

namespace MiniGames.SAS
{
    public class SASGameManager : Singleton<SASGameManager>
    {
        public MapData mapdata;
        public BlockMapData blockMapdata;

        // 已经消除的个数
        public int matchCount = 0;
        // 选择的难度
        public int selectDiff = 0;

        // 道具使用次数
        public int withdrawCount = 0;
        public int withdrawAdvCount = 0;
        public int shuffleCount = 0;
        public int shuffleAdvCount = 0;
        public int pop3Count = 0;
        public int Pop3AdvCount = 0;
        public int reviveAdvCount = 0;

        // 已经复活的次数
        public int reviveCount = 0;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            
        }

        /// <summary>
        /// 点击block
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns>插入的pattern值</returns>
        public int ClickBlock(int layer,int row,int col)
        {
            Debug.Log("click:" + layer + "," + row + "," + col);
            int p = blockMapdata.AddToSlot(layer, row, col);
            if(p != -1) CheckMatch3();
            return p;
        }

        private int CheckMatch3()
        {
            int matchStartIndex = blockMapdata.Match();
            if(matchStartIndex != -1)
            {
                matchCount += 3;
                Debug.Log(string.Format("消除{0},{1},{2}", matchStartIndex, matchStartIndex + 1, matchStartIndex + 2));
            }
            else
            {
                Debug.Log(string.Format("无法消除"));
            }
            return matchStartIndex;
        }

        public void ChangeDifficulty(int diff)
        {
            selectDiff = diff;
            Debug.Log("select:" + ((Difficulty)selectDiff).ToString());
        }

        public void StartGame()
        {
            matchCount = 0;
            withdrawCount = Constants.FREE_WITHDRAW_COUNT[selectDiff];
            withdrawAdvCount = Constants.MAX_WITHDRAW_COUNT[selectDiff] - withdrawCount;

            shuffleCount = Constants.FREE_SHUFFLE_COUNT[selectDiff];
            shuffleAdvCount = Constants.MAX_SHUFFLE_COUNT[selectDiff] - shuffleCount;

            pop3Count = Constants.FREE_POP3_COUNT[selectDiff];
            Pop3AdvCount = Constants.MAX_POP3_COUNT[selectDiff] - pop3Count;

            reviveAdvCount = Constants.MAX_REVIVE_COUNT[selectDiff];
            reviveCount = 0;

            // 创建地图数据
            int index = Random.Range(0, Constants.MAP_DATA_NUMS[selectDiff]);
            string mapdataPath = string.Format("SAS/{0}/{1}.json", selectDiff, index);

            mapdata = IOUtil.LoadJsonFromSA<MapData>(mapdataPath);
            blockMapdata = new BlockMapData(mapdata, Constants.TYPE_COUNT[selectDiff]);
            EventCenter.Broadcast<BlockMapData>(EventDefine.SAS_InitBlockMap, blockMapdata);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>0表示没结束，1表示赢了，-1表示输了</returns>
        public int CheckGameover()
        {
            // 判断赢了
            if(matchCount == mapdata.blockCount)
            {
                // 获得宝石，并保存
                int gem = Constants.REWARD_GEM[selectDiff];
                GameManager.Instance.ModifyGem(gem);

                return 1;
            }

            // 判断输了
            if(blockMapdata.slotPattern[Constants.MAX_SLOT_NUM-1] != -1)
            {
                return -1;
            }

            return 0;

        }

        // 撤回一个
        public BlockData Withdraw()
        {
            var data = blockMapdata.Withdraw();
            if (data != null) withdrawCount--;
            return data;
        }

        // 重新洗牌
        public void ShuffleBlock()
        {
            blockMapdata.Shuffle();
            shuffleCount--;
        }

        // 弹出前三个
        public int Pop3(bool isRevive=false)
        {
            int f = blockMapdata.Pop3();
            if(f == 0 && !isRevive) pop3Count--;
            return f;
        }

    }
}
