using MiniGames.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.SAS
{
    public class SASAudioManager : Singleton<SASAudioManager>
    {
        private AudioSource bgmAudioSource;
        private AudioSource effectAudioSource;

        public bool effectOn = true;
        public bool musicOn = true;

        public AudioClip effectClick;
        public AudioClip effectMatch;
        public AudioClip effectShuffle;
        public AudioClip effectWithdraw;
        public AudioClip effectPop;
        public AudioClip effectButton;

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

        public void PlayClickEffect()
        {
            PlayEffectAudio(effectClick);
        }

        public void PlayMatchEffect()
        {
            PlayEffectAudio(effectMatch);
        }

        public void PlayShuffleEffect()
        {
            PlayEffectAudio(effectShuffle);
        }

        public void PlayWithdrawEffect()
        {
            PlayEffectAudio(effectWithdraw);
        }

        public void PlayPopEffect()
        {
            PlayEffectAudio(effectPop);
        }

        public void PlayButtonEffect()
        {
            PlayEffectAudio(effectButton);
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

