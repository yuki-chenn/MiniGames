using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using MiniGames.Main;
using MiniGames.GuessColor;


namespace MiniGames.DoodleJump
{
    public class DJSettingPanel : MonoBehaviour
    {
        private CanvasGroup canvas;

        private Button btnStart;
        private Button btnBack;

        private Text txtBestScore;


        private void Awake()
        {
            EventCenter.AddListener(EventDefine.DJ_ShowSettingPanel, Show);

            BindComponent();
            AddListener();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener(EventDefine.DJ_ShowSettingPanel, Show);
        }

        private void Start()
        {
            gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                txtBestScore.text = "Best Score:" + GameManager.Instance.gameData.djGameData.bestScore.ToString();
            }
        }

        private void BindComponent()
        {
            canvas = GetComponent<CanvasGroup>();
            btnStart = transform.Find("BtnStart").GetComponent<Button>();
            btnBack = transform.Find("BtnBack").GetComponent<Button>();
            txtBestScore = transform.Find("BestScore").GetComponent<Text>();
        }
        private void AddListener()
        {
            btnStart.onClick.AddListener(() =>
            {
                if (DJGameManager.Instance.isStart) return;
                DJAudioManager.Instance.PlayClickEffect();
                DJGameManager.Instance.StartGame();
                Hide();
                EventCenter.Broadcast(EventDefine.DJ_StartGame);
            });
            btnBack.onClick.AddListener(() =>
            {
                DJAudioManager.Instance.PlayClickEffect();
                SceneManager.LoadScene(0);
            });
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
            DJGameManager.Instance.player.GetComponent<Rigidbody2D>().gravityScale = 4;
            gameObject.SetActive(true);
            canvas.DOFade(1, 0.5f);
        }

    }
}

