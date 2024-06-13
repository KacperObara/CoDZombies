#if H3VR_IMPORTED
using CustomScripts.Zombie;
using UnityEngine;
namespace CustomScripts
{
    public class RandomZombieSound : MonoBehaviour
    {
        public void Initialize()
        {
            Invoke("PlayRandomSound", Random.Range(4f, 6f));
        }

        void PlayRandomSound()
        {
            // if (Controller.State == State.Dead)
            //     return;

            SoundPoolableObject soundPoolable = SoundPool.Instance.Spawn();
            if (soundPoolable == null)
                return;
            soundPoolable.transform.position = transform.position;
            soundPoolable.Initialize();
            soundPoolable.AudioSource.pitch = Random.Range(0.8f, 1.2f);

            if (RoundManager.Instance.IsRoundSpecial)
            {
                soundPoolable.AudioSource.clip =
                    AudioManager.Instance.HellHoundsSounds[
                        Random.Range(0, AudioManager.Instance.HellHoundsSounds.Count)];
            }
            else if (GameRefs.Instance.IsPlayerClose(transform, 5f))
            {
                soundPoolable.AudioSource.clip =
                    AudioManager.Instance.CloseZombieSounds[
                        Random.Range(0, AudioManager.Instance.CloseZombieSounds.Count)];
            }
            else
            {
                soundPoolable.AudioSource.clip =
                    AudioManager.Instance.FarZombieSounds[
                        Random.Range(0, AudioManager.Instance.FarZombieSounds.Count)];
            }

            if (RoundManager.Instance.IsRoundSpecial)
                soundPoolable.AudioSource.volume = .4f;
            else
                soundPoolable.AudioSource.volume = 1f;

            soundPoolable.AudioSource.Play();

            Invoke("PlayRandomSound", Random.Range(4f, 6f));
        }
    }
}
#endif