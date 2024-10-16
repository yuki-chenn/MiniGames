using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using MiniGames.SAS;
using MiniGames.Main;
using UnityEngine.SocialPlatforms.Impl;
using MiniGames.GuessColor;


namespace MiniGames.BigWatermelon
{
    public class BWGamePanel : MonoBehaviour
    {
        private CanvasGroup canvas;

        private Button btnBack;
        private Button btnBgm;
        private Button btnEffect;

        private Text txtScore;
        private Text txtBestScore;

        private void Awake()
        {
            EventCenter.AddListener(EventDefine.BW_StartGame, StartGame);
            EventCenter.AddListener(EventDefine.BW_RefreshScore, RefreshScore);

            BindComponent();
            AddListener();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener(EventDefine.BW_StartGame, StartGame);
            EventCenter.RemoveListener(EventDefine.BW_RefreshScore, RefreshScore);
        }

        private void OnEnable()
        {
            if(BWAudioManager.Instance != null) BWAudioManager.Instance.PlayBgm();
        }

        private void OnDisable()
        {
            if (BWAudioManager.Instance != null) BWAudioManager.Instance.StopBgm();
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }
        private void BindComponent()
        {
            canvas = GetComponent<CanvasGroup>();
            btnBack = transform.Find("BtnBack").GetComponent<Button>();
            btnBgm = transform.Find("BtnBgm").GetComponent<Button>();
            btnEffect = transform.Find("BtnEffect").GetComponent<Button>();
            txtScore = transform.Find("Score/Value").GetComponent<Text>();
            txtBestScore = transform.Find("BestScore/Value").GetComponent<Text>();
        }

        private void AddListener()
        {
            btnBack.onClick.AddListener(() =>
            {
                BWAudioManager.Instance.PlayClickEffect();
                EventCenter.Broadcast(EventDefine.BW_ShowSettingPanel);
                Hide();
                BWGameManager.Instance.UpdateBestScore();
            });
            btnBgm.onClick.AddListener(() =>
            {
                BWAudioManager.Instance.PlayClickEffect();
                BWAudioManager.Instance.ChangeBgmState();
                btnBgm.transform.GetComponent<Image>().sprite =
                    BWAudioManager.Instance.musicOn ?
                    MainAssetManager.Instance.bgmOnSprite :
                    MainAssetManager.Instance.bgmOffSprite;
            });

            btnEffect.onClick.AddListener(() =>
            {
                BWAudioManager.Instance.ChangeEffectState();
                btnEffect.transform.GetComponent<Image>().sprite =
                    BWAudioManager.Instance.effectOn ?
                    MainAssetManager.Instance.effectOnSprite :
                    MainAssetManager.Instance.effectOffSprite;
                BWAudioManager.Instance.PlayClickEffect();
            });

        }

        private void StartGame()
        {
            RefreshScore();
            Show();
        }

        private void RefreshScore()
        {
            txtScore.text = BWGameManager.Instance.score.ToString();
            if(GameManager.Instance != null)
            {
                txtBestScore.text = GameManager.Instance.gameData.bwGameData.bestScore.ToString();
            }

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

