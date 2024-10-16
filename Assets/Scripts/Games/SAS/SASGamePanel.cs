using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MiniGames.BigWatermelon;

namespace MiniGames.SAS
{
    public class SASGamePanel : MonoBehaviour
    {
        private CanvasGroup canvas;

        public float paddingTop = 250.0f;
        public float pileOffset = 10.0f;

        private Color greyIconColor = new Color(0.6f, 0.6f, 0.6f);

        private RectTransform mapContent;
        private Transform slotContent;
        private Slot slot;

        private Button btnBack;
        private Button btnWithdraw;
        private Button btnShuffle;
        private Button btnPop;

        private Button btnBgm;
        private Button btnEffect;

        private Text txtWithdraw;
        private Text txtShuffle;
        private Text txtPop;

        private void Awake()
        {
            EventCenter.AddListener<BlockMapData>(EventDefine.SAS_InitBlockMap, InitBlockMap);
            EventCenter.AddListener<BlockMapData>(EventDefine.SAS_RefreshOccusion, RefreshOccusion);
            EventCenter.AddListener<BlockMapData>(EventDefine.SAS_RefreshOffsetPosition, RefreshOffsetPosition);
            EventCenter.AddListener<BlockMapData>(EventDefine.SAS_RefreshAllBlocks, RefreshAllBlocks);
            EventCenter.AddListener<BlockMapData>(EventDefine.SAS_RefreshBlockMap, RefreshBlockMap);
            EventCenter.AddListener(EventDefine.SAS_RefreshSkillCount, RefreshSkillCount);
            EventCenter.AddListener(EventDefine.SAS_Revive, Revive);

            BindComponent();
            AddListener();
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (SASAudioManager.Instance != null) SASAudioManager.Instance.PlayBgm();
        }

        private void OnDisable()
        {
            if (SASAudioManager.Instance != null) SASAudioManager.Instance.StopBgm();
        }

        private void BindComponent()
        {
            canvas = GetComponent<CanvasGroup>();
            mapContent = transform.Find("Map") as RectTransform;
            slotContent = transform.Find("Slot");
            slot = slotContent.GetComponent<Slot>();
            btnBack = transform.Find("BtnBack").GetComponent<Button>();
            btnWithdraw = transform.Find("SkillBtns/BtnWithdraw").GetComponent<Button>();
            btnShuffle = transform.Find("SkillBtns/BtnShuffle").GetComponent<Button>();
            btnPop = transform.Find("SkillBtns/BtnPop").GetComponent<Button>();

            txtWithdraw = transform.Find("SkillBtns/BtnWithdraw/Count/Text").GetComponent<Text>();
            txtShuffle = transform.Find("SkillBtns/BtnShuffle/Count/Text").GetComponent<Text>();
            txtPop = transform.Find("SkillBtns/BtnPop/Count/Text").GetComponent<Text>();

            btnBgm = transform.Find("BtnBgm").GetComponent<Button>();
            btnEffect = transform.Find("BtnEffect").GetComponent<Button>();

        }

        private void AddListener()
        {
            btnBack.onClick.AddListener(() =>
            {
                SASAudioManager.Instance.PlayButtonEffect();
                EventCenter.Broadcast(EventDefine.SAS_ShowSettingPanel);
                Hide();
            });
            btnWithdraw.onClick.AddListener(OnWithdrawBtnClick);
            btnShuffle.onClick.AddListener(OnShuffleBtnClick);
            btnPop.onClick.AddListener(OnPopBtnClick);


            btnBgm.onClick.AddListener(() =>
            {
                SASAudioManager.Instance.PlayButtonEffect();
                SASAudioManager.Instance.ChangeBgmState();
                btnBgm.transform.GetComponent<Image>().sprite =
                    SASAudioManager.Instance.musicOn ?
                    MainAssetManager.Instance.bgmOnSprite :
                    MainAssetManager.Instance.bgmOffSprite;
            });

            btnEffect.onClick.AddListener(() =>
            {
                SASAudioManager.Instance.ChangeEffectState();
                btnEffect.transform.GetComponent<Image>().sprite =
                    SASAudioManager.Instance.effectOn ?
                    MainAssetManager.Instance.effectOnSprite :
                    MainAssetManager.Instance.effectOffSprite;
                SASAudioManager.Instance.PlayButtonEffect();
            });

        }

        private void InitBlockMap(BlockMapData data)
        {
            // ��Slot���
            slot.Clear();

            Show();

            RefreshSkillCount();

            RefreshBlockMap(data);
        }

        private void RefreshBlockMap(BlockMapData data)
        {
            // ȷ��0��0���λ��
            float width = 1080f;
            float paddingLR = (width - 50 * (data.colNum - 1)) / 2.0f;
            mapContent.anchoredPosition = new Vector2(paddingLR, -paddingTop);

            RefreshAllBlocks(data);

            Debug.Log("blockNum:" + mapContent.childCount);

            RefreshOffsetPosition(data);

            RefreshOccusion(data);
        }

        /// <summary>
        /// ����dataˢ�����е�blockλ�ú�ͼ��
        /// </summary>
        /// <param name="data"></param>
        private void RefreshAllBlocks(BlockMapData data)
        {
            // ����ͼ
            int bi = 0;
            for (int layer = 0; layer < data.layerNum; ++layer)
            {
                for (int i = 0; i < data.rowNum; ++i)
                {
                    for (int j = 0; j < data.colNum; ++j)
                    {

                        if (Math.Abs(data.graph[layer, i, j]) == 1)
                        {
                            RectTransform block = null;

                            if (bi < mapContent.childCount)
                            {
                                block = mapContent.GetChild(bi) as RectTransform;
                            }
                            else
                            {
                                block = Instantiate(SASAssetManager.Instance.blockPrefab, mapContent as Transform).transform as RectTransform;
                            }

                            bi++;
                            int l = layer, r = i, c = j;
                            var btnblock = block.GetComponent<Button>();
                            btnblock.enabled = true;
                            btnblock.onClick.RemoveAllListeners();
                            btnblock.onClick.AddListener(() => OnBlockClick(l, r, c));
                            //Debug.Log("patter:" + data.pattern[l, r, c]);
                            block.Find("Icon").GetComponent<Image>().sprite = SASAssetManager.Instance.patternSprites[data.pattern[l, r, c]];
                            block.name = string.Format(Constants.LRC_TPL, layer, i, j);
                            block.anchoredPosition = new Vector2(j * 50, -i * 50);
                        }

                    }
                }
            }

            // zone�����⴦��
            for(int i = 0; i < 3; ++i)
            {
                var list = data.zonePattern[i];
                for(int j = 0; j < list.Count; ++j)
                {
                    RectTransform block = null;

                    if (bi < mapContent.childCount)
                    {
                        block = mapContent.GetChild(bi) as RectTransform;
                    }
                    else
                    {
                        block = Instantiate(SASAssetManager.Instance.blockPrefab, mapContent as Transform).transform as RectTransform;
                    }

                    bi++;
                    int layer = j, row = -1, col = i;
                    var btnblock = block.GetComponent<Button>();
                    btnblock.enabled = true;
                    btnblock.onClick.RemoveAllListeners();
                    btnblock.onClick.AddListener(() => OnBlockClick(layer, row, col));
                    //Debug.Log("patter:" + data.pattern[l, r, c]);
                    block.Find("Icon").GetComponent<Image>().sprite = SASAssetManager.Instance.patternSprites[list[j]];
                    block.name = string.Format(Constants.LRC_TPL, layer, row, col);
                    block.anchoredPosition = new Vector2((SASGameManager.Instance.blockMapdata.colNum - 1) * 50 / 2.0f + (i - 1) * 100,
                        -(SASGameManager.Instance.blockMapdata.rowNum + 2) * 50);
                }
            }

        }

        /// <summary>
        /// ����dataˢ��������Ҫƫ�Ƶ�blockλ��
        /// </summary>
        /// <param name="data"></param>
        private void RefreshOffsetPosition(BlockMapData data)
        {
            // ƫ��block
            foreach (var entry in data.offsetBlock)
            {
                string blockName = entry.Key;
                int type = entry.Value;
                // �� '-' �ָ��ַ���
                string[] values = blockName.Split('_');
                int layer = int.Parse(values[0]);
                int row = int.Parse(values[1]);
                int col = int.Parse(values[2]);

                Vector2 offset = Vector2.zero;
                switch (type)
                {
                    case 1:
                        offset.y = pileOffset;
                        break;
                    case 2:
                        offset.y = -pileOffset;
                        break;
                    case 3:
                        offset.x = -pileOffset;
                        break;
                    case 4:
                        offset.x = pileOffset;
                        break;
                    default:
                        break;
                }

                for (int l = layer - 1; l >= 0; l--)
                {
                    string name = string.Format(Constants.LRC_TPL, l, row, col);
                    RectTransform block = mapContent.Find(name) as RectTransform;
                    if (block == null) continue;
                    block.anchoredPosition = new Vector2(col * 50, -row * 50) + offset * (layer - l);
                }

            }

            // zone���⴦��
            for (int i = 0; i < 3; ++i)
            {
                var list = data.zonePattern[i];
                Vector2 offset = new Vector2(0, pileOffset);
                for (int j = 0; j < list.Count; ++j)
                {
                    string name = string.Format(Constants.LRC_TPL, j, -1, i);
                    RectTransform block = mapContent.Find(name) as RectTransform;
                    if (block == null) continue;
                    block.anchoredPosition = new Vector2((SASGameManager.Instance.blockMapdata.colNum - 1) * 50 / 2.0f + (i - 1) * 100,
                        -(SASGameManager.Instance.blockMapdata.rowNum + 2) * 50) + offset * j;
                }
            }
        }

        /// <summary>
        ///  ����dataˢ�����и��²����ϵ
        /// </summary>
        /// <param name="data"></param>
        private void RefreshOccusion(BlockMapData data)
        {
            foreach (Transform block in mapContent)
            {

                string[] values = block.name.Split('_');
                int layer = int.Parse(values[0]);
                int row = int.Parse(values[1]);
                int col = int.Parse(values[2]);

                // zone ���⴦��
                if (row == -1)
                {
                    if (data.zonePattern[col].Count - 1 == layer)
                    {
                        block.GetComponent<Image>().sprite = SASAssetManager.Instance.blockBg;
                        block.GetComponent<Button>().enabled = true;
                        block.Find("Icon").GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        block.GetComponent<Image>().sprite = SASAssetManager.Instance.blockBgGrey;
                        block.GetComponent<Button>().enabled = false;
                        block.Find("Icon").GetComponent<Image>().color = greyIconColor;
                    }
                }
                else
                {
                    if (data.graph[layer, row, col] == 1)
                    {
                        block.GetComponent<Image>().sprite = SASAssetManager.Instance.blockBg;
                        block.GetComponent<Button>().enabled = true;
                        block.Find("Icon").GetComponent<Image>().color = Color.white;
                    }
                    else if (data.graph[layer, row, col] == -1)
                    {
                        block.GetComponent<Image>().sprite = SASAssetManager.Instance.blockBgGrey;
                        block.GetComponent<Button>().enabled = false;
                        block.Find("Icon").GetComponent<Image>().color = greyIconColor;
                    }
                    else
                    {
                        Debug.LogError(string.Format("({0},{1},{2}) block ���ݲ�ƥ��", layer, row, col));
                    }
                }
            }

        }

        private void RefreshSkillCount()
        {
            txtWithdraw.text = SASGameManager.Instance.withdrawCount == 0 ? "+" : SASGameManager.Instance.withdrawCount.ToString();
            txtShuffle.text = SASGameManager.Instance.shuffleCount == 0 ? "+" : SASGameManager.Instance.shuffleCount.ToString();
            txtPop.text = SASGameManager.Instance.pop3Count == 0 ? "+" : SASGameManager.Instance.pop3Count.ToString();
        }

        /// <summary>
        /// ����
        /// </summary>
        private void Revive()
        {
            SASGameManager.Instance.reviveCount++;
            SASGameManager.Instance.reviveAdvCount--;
            // չʾ��ǰҳ��
            Show();
            // �ó���3��
            SASGameManager.Instance.Pop3(true);
            slot.Pop3Block();
        }

        private void OnBlockClick(int layer, int row, int col)
        {
            // ����ɾ����ʱ�򲻸�����
            if (slot.insertLock || slot.skillLock) return;

            // ��֪���ƶ���ͼ��
            int pattern = SASGameManager.Instance.ClickBlock(layer, row, col);

            // �Ѿ�����
            if (pattern == -1) return;

            SASAudioManager.Instance.PlayClickEffect();

            // ����slot
            var block = mapContent.Find(string.Format(Constants.LRC_TPL, layer, row, col));
            slot.InsertBlock(block, pattern);

            RefreshOccusion(SASGameManager.Instance.blockMapdata);

            CheckGameOver();
        }

        private void OnWithdrawBtnClick()
        {
            // û�д���
            if (SASGameManager.Instance.withdrawCount <= 0)
            {
                EventCenter.Broadcast(EventDefine.SAS_ShowAddSkillPanel, 0);
                return;
            }

            if (slot.matchLock > 0 || slot.insertLock || slot.skillLock) return;

            BlockData data = SASGameManager.Instance.Withdraw();

            if (data == null) return;

            // slot �ƶ���ȥ
            slot.WithdrawBlock(data.blockId, data.type, data.row, data.col);

            RefreshOccusion(SASGameManager.Instance.blockMapdata);

            RefreshSkillCount();
        }

        private void OnShuffleBtnClick()
        {
            // û�д���
            if (SASGameManager.Instance.shuffleCount <= 0)
            {
                EventCenter.Broadcast(EventDefine.SAS_ShowAddSkillPanel, 1);
                return;
            }

            if (slot.matchLock > 0 || slot.insertLock || slot.skillLock) return;

            SASGameManager.Instance.ShuffleBlock();

            // ִ��ϴ�ƶ���
            slot.ShuffleBlock();

            RefreshSkillCount();
        }

        private void OnPopBtnClick()
        {
            // û�д���
            if (SASGameManager.Instance.pop3Count <= 0)
            {
                EventCenter.Broadcast(EventDefine.SAS_ShowAddSkillPanel, 2);
                return;
            }

            if (slot.matchLock > 0 || slot.insertLock || slot.skillLock) return;

            int f = SASGameManager.Instance.Pop3();

            if(f != 0) return;

            slot.Pop3Block();

            RefreshSkillCount();
        }

        private void CheckGameOver()
        {
            // �����Ϸ�Ƿ����
            int flag = SASGameManager.Instance.CheckGameover();

            if(flag == 0) return;

            if (Math.Abs(flag) == 1)
            {
                
                if(flag == -1 && 
                    SASGameManager.Instance.reviveAdvCount > 0)
                {
                    // ʧ�� �� �ܸ���
                    EventCenter.Broadcast(EventDefine.SAS_ShowRevivePanel, SASGameManager.Instance.reviveAdvCount);
                }
                else
                {
                    // ���ܸ�����
                    EventCenter.Broadcast(EventDefine.SAS_ShowGameoverPanel, flag);
                }
            }
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener<BlockMapData>(EventDefine.SAS_InitBlockMap, InitBlockMap);
            EventCenter.RemoveListener<BlockMapData>(EventDefine.SAS_RefreshOccusion, RefreshOccusion);
            EventCenter.RemoveListener<BlockMapData>(EventDefine.SAS_RefreshOffsetPosition, RefreshOffsetPosition);
            EventCenter.RemoveListener<BlockMapData>(EventDefine.SAS_RefreshAllBlocks, RefreshAllBlocks);
            EventCenter.RemoveListener<BlockMapData>(EventDefine.SAS_RefreshBlockMap, RefreshBlockMap);
            EventCenter.RemoveListener(EventDefine.SAS_Revive, Revive);
        }

        private void Hide()
        {
            canvas.DOFade(0, 0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });

        }

        private void Show()
        {
            gameObject.SetActive(true);
            canvas.DOFade(1, 0.5f);
        }


    }
}

