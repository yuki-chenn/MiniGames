using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MiniGames.SAS
{
    public class SASGameOverPanel : MonoBehaviour
    {
        private CanvasGroup canvas;
        private Text txtHint;
        private Button btnRestart;
        private Button btnBack;
        private GameObject awardGo;
        private Text txtAward;

        private int diff { get { return SASGameManager.Instance.selectDiff; } }

        private void Awake()
        {
            EventCenter.AddListener<int>(EventDefine.SAS_ShowGameoverPanel, Show);

            BindComponent();
            AddListener();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener<int>(EventDefine.SAS_ShowGameoverPanel, Show);
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
            awardGo = transform.Find("Award").gameObject;
            txtAward = transform.Find("Award/Count").GetComponent<Text>();
        }
        private void AddListener()
        {
            btnRestart.onClick.AddListener(() =>
            {
                SASAudioManager.Instance.PlayButtonEffect();
                Hide();
                SASGameManager.Instance.StartGame();
            });
            btnBack.onClick.AddListener(() =>
            {
                SASAudioManager.Instance.PlayButtonEffect();
                Hide();
                EventCenter.Broadcast(EventDefine.SAS_ShowSettingPanel);
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
                txtAward.text = string.Format("+{0}", Constants.REWARD_GEM[diff]);
            }
            gameObject.SetActive(true);
            canvas.DOFade(1, 0.5f);
        }
    }
}
