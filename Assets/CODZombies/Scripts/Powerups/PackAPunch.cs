using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomScripts.Gamemode;
using CustomScripts.Gamemode.GMDebug;
using CustomScripts.Multiplayer;
using CustomScripts.Objects.Weapons;
using FistVR;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace CustomScripts.Powerups
{
    // Player puts his hand into the machine. If the player is holding a weapon, the weapon is PackAPunched.
    // In multiplayer, if client, then destroy gun and send data to host to spawn the weapon.
    public class PackAPunch : MonoBehaviour, IPurchasable, IRequiresPower
    {
        public static Action PurchaseEvent;

        public int Cost;
        public int PurchaseCost { get { return Cost; } }

        [SerializeField] private bool _isOneTimeOnly;
        public bool IsOneTimeOnly { get { return _isOneTimeOnly; } }

        private bool _alreadyBought;
        public bool AlreadyBought { get { return _alreadyBought; } }
        public bool IsPowered { get { return GMgr.Instance.PowerEnabled; } }

        public List<WeaponData> WeaponsData;
        public List<CustomItemSpawner> Spawners;

        public AudioClip UpgradeSound;
        [HideInInspector] public bool InUse = false;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<FVRPhysicalObject>())
            {
                FVRPhysicalObject fvrPhysicalObject = other.GetComponent<FVRPhysicalObject>();
                if (fvrPhysicalObject.ObjectWrapper == null)
                    return;
                
                bool isMeHolding = fvrPhysicalObject.m_hand == GM.CurrentMovementManager.Hands[0] || GM.CurrentMovementManager.Hands[1];
                
                if (isMeHolding && CanBuy(fvrPhysicalObject))
                {
                    Buy(fvrPhysicalObject);
                }
            }
        }

        private bool CanBuy(FVRPhysicalObject fvrPhysicalObject)
        {
            WeaponWrapper weaponWrapper = fvrPhysicalObject.GetComponent<WeaponWrapper>();

            if (fvrPhysicalObject as FVRFireArm == null)
                return false;
            
            // Disabling minigun since it could break the DeathMachine
            if (fvrPhysicalObject.ObjectWrapper.ItemID == "M134Minigun")
                return false;

            if (weaponWrapper == null)
                return false;
            if (weaponWrapper.PackAPunchDeactivated)
                return false;
            
            WeaponData weapon = WeaponsData.FirstOrDefault(x => x.Id == fvrPhysicalObject.ObjectWrapper.ItemID);

            if (weapon && !weapon.UpgradedWeapon)
                return false;

            if (!IsPowered || GMgr.Instance.Points < Cost || weapon.UpgradedWeapon == null)
                return false;

            return true;
        }
        
        public void Buy(FVRPhysicalObject fvrPhysicalObject)
        {
            WeaponData weapon = WeaponsData.FirstOrDefault(x => x.Id == fvrPhysicalObject.ObjectWrapper.ItemID);
            
            _alreadyBought = true;
            GMgr.Instance.RemovePoints(Cost);

            fvrPhysicalObject.ForceBreakInteraction();
            fvrPhysicalObject.IsPickUpLocked = true;
            Destroy(fvrPhysicalObject.gameObject);

            if (Networking.IsHostOrSolo())
            {
                SpawnWeapon(weapon.Id);
                OnBuying();
                CodZNetworking.Instance.PaPPurchased_Send();
            }
            else
            {
                CodZNetworking.Instance.Client_PaPPurchased_Send(weapon.Id);
            }
            
            if (PurchaseEvent != null)
                PurchaseEvent.Invoke();
        }

        public void OnBuying()
        {
            if (InUse)
                return;
            InUse = true;
            
            AudioManager.Instance.Play(AudioManager.Instance.BuySound, .5f);
            AudioManager.Instance.Play(UpgradeSound, .3f);
        }
        
        public void SpawnWeapon(string weaponId)
        {
            WeaponData weapon = WeaponsData.FirstOrDefault(x => x.Id == weaponId);
            StartCoroutine(DelayedSpawn(weapon));
        }

        private IEnumerator DelayedSpawn(WeaponData weapon)
        {
            yield return new WaitForSeconds(5f);

            for (int i = 0; i < weapon.UpgradedWeapon.DefaultSpawners.Count; i++)
            {
                Spawners[i].ObjectId = weapon.UpgradedWeapon.DefaultSpawners[i];
                Spawners[i].Spawn();
            }

            InUse = false;
        }
    }
}