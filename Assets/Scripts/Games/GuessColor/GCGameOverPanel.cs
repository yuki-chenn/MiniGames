using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MiniGames.GuessColor;

namespace MiniGames.GuessColor
{
    public class GCGameOverPanel : MonoBehaviour
    {
        private CanvasGroup canvas;
        private Text txtHint;
        private Button btnRestart;
        private Button btnBack;
        private Button btnHide;
        private GameObject awardGo;
        private Text txtAward;

        private int rewardGem
        {
            get { return GCGameManager.Instance.guessData.rewardGem; }
        }


        private void Awake()
        {
            EventCenter.AddListener<int>(EventDefine.GC_ShowGameoverPanel, Show);

            BindComponent();
            AddListener();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener<int>(EventDefine.GC_ShowGameoverPanel, Show);
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void BindComponent()
        {
            canvas = GetComponent<CanvasGroup>();
            txtHint = transform.Find("TxtHint").GetComponent<Text>();
            btnRestart = transform.Find("BtnRestart").GetComponent<Button>();
            btnBack = transform.Find("BtnBack").GetComponent<Button>();
            btnHide = transform.Find("BtnHide").GetComponent<Button>();
            awardGo = transform.Find("Award").gameObject;
            txtAward = transform.Find("Award/Count").GetComponent<Text>();
        }
        private void AddListener()
        {
            btnRestart.onClick.AddListener(() =>
            {
                GCAudioManager.Instance.PlaySelectEffect();
                Hide();
                GCGameManager.Instance.StartGame();
                EventCenter.Broadcast(EventDefine.GC_StartGame);
            });
            btnBack.onClick.AddListener(() =>
            {
                GCAudioManager.Instance.PlaySelectEffect();
                Hide();
                EventCenter.Broadcast(EventDefine.GC_ShowSettingPanel);
            });
            btnHide.onClick.AddListener(() => {
                GCAudioManager.Instance.PlaySelectEffect();
                Hide();
            });
        }

        private void Hide()
        {
            canvas.DOFade(0, 0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });

        }

        private void Show(int flag)
        {
            txtHint.text = flag > 0 ? "¹§Ï²¹ý¹Ø":"ÒÅº¶Ê§°Ü";
            awardGo.SetActive(flag > 0);
            if (flag > 0)
            {
                txtAward.text = string.Format("+{0}", rewardGem.ToString());
            }
            gameObject.SetActive(true);
            canvas.DOFade(1, 0.5f);
        }
    }
}
