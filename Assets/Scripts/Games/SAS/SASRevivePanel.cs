using DG.Tweening;
using MiniGames.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.SAS
{
    public class SASRevivePanel : MonoBehaviour
    {
        private CanvasGroup canvas;

        private Button btnRevive;
        private Button btnEndGame;

        private Text txtReviveCount;
        private Text txtReviveCost;

        private int reviveGem 
        { 
            get
            {
                return SASGameManager.Instance.reviveCount < Constants.FREE_REVIVE_COUNT ? 0 : Constants.REVIVE_COST;
            } 
        }

        private void Awake()
        {
            EventCenter.AddListener<int>(EventDefine.SAS_ShowRevivePanel, Show);

            BindComponent();
            AddListener();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener<int>(EventDefine.SAS_ShowRevivePanel, Show);
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void BindComponent()
        {
            canvas = GetComponent<CanvasGroup>();
            txtReviveCount = transform.Find("Frame/BtnRevive/Text").GetComponent<Text>();
            txtReviveCost = transform.Find("Frame/Cost/Count").GetComponent<Text>();
            btnRevive = transform.Find("Frame/BtnRevive").GetComponent<Button>();
            btnEndGame = transform.Find("Frame/BtnEndGame").GetComponent<Button>();
        }

        private void AddListener()
        {
            btnRevive.onClick.AddListener(() =>
            {
                SASAudioManager.Instance.PlayButtonEffect();
                GameManager.Instance.ModifyGem(-reviveGem);
                Hide();
                EventCenter.Broadcast(EventDefine.SAS_Revive);
            });
            btnEndGame.onClick.AddListener(() => 
            {
                SASAudioManager.Instance.PlayButtonEffect();
                Hide();
                EventCenter.Broadcast(EventDefine.SAS_ShowGameoverPanel, -1);
            });
        }

        private void Hide()
        {
            canvas.DOFade(0, 0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });

        }

        private void Show(int reviveCount)
        {
            txtReviveCount.text = string.Format("¸´»î{0}/{1}", 
                reviveCount.ToString(), 
                Constants.MAX_REVIVE_COUNT[SASGameManager.Instance.selectDiff]);
            txtReviveCost.text = reviveGem.ToString();
            gameObject.SetActive(true);
            canvas.DOFade(1, 0.5f);
        }

    }
}

