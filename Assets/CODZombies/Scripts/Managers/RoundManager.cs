#if H3VR_IMPORTED
using System;
using System.Collections;
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

        public int ZombieFastWalkRound = 2;
        public int ZombieRunRound = 6;
        public int HardModeFastWalkRound = 0;
        public int HardModeRunRound = 3;
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

        public bool IsFastWalking
        {
            get
            {
                if (GameSettings.HardMode)
                    return RoundNumber >= HardModeFastWalkRound;
                return RoundNumber >= ZombieFastWalkRound;
            }
        }

        public bool IsRunning
        {
            get
            {
                if (GameSettings.HardMode)
                    return RoundNumber >= HardModeRunRound;
                return RoundNumber >= ZombieRunRound;
            }
        }

        public void SendStartGame()
        {
            StartGame();
            CodZNetworking.Instance.StartGame_Send();
        }
        
        public void StartGame()
        {
            Debug.Log("Starting game");
            if (!Application.isEditor)
            {
                GM.CurrentMovementManager.TeleportToPoint(GMgr.Instance.StartGameWaypoint.position, true);
            }

            GMgr.Instance.GameStarted = true;

            RoundNumber = 0;

            AdvanceRound();

            if (OnGameStarted != null)
                OnGameStarted.Invoke();
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