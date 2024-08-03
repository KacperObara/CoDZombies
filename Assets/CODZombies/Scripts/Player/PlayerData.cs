#if H3VR_IMPORTED
using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts.Gamemode;
using CustomScripts.Managers;
using CustomScripts.Multiplayer;
using CustomScripts.Zombie;
using FistVR;
using HarmonyLib;
using UnityEngine;
using Valve.VR;

namespace CustomScripts.Player
{
    public class PlayerData : MonoBehaviourSingleton<PlayerData>
    {
        public static Action GettingHitEvent;

        [HideInInspector] public List<PerkShop> UsedPerkShops = new List<PerkShop>();
        public List<GameObject> PlayerPerkIcons;

        public PowerUpIndicator DoublePointsPowerUpIndicator;
        public PowerUpIndicator InstaKillPowerUpIndicator;
        public PowerUpIndicator DeathMachinePowerUpIndicator;

        public float DamageModifier = 1f;
        [HideInInspector] public float MoneyModifier = 1f;
        
        private float _currentSpeedMult = 1f;

        [HideInInspector] public bool InstaKill;

        [HideInInspector] public bool DeadShotPerkActivated;
        [HideInInspector] public bool DoubleTapPerkActivated;
        [HideInInspector] public bool SpeedColaPerkActivated;
        [HideInInspector] public bool QuickRevivePerkActivated;
        [HideInInspector] public bool StaminUpPerkActivated;
        [HideInInspector] public bool PHDFlopperPerkActivated;
        [HideInInspector] public bool ElectricCherryPerkActivated;

        public FVRViveHand LeftHand
        {
            get { return GM.CurrentMovementManager.Hands[0]; }
        }

        public FVRViveHand RightHand
        {
            get { return GM.CurrentMovementManager.Hands[1]; }
        }

        public override void Awake()
        {
            base.Awake();

            RoundManager.OnRoundChanged += OnRoundAdvance;

            ResetPerks();
        }

        public void ResetPerks()
        {
            DeadShotPerkActivated = false;
            DoubleTapPerkActivated = false;
            SpeedColaPerkActivated = false;
            QuickRevivePerkActivated = false;
            StaminUpPerkActivated = false;
            PHDFlopperPerkActivated = false;
            ElectricCherryPerkActivated = false;
            
            UsedPerkShops.Clear();
            
            GM.CurrentPlayerBody.SetHealthThreshold(5000);
            GM.CurrentPlayerBody.ResetHealth();
            
            GM.CurrentSceneSettings.MaxSpeedClamp = 3f;

            foreach (var icon in PlayerPerkIcons)
            {
                icon.SetActive(false);
            }
        }

        private void OnRoundAdvance()
        {
            GM.CurrentPlayerBody.HealPercent(1f);
        }

        [HarmonyPatch(typeof(FVRPlayerHitbox), "Damage", new Type[] { typeof(Damage) })]
        [HarmonyPrefix]
        private static void OnBeforePlayerHit(Damage d)
        {
            //Debug.Log("I'm hit!: " + d.Source_IFF + "  " + d.Class);
            // Disable FriendyFire
            // foreach (var player in PlayersMgr.Instance.Players)
            // {
            //     //TODO don't hardcode number 5 when assigning IFFs
            //     if (!player.IsMe && player.PlayerManager.ID + 5 == d.Source_IFF)
            //     {
            //         d.Dam_Blunt = 0;
            //         d.Dam_Piercing = 0;
            //         d.Dam_Cutting = 0;
            //         d.Dam_TotalKinetic = 0;
            //         d.Dam_Thermal = 0;
            //         d.Dam_Chilling = 0;
            //         d.Dam_EMP = 0;
            //         d.Dam_TotalEnergetic = 0;
            //         d.Dam_Stunning = 0;
            //         d.Dam_Blinding = 0;
            //         d.damageSize = 0;
            //     }
            // }
            // if (d.Source_IFF != 1)
            // {
            //     d.Dam_Blunt = 0;
            //     d.Dam_Piercing = 0;
            //     d.Dam_Cutting = 0;
            //     d.Dam_TotalKinetic = 0;
            //     d.Dam_Thermal = 0;
            //     d.Dam_Chilling = 0;
            //     d.Dam_EMP = 0;
            //     d.Dam_TotalEnergetic = 0;
            //     d.Dam_Stunning = 0;
            //     d.Dam_Blinding = 0;
            //     d.damageSize = 0;
            // }
            
            if (d.Class == Damage.DamageClass.Explosive)
            {
                if (Instance.PHDFlopperPerkActivated)
                {
                    d.Dam_TotalKinetic *= .05f; //.3f
                    d.Dam_TotalEnergetic *= .05f; //.3f
                }
                else
                {
                    d.Dam_TotalKinetic *= .5f; //.3f
                    d.Dam_TotalEnergetic *= .5f; //.3f
                }
            }
            
            if (d.Source_IFF != GM.CurrentPlayerBody.GetPlayerIFF() && GettingHitEvent != null)
                GettingHitEvent.Invoke();
        }


        /// <summary>
        /// Place in which weapon or magazine wrapper classes are added to the objects
        /// </summary>
        [HarmonyPatch(typeof(FVRPhysicalObject), "BeginInteraction")]
        [HarmonyPrefix]
        private static void OnPhysicalObjectStartInteraction(FVRPhysicalObject __instance, FVRViveHand hand)
        {
            if (__instance as FVRFireArm)
            {
                WeaponWrapper wrapper = __instance.GetComponent<WeaponWrapper>();
                if (wrapper == null)
                {
                    wrapper = __instance.gameObject.AddComponent<WeaponWrapper>();
                    wrapper.Initialize((FVRFireArm) __instance);
                }
                
                wrapper.OnWeaponGrabbed();
            }
            else if (__instance as FVRFireArmMagazine)
            {
                MagazineWrapper wrapper = __instance.GetComponent<MagazineWrapper>();
                if (wrapper == null)
                {
                    wrapper = __instance.gameObject.AddComponent<MagazineWrapper>();
                    wrapper.Initialize((FVRFireArmMagazine) __instance);
                }
            }
        }
        
        [HarmonyPatch(typeof(FVRFireArm), "Awake")]
        [HarmonyPostfix]
        private static void OnWeaponSpawned(FVRFireArm __instance)
        {
            WeaponWrapper wrapper = __instance.gameObject.AddComponent<WeaponWrapper>();
            wrapper.Initialize(__instance);
            __instance.IsPickUpLocked = true;
        }

        [HarmonyPatch(typeof(FVRFireArmMagazine), "Release")]
        [HarmonyPostfix]
        private static void OnMagRelease(FVRFireArmMagazine __instance, bool PhysicalRelease = false)
        {
            //////// Electric cherry
            if (Instance.ElectricCherryPerkActivated && __instance.m_numRounds == 0)
            {
                if (!Instance.stunThrottle)
                    Instance.StartCoroutine(Instance.ActivateStun());
            }
        }

        private bool stunThrottle = false;

        public IEnumerator ActivateStun()
        {
            stunThrottle = true;

            for (int i = 0; i < ZombieManager.Instance.ExistingZombies.Count; i++)
            {
                (ZombieManager.Instance.ExistingZombies[i] as ZosigZombieController).Stun(2f);
            }

            yield return new WaitForSeconds(5f);
            stunThrottle = false;
        }
        
        
        private void OnDestroy()
        {
            RoundManager.OnRoundChanged -= OnRoundAdvance;
        }
    }
}
#endif