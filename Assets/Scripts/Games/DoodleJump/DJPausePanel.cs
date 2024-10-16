using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using MiniGames.SAS;
using MiniGames.Main;
using UnityEngine.SocialPlatforms.Impl;
using MiniGames.GuessColor;
using MiniGames.DoodleJump;


namespace MiniGames.BigWatermelon
{
    public class DJPausePanel : MonoBehaviour
    {
        private CanvasGroup canvas;

        private Button btnMenu;
        private Button btnResume;
        

        private void Awake()
        {
            EventCenter.AddListener(EventDefine.DJ_Pause, Show);

            BindComponent();
            AddListener();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener(EventDefine.DJ_Pause, Show);
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }
        private void BindComponent()
        {
            canvas = GetComponent<CanvasGroup>();
            btnResume = transform.Find("BtnResume").GetComponent<Button>();
            btnMenu = transform.Find("BtnMenu").GetComponent<Button>();
        }

        private void AddListener()
        {
            btnResume.onClick.AddListener(() =>
            {
                DJAudioManager.Instance.PlayClickEffect();
                Hide();
                EventCenter.Broadcast(EventDefine.DJ_Resume);
                DJGameManager.Instance.ResumeGame();
            });

            btnMenu.onClick.AddListener(() =>
            {
                DJAudioManager.Instance.PlayClickEffect();
                Hide();
                DJGameManager.Instance.Reset();
                EventCenter.Broadcast(EventDefine.DJ_ShowSettingPanel);
               
            });


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

