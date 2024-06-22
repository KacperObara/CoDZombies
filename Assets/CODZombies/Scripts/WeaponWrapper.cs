using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts.Multiplayer;
using CustomScripts.Player;
using FistVR;
using H3MP;
using UnityEngine;

namespace CustomScripts.Gamemode
{
    /// <summary>
    /// Weapon script that gets added to every weapon grabbed by the player
    /// </summary>
    public class WeaponWrapper : MonoBehaviour
    {
        public int OwnerId;
        
        public bool DoubleTapActivated = false;
        public bool SpeedColaActivated = false;

        private FVRFireArm weapon;

        public bool PackAPunchDeactivated = false;

        private float _despawnTimer;
        private float _despawnTime = 60f;

        public bool IsWeaponMine()
        {
            if (GameManager.ID == OwnerId)
                return true;

            if (Networking.IsSolo())
                return true;
            
            return false;
        }

        public void SetOwner(int ownerId)
        {
            OwnerId = ownerId;
            
            if (GameManager.ID == OwnerId || Networking.IsSolo())
                weapon.IsPickUpLocked = false;
        }

        public void Initialize(FVRFireArm weapon)
        {
            this.weapon = weapon;
            if (Networking.IsHostOrSolo())
                StartCoroutine(DespawnTimer());
        }

        private IEnumerator DespawnTimer()
        {
            while (weapon != null)
            {
                yield return new WaitForSeconds(1f);

                if (weapon.m_hand != null || weapon.QuickbeltSlot != null)
                {
                    _despawnTimer = 0f;
                    continue;
                }
                _despawnTimer += 5f;
                
                if (_despawnTimer >= _despawnTime)
                {
                    Destroy(gameObject);
                    yield break;
                }
            }
        }

        // Called when the weapon is in hand
        public void OnWeaponGrabbed()
        {
            if (!IsWeaponMine())
                return;
            
            if (!DoubleTapActivated && PlayerData.Instance.DoubleTapPerkActivated)
            {
                DoubleTapActivated = true;
                IncreaseFireRate(1.33f);
            }

            if (!SpeedColaActivated && PlayerData.Instance.SpeedColaPerkActivated)
            {
                SpeedColaActivated = true;
                AddSpeedColaEffect();
            }
        }

        public void IncreaseFireRate(float amount)
        {
            if (weapon as Handgun)
            {
                Handgun handgun = (Handgun) weapon;
                handgun.Slide.SpringStiffness *= amount;
                handgun.Slide.Speed_Forward *= amount;
                handgun.Slide.Speed_Rearward *= amount;
            }

            if (weapon as ClosedBoltWeapon)
            {
                ClosedBoltWeapon closedBolt = (ClosedBoltWeapon) weapon;
                closedBolt.Bolt.SpringStiffness *= amount;
                closedBolt.Bolt.Speed_Forward *= amount;
                closedBolt.Bolt.Speed_Rearward *= amount;
            }

            if (weapon as OpenBoltReceiver)
            {
                OpenBoltReceiver openBolt = (OpenBoltReceiver) weapon;
                openBolt.Bolt.BoltSpringStiffness *= amount;
                openBolt.Bolt.BoltSpeed_Forward *= amount;
                openBolt.Bolt.BoltSpeed_Rearward *= amount;
            }
        }

        private void AddSpeedColaEffect()
        {
            if (weapon as Handgun)
            {
                Handgun handgun = (Handgun) weapon;
                handgun.HasMagReleaseButton = true;
            }

            if (weapon as ClosedBoltWeapon)
            {
                ClosedBoltWeapon closedBolt = (ClosedBoltWeapon) weapon;
                closedBolt.HasMagReleaseButton = true;
            }

            if (weapon as OpenBoltReceiver)
            {
                OpenBoltReceiver openBolt = (OpenBoltReceiver) weapon;
                openBolt.HasMagReleaseButton = true;
            }
        }
        
    }
}