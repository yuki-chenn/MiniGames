using MiniGames.Base;
using UnityEngine;

namespace MiniGames.BigWatermelon
{
    public class BWAudioManager : Singleton<BWAudioManager>
    {
        private AudioSource bgmAudioSource;
        private AudioSource effectAudioSource;

        public bool effectOn = true;
        public bool musicOn = true;

        public AudioClip effectClick;
        public AudioClip effectMerge;
        public AudioClip effectRelease;
        public AudioClip effectWatermelon;
        public AudioClip effectBonus;
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

        public void PlayClickEffect()
        {
            PlayEffectAudio(effectClick);
        }

        public void PlayMergeEffect()
        {
            PlayEffectAudio(effectMerge);
        }

        public void PlayReleaseEffect()
        {
            PlayEffectAudio(effectRelease);
        }

        public void PlayWatermelonEffect()
        {
            PlayEffectAudio(effectWatermelon);
        }

        public void PlayBonusEffect()
        {
            PlayEffectAudio(effectBonus);
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

