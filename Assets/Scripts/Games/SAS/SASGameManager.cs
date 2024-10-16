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

        // �Ѿ������ĸ���
        public int matchCount = 0;
        // ѡ����Ѷ�
        public int selectDiff = 0;

        // ����ʹ�ô���
        public int withdrawCount = 0;
        public int withdrawAdvCount = 0;
        public int shuffleCount = 0;
        public int shuffleAdvCount = 0;
        public int pop3Count = 0;
        public int Pop3AdvCount = 0;
        public int reviveAdvCount = 0;

        // �Ѿ�����Ĵ���
        public int reviveCount = 0;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            
        }

        /// <summary>
        /// ���block
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns>�����patternֵ</returns>
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
                Debug.Log(string.Format("����{0},{1},{2}", matchStartIndex, matchStartIndex + 1, matchStartIndex + 2));
            }
            else
            {
                Debug.Log(string.Format("�޷�����"));
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

            // ������ͼ����
            int index = Random.Range(0, Constants.MAP_DATA_NUMS[selectDiff]);
            string mapdataPath = string.Format("SAS/{0}/{1}.json", selectDiff, index);

            mapdata = IOUtil.LoadJsonFromSA<MapData>(mapdataPath);
            blockMapdata = new BlockMapData(mapdata, Constants.TYPE_COUNT[selectDiff]);
            EventCenter.Broadcast<BlockMapData>(EventDefine.SAS_InitBlockMap, blockMapdata);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>0��ʾû������1��ʾӮ�ˣ�-1��ʾ����</returns>
        public int CheckGameover()
        {
            // �ж�Ӯ��
            if(matchCount == mapdata.blockCount)
            {
                // ��ñ�ʯ��������
                int gem = Constants.REWARD_GEM[selectDiff];
                GameManager.Instance.ModifyGem(gem);

                return 1;
            }

            // �ж�����
            if(blockMapdata.slotPattern[Constants.MAX_SLOT_NUM-1] != -1)
            {
                return -1;
            }

            return 0;

        }

        // ����һ��
        public BlockData Withdraw()
        {
            var data = blockMapdata.Withdraw();
            if (data != null) withdrawCount--;
            return data;
        }

        // ����ϴ��
        public void ShuffleBlock()
        {
            blockMapdata.Shuffle();
            shuffleCount--;
        }

        // ����ǰ����
        public int Pop3(bool isRevive=false)
        {
            int f = blockMapdata.Pop3();
            if(f == 0 && !isRevive) pop3Count--;
            return f;
        }

    }
}
