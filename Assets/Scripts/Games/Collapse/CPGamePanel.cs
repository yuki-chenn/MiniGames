using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using MiniGames.SAS;
using MiniGames.Main;
using UnityEngine.SocialPlatforms.Impl;
using MiniGames.GuessColor;
using UnityEngine.SceneManagement;


namespace MiniGames.Collapse
{
    public class CPGamePanel : MonoBehaviour
    {
        private CanvasGroup canvas;

        private Button btnBack;
        private Button btnBgm;
        private Button btnEffect;

        private Text txtGemCount;
        private Text txtTimes;
        private Text txtLastTimes;
        private Text txtBetAll;
        private Text txtBetCur;
        private Text txtBPBtn;
        private Text txtEnoughHint;

        private Button[] btnAdds = new Button[3];
        private Button[] btnMuls = new Button[3];
        private Button btn0;
        private Button btnAll;
        private Button btnBP;
        private Button btnEnough;


        private void Awake()
        {
            EventCenter.AddListener(EventDefine.CP_UpdateAll, UpdateAll);
            EventCenter.AddListener<float>(EventDefine.CP_UpdateLast, UpdateLastTime);

            BindComponent();
            AddListener();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener(EventDefine.CP_UpdateAll, UpdateAll);
            EventCenter.RemoveListener<float>(EventDefine.CP_UpdateLast, UpdateLastTime);
        }

        private void OnEnable()
        {
            //if(BWAudioManager.Instance != null) BWAudioManager.Instance.PlayBgm();
        }

        private void OnDisable()
        {
            //if (BWAudioManager.Instance != null) BWAudioManager.Instance.StopBgm();
        }

        private void Start()
        {
            gameObject.SetActive(true);
            UpdateAll();
        }

        private void Update()
        {
            if(CPGameManager.Instance.isRising)
            {
                UpdateRiseTimes();
                if(!CPGameManager.Instance.isEnough) UpdateEnoughHint();
            }
        }

        private void BindComponent()
        {
            canvas = GetComponent<CanvasGroup>();
            btnBack = transform.Find("BtnBack").GetComponent<Button>();
            btnBgm = transform.Find("BtnBgm").GetComponent<Button>();
            btnEffect = transform.Find("BtnEffect").GetComponent<Button>();
            
            txtGemCount = transform.Find("Gem/Count").GetComponent<Text>();
            txtTimes = transform.Find("ShowFrame/TimesTxt").GetComponent<Text>();
            txtLastTimes = transform.Find("ShowFrame/LastTimesTxt").GetComponent<Text>();
            txtBetAll = transform.Find("BetFrame/BetTxt/All").GetComponent<Text>();
            txtBetCur = transform.Find("BetFrame/BetTxt/Cur").GetComponent<Text>();
            
            
            var btns = transform.Find("Btns");
            for(int i=0;i<3;++i) btnAdds[i] = btns.GetChild(i).GetComponent<Button>();
            for(int i=0;i<3;++i) btnMuls[i] = btns.GetChild(3+i).GetComponent<Button>();
            txtBPBtn = btns.Find("BPBtn/Text").GetComponent<Text>();

            btn0 = btns.Find("ZeroBtn").GetComponent<Button>();
            btnAll = btns.Find("AllBtn").GetComponent<Button>();
            btnBP = btns.Find("BPBtn").GetComponent<Button>();

            btnEnough = transform.Find("StopBtn").GetComponent<Button>();
            txtEnoughHint = transform.Find("StopBtn/Text").GetComponent<Text>();

        }

        private void AddListener()
        {
            btnBack.onClick.AddListener(() =>
            {
                //BWAudioManager.Instance.PlayClickEffect();
                SceneManager.LoadScene(0);
            });
            btnBgm.onClick.AddListener(() =>
            {
                CPAudioManager.Instance.PlayClickEffect();
                CPAudioManager.Instance.ChangeBgmState();
                btnBgm.transform.GetComponent<Image>().sprite =
                    CPAudioManager.Instance.musicOn ?
                    MainAssetManager.Instance.bgmOnSprite :
                    MainAssetManager.Instance.bgmOffSprite;
            });
            btnEffect.onClick.AddListener(() =>
            {
                CPAudioManager.Instance.ChangeEffectState();
                btnEffect.transform.GetComponent<Image>().sprite =
                    CPAudioManager.Instance.effectOn ?
                    MainAssetManager.Instance.effectOnSprite :
                    MainAssetManager.Instance.effectOffSprite;
                CPAudioManager.Instance.PlayClickEffect();
            });

            for(int i = 0; i < 3; ++i)
            {
                // 10 100 1000
                int e = i + 1;
                btnAdds[i].onClick.AddListener(() =>
                {
                    CPAudioManager.Instance.PlayClickEffect();
                    CPGameManager.Instance.AddBet((int)Mathf.Pow(10, e));
                    UpdateBet();
                });
            }

            for (int i = 0; i < 3; ++i)
            {
                // 2 5 10
                int a = i + 1;
                btnMuls[i].onClick.AddListener(() =>
                {
                    CPAudioManager.Instance.PlayClickEffect();
                    CPGameManager.Instance.MutiBet(a * a + 1);
                    UpdateBet();
                });
            }

            btn0.onClick.AddListener(() =>
            {
                CPAudioManager.Instance.PlayClickEffect();
                CPGameManager.Instance.RemoveBet();
                UpdateBet();
            });
            btnAll.onClick.AddListener(() =>
            {
                CPAudioManager.Instance.PlayClickEffect();
                CPGameManager.Instance.BetAll();
                UpdateBet();
            });
            btnBP.onClick.AddListener(() =>
            {
                CPAudioManager.Instance.PlayClickEffect();
                CPGameManager.Instance.BP();
                UpdateAll();
            });
            btnEnough.onClick.AddListener(() =>
            {
                CPGameManager.Instance.Enough();
                UpdateEnoughHint();
            });

        }

        public void UpdateGem()
        {
            txtGemCount.text = CPGameManager.Instance.gem.ToString();
        }

        public void UpdateRiseTimes()
        {
            txtTimes.text = string.Format("x{0}", CPGameManager.Instance.riseTime.ToString("F2"));
        }

        public void UpdateLastTime(float lastTime)
        {
            txtLastTimes.text = string.Format("x{0}", lastTime.ToString("F2"));
        }

        public void UpdateBet()
        {
            txtBetAll.text = CPGameManager.Instance.betAll.ToString();
            txtBetCur.text = CPGameManager.Instance.betCur.ToString();
        }

        public void UpdateBPButton()
        {
            txtBPBtn.text = CPGameManager.Instance.isRising ? "Pause" : "Bet";
        }

        public void UpdateEnoughHint()
        {
            
            if (CPGameManager.Instance.isEnough)
            {
                btnEnough.interactable = false;
                int value = Mathf.RoundToInt(CPGameManager.Instance.betAll * CPGameManager.Instance.enoughTime);
                txtEnoughHint.text = string.Format("You got {0}", value);
            }
            else
            {
                btnEnough.interactable = true;
                int win = Mathf.RoundToInt(CPGameManager.Instance.betAll * CPGameManager.Instance.riseTime);
                txtEnoughHint.text = CPGameManager.Instance.isRising ?
                    string.Format("{0} Enough ", win) :
                    "Waiting for bet";
            }
        }

        public void UpdateAll()
        {
            UpdateBet();
            UpdateGem();
            UpdateRiseTimes();
            UpdateBPButton();
            UpdateEnoughHint();
        }

    }
}

