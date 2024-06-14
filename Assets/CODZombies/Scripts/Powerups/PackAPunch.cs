using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomScripts.Gamemode;
using CustomScripts.Gamemode.GMDebug;
using CustomScripts.Multiplayer;
using CustomScripts.Objects.Weapons;
using FistVR;
using H3MP;
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
            if (fvrPhysicalObject == null)
                return false;
            
            if (fvrPhysicalObject as FVRFireArm == null)
                return false;

            if (fvrPhysicalObject.ObjectWrapper == null)
                return false;
            
            // Disabling minigun since it could break the DeathMachine
            if (fvrPhysicalObject.ObjectWrapper.ItemID == "M134Minigun")
                return false;

            WeaponWrapper weaponWrapper = fvrPhysicalObject.GetComponent<WeaponWrapper>();
            
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
                CodZNetworking.Instance.PaPPurchased_Send();
                OnBuying();
            }
            else
            {
                CodZNetworking.Instance.Client_PaPPurchased_Send();
            }
            
            SpawnWeapon(weapon.Id, GameManager.ID);
            
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
        
        public void SpawnWeapon(string weaponId, int ownerId)
        {
            WeaponData weapon = WeaponsData.FirstOrDefault(x => x.Id == weaponId);
            StartCoroutine(DelayedSpawn(weapon, ownerId));
        }

        private IEnumerator DelayedSpawn(WeaponData weaponData, int ownerId)
        {
            yield return new WaitForSeconds(5f);

            Spawners[0].ObjectId = weaponData.UpgradedWeapon.DefaultSpawners[0];
            GameObject weapon = Spawners[0].Spawn();
            
            Spawners[1].ObjectId = weaponData.UpgradedWeapon.DefaultSpawners[1];
            Spawners[1].Spawn();
            
            WeaponWrapper weaponWrapper = weapon.GetComponent<WeaponWrapper>();
            if (weaponWrapper)
            {
                weaponWrapper.SetOwner(ownerId);
            }

            InUse = false;
        }
    }
}