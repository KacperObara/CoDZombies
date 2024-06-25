#if H3VR_IMPORTED
using System;
using System.Collections;
using CustomScripts.Gamemode;
using CustomScripts.Managers;
using CustomScripts.Multiplayer;
using FistVR;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomScripts
{
    public class RoundManager : MonoBehaviourSingleton<RoundManager>
    {
        public static Action RoundStarted;
        public static Action RoundEnded;

        public static Action OnRoundChanged;
        public static Action OnZombiesLeftChanged;
        public static Action<GameObject> OnZombieKilled;

        public static Action OnGameStarted;
        
        public int SpecialRoundInterval;

        [HideInInspector] public int RoundNumber = 0;

        private Coroutine _roundDelayCoroutine;

        public bool IsRoundSpecial
        {
            get
            {
                if (GameSettings.SpecialRoundDisabled) return false;
                return RoundNumber % SpecialRoundInterval == 0;
            }
        }

        public void SendStartGame()
        {
            StartGame();
            CodZNetworking.Instance.StartGame_Send();
        }
        
        public void StartGame()
        {
            if (!Application.isEditor)
            {
                PlayerSpawner.Instance.SpawnPlayer();
            }

            GMgr.Instance.GameStarted = true;

            RoundNumber = 0;

            
            if (OnGameStarted != null)
                OnGameStarted.Invoke();
            
            AdvanceRound();
        }

        public void AdvanceRound()
        {
            if (GMgr.Instance.GameEnded)
                return;

            RoundNumber++;

            ZombieManager.Instance.BeginSpawningEnemies();

            if (OnZombiesLeftChanged != null)
                OnZombiesLeftChanged.Invoke();
            if (OnRoundChanged != null)
                OnRoundChanged.Invoke();
        }

        public void EndRound()
        {
            AudioManager.Instance.Play(AudioManager.Instance.RoundEndSound, 0.2f, 1f);

            if (RoundEnded != null)
                RoundEnded.Invoke();

            _roundDelayCoroutine = StartCoroutine(DelayedAdvanceRound(17f));
        }

        private IEnumerator DelayedAdvanceRound(float delay)
        {
            yield return new WaitForSeconds(delay);

            AdvanceRound();

            if (RoundStarted != null)
                RoundStarted.Invoke();
        }

        public void PauseGame()
        {
            StopCoroutine(_roundDelayCoroutine);
        }

        public void ResumeGame()
        {
            _roundDelayCoroutine = StartCoroutine(DelayedAdvanceRound(0f));
        }
    }
}
#endif