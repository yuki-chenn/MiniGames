using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MiniGames.Main
{
    public class SelectGamePanel : MonoBehaviour
    {
        private CanvasGroup canvas;

        private Text txtGemCount;

        private Transform gameContent;


        private void Awake()
        {
            BindComponent();
            AddListener();
        }

        private void OnDestroy()
        {
        }

        private void Start()
        {
            RefreshGem();
        }

        private void BindComponent()
        {
            canvas = GetComponent<CanvasGroup>();
            txtGemCount = transform.Find("Gem/Count").GetComponent<Text>();
            gameContent = transform.Find("ScrollRect/Viewport/Content");
        }
        private void AddListener()
        {
            for (int i = 0; i < gameContent.childCount; i++)
            {
                var child = gameContent.GetChild(i);
                Button button = child.GetComponent<Button>();

                int gameIndex = i + 1;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    SceneManager.LoadScene(gameIndex);
                });
            }
        }

        private void RefreshGem()
        {
            txtGemCount.text = GameManager.Instance.gameData.gemCount.ToString();
        }
    }
}

