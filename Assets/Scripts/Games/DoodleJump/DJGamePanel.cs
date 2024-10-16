using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using MiniGames.SAS;
using MiniGames.Main;
using UnityEngine.SocialPlatforms.Impl;
using MiniGames.GuessColor;
using MiniGames.DoodleJump;


namespace MiniGames.DoodleJump
{
    public class DJGamePanel : MonoBehaviour
    {
        private CanvasGroup canvas;

        private Button btnBgm;
        private Button btnEffect;
        private Button btnPause;
        private Text txtScore;

        private void Awake()
        {
            EventCenter.AddListener(EventDefine.DJ_StartGame, StartGame);
            EventCenter.AddListener(EventDefine.DJ_Gameover, Hide);
            EventCenter.AddListener(EventDefine.DJ_Resume, Show);
            EventCenter.AddListener(EventDefine.DJ_RefreshScore, RefreshScore);

            BindComponent();
            AddListener();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener(EventDefine.DJ_StartGame, StartGame);
            EventCenter.RemoveListener(EventDefine.DJ_Gameover, Hide);
            EventCenter.RemoveListener(EventDefine.DJ_Resume, Show);
            EventCenter.RemoveListener(EventDefine.DJ_RefreshScore, RefreshScore);
        }

        private void OnEnable()
        {
            if(DJAudioManager.Instance != null) DJAudioManager.Instance.PlayBgm();
        }

        private void OnDisable()
        {
            if (DJAudioManager.Instance != null) DJAudioManager.Instance.StopBgm();
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }
        private void BindComponent()
        {
            canvas = GetComponent<CanvasGroup>();
            btnPause = transform.Find("BtnPause").GetComponent<Button>();
            btnBgm = transform.Find("BtnBgm").GetComponent<Button>();
            btnEffect = transform.Find("BtnEffect").GetComponent<Button>();
            txtScore = transform.Find("Score").GetComponent<Text>();
        }

        private void AddListener()
        {
            btnPause.onClick.AddListener(() =>
            {
                Hide();
                EventCenter.Broadcast(EventDefine.DJ_Pause);
                DJGameManager.Instance.PauseGame();
 
            });

            btnBgm.onClick.AddListener(() =>
            {
                DJAudioManager.Instance.PlayClickEffect();
                DJAudioManager.Instance.ChangeBgmState();
                btnBgm.transform.GetComponent<Image>().sprite =
                    DJAudioManager.Instance.musicOn ?
                    MainAssetManager.Instance.bgmOnSprite :
                    MainAssetManager.Instance.bgmOffSprite;
            });

            btnEffect.onClick.AddListener(() =>
            {
                DJAudioManager.Instance.ChangeEffectState();
                btnEffect.transform.GetComponent<Image>().sprite =
                    DJAudioManager.Instance.effectOn ?
                    MainAssetManager.Instance.effectOnSprite :
                    MainAssetManager.Instance.effectOffSprite;
                DJAudioManager.Instance.PlayClickEffect();
            });

        }

        private void StartGame()
        {
            RefreshScore();
            Show();
        }

        private void RefreshScore()
        {
            txtScore.text = DJGameManager.Instance.score.ToString();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

    }
}

