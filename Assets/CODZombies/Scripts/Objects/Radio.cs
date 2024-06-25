#if H3VR_IMPORTED

using System;
using System.Collections;
using CustomScripts.Managers.Sound;
using CustomScripts.Multiplayer;
using FistVR;
using UnityEngine;
namespace CustomScripts.Objects
{
    public class Radio : MonoBehaviour, IFVRDamageable
    {
        public AudioClip Song;

        private bool _isThrottled;
        private bool _isPlaying;

        private Coroutine _musicEndCoroutine;
        
        public void Damage(Damage dam)
        {
            if (dam.Class == FistVR.Damage.DamageClass.Explosive)
                return;
            
            if (_isThrottled)
                return;

            if (Networking.IsSolo())
            {
                ToggleMusic();
            }
            else if (Networking.IsHost())
            {
                //ToggleMusic();
                CodZNetworking.Instance.CustomData_Send((int)CustomDataType.RADIO_TOGGLE);
            }
            else
            {
                CodZNetworking.Instance.Client_CustomData_Send((int)CustomDataType.RADIO_TOGGLE);
            }
        }

        public void ToggleMusic()
        {
            if (_isPlaying)
            {
                _isPlaying = false;

                AudioManager.Instance.MusicAudioSource.Stop();

                if (_musicEndCoroutine != null)
                    StopCoroutine(_musicEndCoroutine);
            }
            else
            {
                _isPlaying = true;

                var musicLength = Song.length;
                _musicEndCoroutine = StartCoroutine(OnMusicEnd(musicLength));

                AudioManager.Instance.PlayMusic(Song, 0.095f);
            }

            StartCoroutine(Throttle());
        }

        private IEnumerator Throttle()
        {
            _isThrottled = true;
            yield return new WaitForSeconds(.5f);
            _isThrottled = false;
        }

        private IEnumerator OnMusicEnd(float endTimer)
        {
            yield return new WaitForSeconds(endTimer);

            _isPlaying = false;
        }
    }
}
#endif