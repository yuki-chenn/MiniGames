using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MiniGames.BigWatermelon
{
    public class BWGameOverPanel : MonoBehaviour
    {
        private CanvasGroup canvas;
        private Text txtHint;
        private Button btnRestart;
        private Button btnBack;
        private GameObject awardGo;
        private Text txtAward;


        private void Awake()
        {
            EventCenter.AddListener(EventDefine.BW_ShowGameoverPanel, Show);

            BindComponent();
            AddListener();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener(EventDefine.BW_ShowGameoverPanel, Show);
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
                BWAudioManager.Instance.PlayClickEffect();
                Hide();
                BWGameManager.Instance.StartGame();
                EventCenter.Broadcast(EventDefine.BW_StartGame);
            });
            btnBack.onClick.AddListener(() =>
            {
                BWAudioManager.Instance.PlayClickEffect();
                Hide();
                EventCenter.Broadcast(EventDefine.BW_ShowSettingPanel);
            });
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
            txtHint.text = "“≈∫∂ ß∞‹";
            awardGo.SetActive(true);
            txtAward.text = string.Format("+{0}", BWGameManager.Instance.score/100);
            gameObject.SetActive(true);
            canvas.DOFade(1, 0.5f);
        }
    }
}
