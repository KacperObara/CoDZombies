using System;
using System.Collections;
using CustomScripts.Multiplayer;
using CustomScripts.Player;
using FistVR;
using HarmonyLib;
using UnityEngine;

namespace CustomScripts.Gamemode
{
    public class PlayerSpawner : MonoBehaviourSingleton<PlayerSpawner>
    {
        public static Action BeingRevivedEvent;

        public Transform EndGameSpawnerPos;

        public override void Awake()
        {
            base.Awake();
            RoundManager.OnGameStarted += OnGameStarted;
        }

        private void OnGameStarted()
        {
            // When game starts, move respawn Pos to end game area
            transform.position = EndGameSpawnerPos.position;
        }

        private IEnumerator Start()
        {
            // Wait one frame so that everything is all setup
            yield return null;
            GM.CurrentSceneSettings.DeathResetPoint = transform;
            GM.CurrentMovementManager.TeleportToPoint(transform.position, true, transform.position + transform.forward);
        }
        
        // [HarmonyPatch(typeof(GM), "BringBackPlayer")]
        // [HarmonyPrefix]
        // private static void BeforePlayerDeath()
        // {
        //     //PlayerData.Instance.AfterPlayerDeath();
        // }

        // [HarmonyPatch(typeof(GM), "BringBackPlayer")]
        // [HarmonyPostfix]
        // private static void AfterPlayerDeath()
        // {
        //     PlayerData.Instance.AfterPlayerDeath();
        //     //Instance.transform.position = Instance.EndGameSpawnerPos.position;
        // }

        private void OnDestroy()
        {
            RoundManager.OnGameStarted -= OnGameStarted;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.5f);
            Gizmos.DrawSphere(transform.position, 0.1f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.25f);
        }
    }
}