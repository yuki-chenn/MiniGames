using MiniGames.Base;
using UnityEngine;

namespace MiniGames.Collapse
{
    public class CPAudioManager : Singleton<CPAudioManager>
    {
        private AudioSource bgmAudioSource;
        private AudioSource effectAudioSource;

        public bool effectOn = true;
        public bool musicOn = true;

        public AudioClip effectClick;
        public AudioClip effectCollapse;
        public AudioClip effectEnough;

        public AudioClip bgmRise;

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
            PlayBgmAudio(bgmRise);
        }

        public void PlayClickEffect()
        {
            PlayEffectAudio(effectClick);
        }

        public void PlayCollapseEffect()
        {
            PlayEffectAudio(effectCollapse);
        }

        public void PlayEnoughEffect()
        {
            PlayEffectAudio(effectEnough);
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

