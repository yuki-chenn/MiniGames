using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using MiniGames.SAS;
using MiniGames.BigWatermelon;


namespace MiniGames.GuessColor
{
    public class GCGamePanel : MonoBehaviour
    {
        private CanvasGroup canvas;
        private CanvasGroup ansMask;

        private Transform colorSelectContent;
        private Transform answerContent;
        private Transform attemptContent;
        private RectTransform attemptRect;

        private Button btnBack;
        private Button btnBgm;
        private Button btnEffect;


        private GuessSetting setting { get { return GCGameManager.Instance.setting; } }
        private GuessData gamedata { get { return GCGameManager.Instance.guessData; } }
        private AttemptData attemptData { get { return gamedata.attemptData; } }

        private void Awake()
        {
            EventCenter.AddListener(EventDefine.GC_StartGame, StartGame);

            BindComponent();
            AddListener();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener(EventDefine.GC_StartGame, StartGame);
        }

        private void OnEnable()
        {
            if (GCAudioManager.Instance != null) GCAudioManager.Instance.PlayBgm();
        }

        private void OnDisable()
        {
            if (GCAudioManager.Instance != null) GCAudioManager.Instance.StopBgm();
        }

        private void Start()
        {
            gameObject.SetActive(false);
            ansMask.gameObject.SetActive(true);
            ansMask.alpha = 1;
        }
        private void BindComponent()
        {
            canvas = GetComponent<CanvasGroup>();
            ansMask = transform.Find("Answer/Mask").GetComponent<CanvasGroup>();
            colorSelectContent = transform.Find("ColorSelect");
            attemptRect = transform.Find("AttemptScrollRect") as RectTransform;
            attemptContent = attemptRect.transform.Find("Viewport/Content");
            answerContent = transform.Find("Answer/Ans");
            btnBack = transform.Find("BtnBack").GetComponent<Button>();
            btnBgm = transform.Find("BtnBgm").GetComponent<Button>();
            btnEffect = transform.Find("BtnEffect").GetComponent<Button>();
        }
        private void AddListener()
        {
            btnBack.onClick.AddListener(() =>
            {
                GCAudioManager.Instance.PlaySelectEffect();
                EventCenter.Broadcast(EventDefine.GC_ShowSettingPanel);
                Hide();
            });
            btnBgm.onClick.AddListener(() =>
            {
                GCAudioManager.Instance.PlaySelectEffect();
                GCAudioManager.Instance.ChangeBgmState();
                btnBgm.transform.GetComponent<Image>().sprite =
                    GCAudioManager.Instance.musicOn ?
                    MainAssetManager.Instance.bgmOnSprite :
                    MainAssetManager.Instance.bgmOffSprite;
            });

            btnEffect.onClick.AddListener(() =>
            {
                GCAudioManager.Instance.ChangeEffectState();
                btnEffect.transform.GetComponent<Image>().sprite =
                    GCAudioManager.Instance.effectOn ?
                    MainAssetManager.Instance.effectOnSprite :
                    MainAssetManager.Instance.effectOffSprite;
                GCAudioManager.Instance.PlaySelectEffect();
            });

        }

        private void StartGame()
        {
            RefreshSelectColorBtn();
            RefreshAttempts();
            RefreshAns();
            Show();
        }

        /// <summary>
        /// ˢ��ѡ����ɫ�İ�ť
        /// </summary>
        private void RefreshSelectColorBtn()
        {
            // �������6������������ʾ
            colorSelectContent.GetComponent<GridLayoutGroup>().constraintCount =
                setting.colorNum > 6 ? 2 : 1;

            for (int i = 0; i < setting.colorNum; i++)
            {
                GameObject go = null;
                if(i < colorSelectContent.childCount)
                {
                    go = colorSelectContent.GetChild(i).gameObject;
                }
                else
                {
                    go = Instantiate(GCAssetManager.Instance.colorSelectItemPrefab, colorSelectContent);
                }

                go.GetComponent<Image>().color = Constants.COLORS[i];
                go.name = i.ToString();
                // ����¼�
                int color = i;
                var btn = go.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnSelectColor(color));

                go.SetActive(true);
            }

            // ���ض����
            for (int i = setting.colorNum; i < colorSelectContent.childCount; i++)
            {
                colorSelectContent.GetChild(i).gameObject.SetActive(false);
            }

        }

        private void RefreshAns()
        {
            ansMask.gameObject.SetActive(true);
            ansMask.alpha = 1;

            for (int i = 0; i < setting.guessNum; i++)
            {
                GameObject go = null;
                if (i < answerContent.childCount)
                {
                    go = answerContent.GetChild(i).gameObject;
                }
                else
                {
                    go = Instantiate(GCAssetManager.Instance.ansItemPrefab, answerContent);
                }

                go.GetComponent<Image>().color = Constants.COLORS[gamedata.ans[i]];
                go.name = i.ToString();
                go.SetActive(true);
            }

            // ���ض����
            for (int i = setting.guessNum; i < answerContent.childCount; i++)
            {
                answerContent.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void RefreshAttempts()
        {
            // �������6���߶�1100����1300
            attemptRect.sizeDelta = new Vector2(attemptRect.sizeDelta.x, setting.colorNum > 6 ? 1100 : 1300);
            // ����λ��
            attemptRect.anchoredPosition = new Vector2(540, setting.colorNum > 6 ? -1220 : -1160);

            for (int i= 0; i < setting.attemptNum; i++) 
            {
                GameObject go = null;
                if (i < attemptContent.childCount)
                {
                    go = attemptContent.GetChild(i).gameObject;
                }
                else
                {
                    go = Instantiate(GCAssetManager.Instance.attemptItemPrefab, attemptContent);
                }
                RefreshAttemptItem(i, go);
                go.SetActive(true);
            }

            // ���ض����
            for (int i = setting.attemptNum; i < attemptContent.childCount; i++)
            {
                attemptContent.GetChild(i).gameObject.SetActive(false);
            }

        }

        // ˢ�³��Ե�һ��
        private void RefreshAttemptItem(int attemptIndex, GameObject item)
        {
            item.name = "attempt-" + attemptIndex;
            
            RefreshAttemptColor(attemptIndex, item);

            RefreshAttemptResult(attemptIndex, item);
        }

        // ˢ�³��Ե���ɫ
        private void RefreshAttemptColor(int attemptIndex, GameObject item)
        {
            var colorContent = item.transform.Find("Color");

            for (int i = 0; i < setting.guessNum; i++)
            {
                GameObject go = null;
                if (i < colorContent.childCount)
                {
                    go = colorContent.GetChild(i).gameObject;
                }
                else
                {
                    go = Instantiate(GCAssetManager.Instance.colorItemPrefab, colorContent);
                }
                go.name = string.Format("{0}_{1}_{2}", attemptIndex, i, attemptData.attemps[attemptIndex, i]);

                // ѡ�б�־
                go.transform.Find("Selected").gameObject.SetActive(
                    attemptData.currentAttemptIndex == attemptIndex &&
                    attemptData.currentGuessIndex == i);

                // ͼ��
                var goImage = go.GetComponent<Image>();
                goImage.sprite = attemptData.attemps[attemptIndex, i] == -1 ?
                    GCAssetManager.Instance.attempColorItemUnselected :
                    GCAssetManager.Instance.attempColorItemselected;

                // ��ɫ
                if (attemptData.attemps[attemptIndex, i] != -1)
                {
                    goImage.color = Constants.COLORS[attemptData.attemps[attemptIndex, i]];
                }
                else
                {
                    goImage.color = Color.white;
                }

                
                // ����
                var btn = go.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                if(attemptIndex != attemptData.currentAttemptIndex)
                {
                    btn.enabled = false;
                }
                else
                {
                    btn.enabled = true;
                    int index = i;
                    btn.onClick.AddListener(() => 
                    {
                        GCAudioManager.Instance.PlaySwitchEffect();
                        GCGameManager.Instance.ChangeCurrentGuessIndex(index);
                        RefreshAttemptColor(attemptIndex, item);
                    });
                }

                go.SetActive(true);
            }

            // ���ض����
            for (int i = setting.guessNum; i < colorContent.childCount; i++)
            {
                colorContent.GetChild(i).gameObject.SetActive(false);
            }
        }

        // ˢ�³��ԵĽ��
        private void RefreshAttemptResult(int attemptIndex, GameObject item)
        {
            var resultContent = item.transform.Find("Result");
            var confirmBtn = item.transform.Find("BtnConfirm").GetComponent<Button>();

            resultContent.gameObject.SetActive(attemptData.currentAttemptIndex > attemptIndex);
            confirmBtn.gameObject.SetActive(attemptData.currentAttemptIndex == attemptIndex);

            confirmBtn.onClick.RemoveAllListeners();
            if(attemptIndex == attemptData.currentAttemptIndex)
            {
                confirmBtn.onClick.AddListener(OnSubmitBtnClick);
            }
            

            // ����ǵ�ǰ�У���ʾ�ύ��������ǰ�е�ʲô������ʾ���Ѿ��ύ����ʾ���
            for (int i = 0; i < setting.guessNum; i++)
            {
                GameObject go = null;
                if (i < resultContent.childCount)
                {
                    go = resultContent.GetChild(i).gameObject;
                }
                else
                {
                    go = Instantiate(GCAssetManager.Instance.resultItemPrefab, resultContent);
                }

                go.name = string.Format("{0}_{1}_{2}", attemptIndex, i, attemptData.res[attemptIndex, i]);

                // ��ʾ���

                if (attemptData.res[attemptIndex, i] != -1)
                {
                    go.GetComponent<Image>().color = Constants.RES_COLORS[attemptData.res[attemptIndex, i]];
                }
            }

            // ���ض����
            for (int i = setting.guessNum; i < resultContent.childCount; i++)
            {
                resultContent.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void OnSubmitBtnClick()
        {
            if (!GCGameManager.Instance.CheckSubmitAttempt())
            {
                // û����
                return;
            }
            GCAudioManager.Instance.PlaySubmitEffect();
            Debug.Log("submit");
            GCGameManager.Instance.SubmitAttempt();
            int gameover = GCGameManager.Instance.CheckGameEnd();
            int attemptIndex = attemptData.currentAttemptIndex;
            StartCoroutine(Coroutine_RefreshAttemptResult(attemptIndex,
                attemptContent.GetChild(attemptIndex).gameObject,gameover));

            GCGameManager.Instance.GoNextAttempt();
            RefreshAttemptColor(attemptData.currentAttemptIndex - 1,
                attemptContent.GetChild(attemptData.currentAttemptIndex - 1).gameObject);
            
            if(gameover == 0)
            {
                RefreshAttemptItem(attemptData.currentAttemptIndex,
                    attemptContent.GetChild(attemptData.currentAttemptIndex).gameObject);
            }
        }

        IEnumerator Coroutine_RefreshAttemptResult(int attemptIndex, GameObject item,int gameover)
        {
            var resultContent = item.transform.Find("Result");
            var confirmBtn = item.transform.Find("BtnConfirm").GetComponent<Button>();

            resultContent.gameObject.SetActive(true);
            confirmBtn.gameObject.SetActive(false);

            for(int i = 0; i < resultContent.childCount; ++i)
            {
                if(i >= setting.guessNum) resultContent.GetChild(i).gameObject.SetActive(false);
                else
                {
                    resultContent.GetChild(i).gameObject.SetActive(true);
                    resultContent.GetChild(i).GetComponent<Image>().enabled = false;
                }
            }

            for (int i = 0; i < setting.guessNum; i++)
            {
                Transform go = resultContent.GetChild(i);
                // ��ʾ���
                if (attemptData.res[attemptIndex, i] != -1)
                {
                    go.GetComponent<Image>().color = Constants.RES_COLORS[attemptData.res[attemptIndex, i]];
                }
                GCAudioManager.Instance.PlayShowResEffect();
                go.gameObject.SetActive(true);
                go.GetComponent<Image>().enabled = true;
                yield return new WaitForSeconds(2.0f / setting.guessNum);
            }

            RefreshAttemptItem(attemptIndex,item);

            // �����Ϸ�Ƿ����
            if (gameover != 0)
            {
                if (gameover == 1) GCAudioManager.Instance.PlayWinEffect();
                else GCAudioManager.Instance.PlayLoseEffect();
                ansMask.DOFade(0, 1.0f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    ansMask.gameObject.SetActive(false);
                    EventCenter.Broadcast(EventDefine.GC_ShowGameoverPanel, gameover);
                });
            }

        }

        private void OnSelectColor(int colorIndex)
        {
            GCAudioManager.Instance.PlaySelectEffect();
            Debug.Log("ѡ������ɫ" + colorIndex);
            GCGameManager.Instance.SelectColor(colorIndex);
            RefreshAttemptColor(attemptData.currentAttemptIndex, attemptContent.GetChild(attemptData.currentAttemptIndex).gameObject);
        }

        private void Hide()
        {
            canvas.DOFade(0, 0.5f).OnComplete(()=>
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

