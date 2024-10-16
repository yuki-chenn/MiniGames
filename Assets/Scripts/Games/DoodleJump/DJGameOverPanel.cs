using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MiniGames.DoodleJump
{
    public class DJGameOverPanel : MonoBehaviour
    {
        private CanvasGroup canvas;
        private Button btnBack;
        private Text txtAward;
        private Text txtScore;


        private void Awake()
        {
            EventCenter.AddListener(EventDefine.DJ_Gameover, Show);

            BindComponent();
            AddListener();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener(EventDefine.DJ_Gameover, Show);
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void BindComponent()
        {
            canvas = GetComponent<CanvasGroup>();
            btnBack = transform.Find("BtnBack").GetComponent<Button>();
            txtScore = transform.Find("Score").GetComponent<Text>();
            txtAward = transform.Find("Award/Count").GetComponent<Text>();
        }
        private void AddListener()
        {
            btnBack.onClick.AddListener(() =>
            {
                DJAudioManager.Instance.PlayClickEffect();
                Hide();
                EventCenter.Broadcast(EventDefine.DJ_ShowSettingPanel);
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
            txtScore.text = string.Format("Score:{0}", DJGameManager.Instance.score);
            txtAward.text = string.Format("+{0}", DJGameManager.Instance.score/10);
            gameObject.SetActive(true);
            canvas.DOFade(1, 0.5f);
        }
    }
}
