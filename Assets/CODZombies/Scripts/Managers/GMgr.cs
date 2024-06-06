#if H3VR_IMPORTED
using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts.Gamemode;
using CustomScripts.Managers;
using CustomScripts.Multiplayer;
using CustomScripts.Objects;
using CustomScripts.Player;
using CustomScripts.Zombie;
using FistVR;
using HarmonyLib;
using UnityEngine;

namespace CustomScripts
{
    public class GMgr : MonoBehaviourSingleton<GMgr>
    {
        public static Action OnPointsChanged;
        public static Action OnPowerEnabled;

        [HideInInspector] public List<Blockade> Blockades;
        
        [HideInInspector] public int Points;
        [HideInInspector] public int TotalPoints; // for highscore

        [HideInInspector] public bool GameStarted = false;
        [HideInInspector] public bool GameEnded = false;

        public bool PowerEnabled;

        [HideInInspector]public int Kills;

        private Harmony _harmony;

        public override void Awake()
        {
            base.Awake();

            _harmony = Harmony.CreateAndPatchAll(typeof (PlayerData), (string) null);
            _harmony.PatchAll(typeof (PlayerSpawner));
            _harmony.PatchAll(typeof (ZombieManager));
        }

        public void PowerLeverPulled()
        {
            if (Networking.IsHost())
            {
                CodZNetworking.Instance.CustomData_Send((int)CustomDataType.POWER_ENABLED);
            }
            else
            {
                CodZNetworking.Instance.Client_CustomData_Send((int)CustomDataType.POWER_ENABLED);
            }
        }
        
        public void TurnOnPower()
        {
            if (PowerEnabled)
                return;

            PowerEnabled = true;
            AudioManager.Instance.Play(AudioManager.Instance.PowerOnSound, .8f);
            if (OnPowerEnabled != null)
                OnPowerEnabled.Invoke();
        }

        public void AddPoints(int amount)
        {
            float newAmount = amount * PlayerData.Instance.MoneyModifier;

            PlayerData.Instance.MoneyModifier.ToString();

            amount = (int) newAmount;

            Points += amount;
            TotalPoints += amount;

            if (OnPointsChanged != null)
                OnPointsChanged.Invoke();
        }

        public bool TryRemovePoints(int amount)
        {
            if (Points >= amount)
            {
                Points -= amount;

                if (OnPointsChanged != null)
                    OnPointsChanged.Invoke();
                return true;
            }

            return false;
        }

        public void KillPlayer()
        {
            if (GameEnded)
                return;

            GM.CurrentPlayerBody.KillPlayer(false);
        }

        public void EndGame()
        {
            if (GameEnded)
                return;

            GameEnded = true;

            AudioManager.Instance.PlayMusic(AudioManager.Instance.EndMusic, 0.25f, 1f);

            EndPanel.Instance.UpdatePanel();
        }

        private void OnDestroy()
        {
            _harmony.UnpatchSelf();
        }
    }
}
#endif