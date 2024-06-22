using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts.Multiplayer;
using CustomScripts.Player;
using FistVR;
using H3MP;
using HarmonyLib;
using UnityEngine;
using Valve.VR;

namespace CustomScripts.Gamemode
{
    public class PlayerSpawner : MonoBehaviourSingleton<PlayerSpawner>
    {
        public static Action BeingRevivedEvent;

        public Transform EndGameSpawnerPos; // On game end
        public Transform RespawnPos; // On spawn on respawn next round
        public Vector3 DownedPos; // On downed
        
        public string StartingWeapon = "M1911";
        public string StartingAmmo = "MagazineM1911";
        
       

        private IEnumerator Start()
        {
            // Wait one frame so that everything is all setup
            yield return null;
            GM.CurrentSceneSettings.DeathResetPoint = transform;
            GM.CurrentMovementManager.TeleportToPoint(transform.position, true, transform.position + transform.forward);
        }
        
        [HarmonyPatch(typeof(GM), "BringBackPlayer")]
        [HarmonyPrefix]
        private static void BeforePlayerDeath()
        {
            Instance.DownedPos = GM.CurrentPlayerBody.transform.position;
        }
        
        public void SpawnPlayer()
        {
            GM.CurrentPlayerBody.HealPercent(1f);
            GM.CurrentPlayerBody.EnableHands();
            GM.CurrentPlayerBody.EnableHitBoxes();
            GM.CurrentPlayerBody.WipeQuickbeltContents();
            PlayersMgr.Me.IsDead = false;
            PlayersMgr.Me.IsDowned = false;
            GM.CurrentMovementManager.TeleportToPoint(RespawnPos.position, true);
            SteamVR_Fade.Start(Color.clear, 0.0f); 
            SpawnStartingLoadout();

            RoundManager.OnRoundChanged -= SpawnPlayer;
        }
        
        private void SpawnStartingLoadout()
        {
            //Equip starting weapon
            FVRObject obj = null;
            if (!IM.OD.TryGetValue(StartingWeapon, out obj))
            {
                Debug.LogError("No object found with id: " + StartingWeapon);
            }
            var callback = obj.GetGameObject();
            FVRPhysicalObject physicalObject = Instantiate(callback, transform.position + Vector3.up, transform.rotation).GetComponent<FVRPhysicalObject>();
            physicalObject.gameObject.SetActive(true);
            
            physicalObject.ForceObjectIntoInventorySlot(GM.CurrentPlayerBody.QBSlots_Internal[0]);
            physicalObject.GetComponent<WeaponWrapper>().SetOwner(GameManager.ID);
            
            //Equip starting ammo
            obj = null;
            if (!IM.OD.TryGetValue(StartingAmmo, out obj))
            {
                Debug.LogError("No object found with id: " + StartingAmmo);
            }
            callback = obj.GetGameObject();
            physicalObject = Instantiate(callback, transform.position + Vector3.up, transform.rotation).GetComponent<FVRPhysicalObject>();
            physicalObject.gameObject.SetActive(true);
            
            physicalObject.ForceObjectIntoInventorySlot(GM.CurrentPlayerBody.QBSlots_Internal[1]);
            physicalObject.m_isSpawnLock = true;
        }
        
        [HarmonyPatch(typeof(GM), "BringBackPlayer")]
        [HarmonyPostfix]
        private static void AfterPlayerDeath()
        {
            Instance.OnPlayerDowned();
        }
        
        public void OnPlayerDowned()
        {
            if (Networking.ServerRunning() && PlayersMgr.Instance.Players.Count > 1)
            {
                // Put into revive state
                PlayersMgr.Me.IsDowned = true;
                GM.CurrentMovementManager.TeleportToPoint(DownedPos, true, transform.position + transform.forward);
                GM.CurrentPlayerBody.HealPercent(1f);
                
                GM.CurrentPlayerBody.DisableHands();
                GM.CurrentPlayerBody.DisableHitBoxes();
                SteamVR_Fade.Start(new Color(0.5f, 0, 0, 0.2f), 0.25f);


                if (Networking.IsHost())
                {
                    CodZNetworking.Instance.CustomData_PlayerID_Send(GameManager.ID, (int)CustomPlayerDataType.PLAYER_DOWNED);
                }
                else
                {
                    CodZNetworking.Instance.Client_CustomData_PlayerID_Send(GameManager.ID, (int)CustomPlayerDataType.PLAYER_DOWNED);
                }
            }
            else
            {
                if (PlayerData.Instance.QuickRevivePerkActivated)
                {
                    PlayerData.Instance.QuickRevivePerkActivated = false;
                    GM.CurrentMovementManager.TeleportToPoint(DownedPos, true, transform.position + transform.forward);
                    GM.CurrentPlayerBody.HealPercent(1f);

                    if (BeingRevivedEvent != null)
                        BeingRevivedEvent.Invoke();
                }
                else
                {
                    MoveToEndGameArea();
                }
            }
        }

        public void Revive()
        {
            PlayersMgr.Me.IsDowned = false;
            GM.CurrentPlayerBody.EnableHands();
            GM.CurrentPlayerBody.EnableHitBoxes();
            
            SteamVR_Fade.Start(Color.clear, 0.25f);
            
            if (BeingRevivedEvent != null)
                BeingRevivedEvent.Invoke();
        }

        public void DieFully()
        {
            PlayersMgr.Me.IsDead = true;
            PlayersMgr.Me.IsDowned = false;
            GM.CurrentPlayerBody.WipeQuickbeltContents();
            SteamVR_Fade.Start(new Color(0, 0, 0, 0.2f), 0.25f);
            RoundManager.OnRoundChanged += SpawnPlayer;
            
            if (Networking.IsHost())
            {
                CodZNetworking.Instance.CustomData_PlayerID_Send(GameManager.ID, (int)CustomPlayerDataType.PLAYER_DEAD);
            }
            else
            {
                CodZNetworking.Instance.Client_CustomData_PlayerID_Send(GameManager.ID, (int)CustomPlayerDataType.PLAYER_DEAD);
            }
        }
        
        public void MoveToEndGameArea()
        {
            GM.CurrentMovementManager.TeleportToPoint(EndGameSpawnerPos.position, true, transform.position + transform.forward);
            GM.CurrentPlayerBody.EnableHands();
            GM.CurrentPlayerBody.EnableHitBoxes();
            SteamVR_Fade.Start(Color.clear, 0.25f);
            GMgr.Instance.EndGame();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.5f);
            Gizmos.DrawSphere(transform.position, 0.1f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.25f);
        }

        private void OnDestroy()
        {
            RoundManager.OnRoundChanged -= SpawnPlayer;
        }
    }
}