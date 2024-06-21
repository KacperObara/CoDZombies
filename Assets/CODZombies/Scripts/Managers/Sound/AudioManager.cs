#if H3VR_IMPORTED
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CustomScripts
{
    public class AudioManager : MonoBehaviourSingleton<AudioManager>
    {
        public List<AudioClip> FarZombieSounds;
        public List<AudioClip> CloseZombieSounds;
        public List<AudioClip> HellHoundsSounds;

        public AudioSource MainAudioSource;
        public AudioSource MusicAudioSource;

        [Space(20)]
        public AudioClip BuySound;
        public AudioClip DrinkSound;

        public AudioClip HellHoundSpawnSound;
        public AudioClip HellHoundDeathSound;

        public AudioClip RoundStartSound;
        public AudioClip RoundEndSound;
        public AudioClip HellHoundRoundStartSound;

        public AudioClip PowerOnSound;

        public AudioClip EndMusic;

        public AudioClip TeleportingSound;

        public AudioClip PlayerHitSound;

        public AudioClip BarricadeRepairSound;

        private Coroutine _musicRepetition;

        public void Play(AudioClip audioClip, float volume = 1f, float delay = 0f)
        {
            if (delay != 0)
                StartCoroutine(PlayDelayed(audioClip, volume, delay));
            else
                MainAudioSource.PlayOneShot(audioClip, volume);
        }

        private IEnumerator PlayDelayed(AudioClip audioClip, float volume, float delay)
        {
            yield return new WaitForSeconds(delay);
            MainAudioSource.PlayOneShot(audioClip, volume);
        }

        /// <summary>
        /// Used to stop old sound when playing the new one
        /// </summary>
        public void PlayMusic(AudioClip audioClip, float volume = 1f, float delay = 0f)
        {
            MusicAudioSource.clip = audioClip;
            MusicAudioSource.volume = volume;
            MusicAudioSource.PlayDelayed(delay);
        }
    }
}
#endif