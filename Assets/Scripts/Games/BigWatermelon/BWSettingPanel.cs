using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using MiniGames.Main;
using MiniGames.GuessColor;


namespace MiniGames.BigWatermelon
{
    public class BWSettingPanel : MonoBehaviour
    {
        private CanvasGroup canvas;

        private Button btnStart;
        private Button btnBack;

        private Text txtBestScore;


        private void Awake()
        {
            EventCenter.AddListener(EventDefine.BW_ShowSettingPanel, Show);

            BindComponent();
            AddListener();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener(EventDefine.BW_ShowSettingPanel, Show);
        }

        private void Start()
        {
            gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                txtBestScore.text = GameManager.Instance.gameData.bwGameData.bestScore.ToString();
            }
        }

        private void BindComponent()
        {
            canvas = GetComponent<CanvasGroup>();
            btnStart = transform.Find("BtnStart").GetComponent<Button>();
            btnBack = transform.Find("BtnBack").GetComponent<Button>();
            txtBestScore = transform.Find("BestScore/Value").GetComponent<Text>();
        }
        private void AddListener()
        {
            btnStart.onClick.AddListener(() =>
            {
                BWAudioManager.Instance.PlayClickEffect();
                BWGameManager.Instance.StartGame();
                Hide();
                EventCenter.Broadcast(EventDefine.BW_StartGame);
            });
            btnBack.onClick.AddListener(() =>
            {
                BWAudioManager.Instance.PlayClickEffect();
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
            gameObject.SetActive(true);
            canvas.DOFade(1, 0.5f);
        }

    }
}

