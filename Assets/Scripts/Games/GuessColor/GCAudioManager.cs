using MiniGames.Base;
using UnityEngine;

namespace MiniGames.GuessColor
{
    public class GCAudioManager : Singleton<GCAudioManager>
    {
        private AudioSource bgmAudioSource;
        private AudioSource effectAudioSource;

        public bool effectOn = true;
        public bool musicOn = true;

        public AudioClip effectSelect;
        public AudioClip effectSwitch;
        public AudioClip effectSubmit;
        public AudioClip effectShowRes;
        public AudioClip effectWin;
        public AudioClip effectLose;

        public AudioClip bgmGame;

        protected override void Awake()
        {
            base.Awake();
            var ass = GetComponents<AudioSource>();
            bgmAudioSource = ass[0];
            effectAudioSource = ass[1];
            bgmAudioSource.loop = true;
        }

        public void ChangeBgmState()
        {
            musicOn = !musicOn;
            if (musicOn)
            {
                bgmAudioSource.volume = 1;
            }
            else
            {
                bgmAudioSource.volume = 0;
            }
        }

        public void ChangeEffectState()
        {
            effectOn = !effectOn;
        }

        public void PlayBgm()
        {
            PlayBgmAudio(bgmGame);
        }

        public void PlaySelectEffect()
        {
            PlayEffectAudio(effectSelect);
        }

        public void PlaySwitchEffect()
        {
            PlayEffectAudio(effectSwitch);
        }

        public void PlaySubmitEffect()
        {
            PlayEffectAudio(effectSubmit);
        }

        public void PlayShowResEffect()
        {
            PlayEffectAudio(effectShowRes);
        }

        public void PlayWinEffect()
        {
            PlayEffectAudio(effectWin);
        }

        public void PlayLoseEffect()
        {
            PlayEffectAudio(effectLose);
        }


        private void PlayBgmAudio(AudioClip clip)
        {
            if(bgmAudioSource.clip == clip) return;

            bgmAudioSource.clip = clip;
            bgmAudioSource.Play();
        }
        public void StopBgm()
        {
            bgmAudioSource.clip = null;
            bgmAudioSource.Stop();
        }


        private void PlayEffectAudio(AudioClip clip)
        {
            if (!effectOn) return;
            effectAudioSource.PlayOneShot(clip);
        }

    }
}

