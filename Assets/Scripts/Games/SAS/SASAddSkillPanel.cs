using DG.Tweening;
using MiniGames.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.SAS
{
    public class SASAddSkillPanel : MonoBehaviour
    {
        private CanvasGroup canvas;

        private Button btnGet;
        private Button btnBack;

        private Text txtGetCount;
        private Text txtHint;
        private Text txtSkillCost;


        private int skillType = -1;

        private int diff { get { return SASGameManager.Instance.selectDiff; } }

        private int gem { get { return GameManager.Instance.gameData.gemCount; } }


        private void Awake()
        {
            EventCenter.AddListener<int>(EventDefine.SAS_ShowAddSkillPanel, Show);

            BindComponent();
            AddListener();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener<int>(EventDefine.SAS_ShowAddSkillPanel, Show);
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void BindComponent()
        {
            canvas = GetComponent<CanvasGroup>();
            txtGetCount = transform.Find("Frame/BtnGet/Text").GetComponent<Text>();
            txtHint = transform.Find("Frame/TxtHint").GetComponent<Text>();
            txtSkillCost = transform.Find("Frame/Cost/Count").GetComponent<Text>();
            btnGet = transform.Find("Frame/BtnGet").GetComponent<Button>();
            btnBack = transform.Find("Frame/BtnBack").GetComponent<Button>();
        }
        private void AddListener()
        {
            btnGet.onClick.AddListener(() =>
            {
                SASAudioManager.Instance.PlayButtonEffect();
                switch (skillType)
                {
                    case 0:
                        // withdraw
                        if (SASGameManager.Instance.withdrawAdvCount > 0 && 
                        gem >= Constants.WITHDRAW_COST)
                        {
                            GameManager.Instance.ModifyGem(-Constants.WITHDRAW_COST);
                            SASGameManager.Instance.withdrawCount++;
                            SASGameManager.Instance.withdrawAdvCount--;
                        }
                        break;
                    case 1:
                        // shuffle
                        if (SASGameManager.Instance.shuffleAdvCount > 0 &&
                        gem >= Constants.SHUFFLE_COST)
                        {
                            GameManager.Instance.ModifyGem(-Constants.SHUFFLE_COST);
                            SASGameManager.Instance.shuffleCount++;
                            SASGameManager.Instance.shuffleAdvCount--;
                        }
                        break;
                    case 2:
                        // pop3
                        if (SASGameManager.Instance.Pop3AdvCount > 0 &&
                        gem >= Constants.POP3_COST)
                        {
                            GameManager.Instance.ModifyGem(-Constants.POP3_COST);
                            SASGameManager.Instance.pop3Count++;
                            SASGameManager.Instance.Pop3AdvCount--;
                        }
                        break;
                    default:
                        Debug.LogError("skilltype" + skillType + "不存在");
                        break;
                }
                Hide();
                EventCenter.Broadcast(EventDefine.SAS_RefreshSkillCount);
            });
            btnBack.onClick.AddListener(() => 
            {
                SASAudioManager.Instance.PlayButtonEffect();
                Hide();
            });
        }

        private void RefreshText()
        {
            string hint = string.Empty;
            switch (skillType)
            {
                case 0:
                    // withdraw
                    hint = "获取撤回道具";
                    break;
                case 1:
                    // shuffle
                    hint = "获取洗牌道具";
                    break;
                case 2:
                    // pop3
                    hint = "获取弹出道具"; 
                    break;
                default:
                    Debug.LogError("skilltype" + skillType + "不存在");
                    break;
            }

            txtHint.text = hint;

            int canGet = -1, maxCanGet = -1;
            switch (skillType)
            {
                case 0:
                    // withdraw
                    canGet = SASGameManager.Instance.withdrawAdvCount;
                    maxCanGet = Constants.MAX_WITHDRAW_COUNT[diff] - Constants.FREE_WITHDRAW_COUNT[diff];
                    break;
                case 1:
                    // shuffle
                    canGet = SASGameManager.Instance.shuffleAdvCount;
                    maxCanGet = Constants.MAX_SHUFFLE_COUNT[diff] - Constants.FREE_SHUFFLE_COUNT[diff];
                    break;
                case 2:
                    // pop3
                    canGet = SASGameManager.Instance.Pop3AdvCount;
                    maxCanGet = Constants.MAX_POP3_COUNT[diff] - Constants.FREE_POP3_COUNT[diff];
                    break;
                default:
                    Debug.LogError("skilltype" + skillType + "不存在");
                    break;
            }

            txtGetCount.text = string.Format("获取（{0}/{1}）", canGet,maxCanGet);
            int cost = -1;
            switch (skillType)
            {
                case 0:
                    // withdraw
                    cost = Constants.WITHDRAW_COST;
                    break;
                case 1:
                    // shuffle
                    cost = Constants.SHUFFLE_COST;
                    break;
                case 2:
                    // pop3
                    cost = Constants.POP3_COST;
                    break;
                default:
                    break;
            }
            txtSkillCost.text = cost.ToString();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Show(int skillType)
        {
            this.skillType = skillType;
            RefreshText();
            gameObject.SetActive(true);
            canvas.DOFade(1, 0.5f);
        }

    }
}

