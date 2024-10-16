using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using MiniGames.Enum;

namespace MiniGames.SAS
{
    public class SASSettingPanel : MonoBehaviour
    {
        private CanvasGroup canvas;

        private Button btnStart;
        private Button btnBack;

        private Transform mapContent;

        private void Awake()
        {
            EventCenter.AddListener(EventDefine.SAS_ShowSettingPanel, Show);

            BindComponent();
            AddListener();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener(EventDefine.SAS_ShowSettingPanel, Show);
        }

        private void Start()
        {
            gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            // 清空mapContent中的所有block
            for (int i = 0; i < mapContent.childCount; i++)
            {
                Destroy(mapContent.GetChild(i).gameObject);
            }
        }

        private void BindComponent()
        {
            canvas = GetComponent<CanvasGroup>();
            mapContent = transform.parent.Find("GamePanel/Map");
            btnStart = transform.Find("BtnStart").GetComponent<Button>();
            btnBack = transform.Find("BtnBack").GetComponent<Button>();
        }
        private void AddListener()
        {
            btnStart.onClick.AddListener(() =>
            {
                SASAudioManager.Instance.PlayButtonEffect();
                SASGameManager.Instance.StartGame();
                Hide();
            });
            btnBack.onClick.AddListener(() =>
            {
                SASAudioManager.Instance.PlayButtonEffect();
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

