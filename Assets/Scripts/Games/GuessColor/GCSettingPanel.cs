using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;


namespace MiniGames.GuessColor
{
    public class GCSettingPanel : MonoBehaviour
    {
        private CanvasGroup canvas;

        private Button btnStart;
        private Button btnBack;

        private Button btnColorNumPrev;
        private Button btnColorNumNext;
        private Text txtColorNum;

        private Button btnGuessNumPrev;
        private Button btnGuessNumNext;
        private Text txtGuessNum;

        private Button btnAttemptNumPrev;
        private Button btnAttemptNumNext;
        private Text txtAttemptNum;

        private Button btnColorRepeatPrev;
        private Button btnColorRepeatNext;
        private Text txtColorRepeat;

        private void Awake()
        {
            EventCenter.AddListener(EventDefine.GC_ShowSettingPanel, Show);

            BindComponent();
            AddListener();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener(EventDefine.GC_ShowSettingPanel, Show);
        }

        private void Start()
        {
            gameObject.SetActive(true);
            Refresh();
        }

        private void BindComponent()
        {
            canvas = GetComponent<CanvasGroup>();
            btnStart = transform.Find("BtnStart").GetComponent<Button>();
            btnBack = transform.Find("BtnBack").GetComponent<Button>();

            // settings
            var settings = transform.Find("Settings");
            btnColorNumPrev = settings.Find("ColorNum/BtnPrev").GetComponent<Button>();
            btnColorNumNext = settings.Find("ColorNum/BtnNext").GetComponent<Button>();
            txtColorNum = settings.Find("ColorNum/Value").GetComponent<Text>();

            btnAttemptNumNext = settings.Find("AttemptNum/BtnNext").GetComponent<Button>();
            btnAttemptNumPrev = settings.Find("AttemptNum/BtnPrev").GetComponent<Button>();
            txtAttemptNum = settings.Find("AttemptNum/Value").GetComponent<Text>();

            btnGuessNumNext = settings.Find("GuessNum/BtnNext").GetComponent<Button>();
            btnGuessNumPrev = settings.Find("GuessNum/BtnPrev").GetComponent<Button>();
            txtGuessNum = settings.Find("GuessNum/Value").GetComponent<Text>();

            btnColorRepeatNext = settings.Find("ColorRepeat/BtnNext").GetComponent<Button>();
            btnColorRepeatPrev = settings.Find("ColorRepeat/BtnPrev").GetComponent<Button>();
            txtColorRepeat = settings.Find("ColorRepeat/Value").GetComponent<Text>();

        }
        private void AddListener()
        {
            btnStart.onClick.AddListener(() =>
            {
                GCAudioManager.Instance.PlaySelectEffect();
                GCGameManager.Instance.StartGame();
                Hide();
                EventCenter.Broadcast(EventDefine.GC_StartGame);
            });
            btnBack.onClick.AddListener(() =>
            {
                GCAudioManager.Instance.PlaySelectEffect();
                SceneManager.LoadScene(0);
            });

            // settings
            btnColorNumPrev.onClick.AddListener(() =>
            {
                GCAudioManager.Instance.PlaySelectEffect();
                if (GCGameManager.Instance.setting.colorNum <= Constants.MIN_COLOR_NUM) return;
                GCGameManager.Instance.setting.colorNum--;
                Refresh();
            });

            btnColorNumNext.onClick.AddListener(() =>
            {
                GCAudioManager.Instance.PlaySelectEffect();
                if (GCGameManager.Instance.setting.colorNum >= Constants.MAX_COLOR_NUM) return;
                GCGameManager.Instance.setting.colorNum++;
                Refresh();
            });

            btnGuessNumPrev.onClick.AddListener(() =>
            {
                GCAudioManager.Instance.PlaySelectEffect();
                if (GCGameManager.Instance.setting.guessNum <= Constants.MIN_GUESS_NUM) return;
                GCGameManager.Instance.setting.guessNum--;
                Refresh();
            });

            btnGuessNumNext.onClick.AddListener(() =>
            {
                GCAudioManager.Instance.PlaySelectEffect();
                if (GCGameManager.Instance.setting.guessNum >= Constants.MAX_GUESS_NUM) return;
                GCGameManager.Instance.setting.guessNum++;
                Refresh();
            });

            btnAttemptNumPrev.onClick.AddListener(() =>
            {
                GCAudioManager.Instance.PlaySelectEffect();
                if (GCGameManager.Instance.setting.attemptNum <= Constants.MIN_ATTEMPT_NUM) return;
                GCGameManager.Instance.setting.attemptNum--;
                Refresh();
            });

            btnAttemptNumNext.onClick.AddListener(() =>
            {
                GCAudioManager.Instance.PlaySelectEffect();
                if (GCGameManager.Instance.setting.attemptNum >= Constants.MAX_ATTEMPT_NUM) return;
                GCGameManager.Instance.setting.attemptNum++;
                Refresh();
            });

            btnColorRepeatPrev.onClick.AddListener(() =>
            {
                GCAudioManager.Instance.PlaySelectEffect();
                GCGameManager.Instance.setting.colorRepeat = !GCGameManager.Instance.setting.colorRepeat;
                Refresh();
            });

            btnColorRepeatNext.onClick.AddListener(() =>
            {
                GCAudioManager.Instance.PlaySelectEffect();
                GCGameManager.Instance.setting.colorRepeat = !GCGameManager.Instance.setting.colorRepeat;
                Refresh();
            });

        }

        private void Refresh()
        {
            txtColorNum.text = GCGameManager.Instance.setting.colorNum.ToString();
            txtGuessNum.text = GCGameManager.Instance.setting.guessNum.ToString();
            txtAttemptNum.text = GCGameManager.Instance.setting.attemptNum.ToString();
            txtColorRepeat.text = GCGameManager.Instance.setting.colorRepeat ? "Yes" : "No";
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

