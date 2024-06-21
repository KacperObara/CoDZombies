#if H3VR_IMPORTED
using System;
using System.Collections;
using System.Collections.Generic;
using Atlas.MappingComponents.Sandbox;
using CustomScripts.Gamemode;
using CustomScripts.Gamemode.GMDebug;
using CustomScripts.Multiplayer;
using CustomScripts.Objects.Weapons;
using FistVR;
using H3MP;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace CustomScripts
{
    public class MysteryBox : MonoBehaviour, IPurchasable
    {
        public static Action<WeaponData> WeaponSpawnedEvent;

        public int Cost = 950;
        public int PurchaseCost { get { return Cost; } }

        [SerializeField] private bool _isOneTimeOnly;
        public bool IsOneTimeOnly { get { return _isOneTimeOnly; } }

        private bool _alreadyBought;
        public bool AlreadyBought { get { return _alreadyBought; } }

        [FormerlySerializedAs("LootId")] public List<WeaponData> Loot;
        public List<WeaponData> RareLoot;
        [Range(0f, 1f)]
        public float RareChance = 0.05f;
        
        public CustomItemSpawner WeaponSpawner;
        public CustomItemSpawner AmmoSpawner;
        
        public AudioClip RollSound;

        [HideInInspector] public bool InUse = false;

        private MysteryBoxMover _mysteryBoxMover;

        public bool WillTeleport;
        public bool DidIRoll;
        
        private void Awake()
        {
            _mysteryBoxMover = GetComponent<MysteryBoxMover>();
        }

        public void TryBuy()
        {
            if (InUse)
                return;

            if (!GMgr.Instance.TryRemovePoints(Cost))
                return;

            DidIRoll = true;
            if (Networking.IsHost())
            {
                CodZNetworking.Instance.CustomData_Send((int)CustomDataType.MYSTERY_BOX_ROLLED);
            }
            else
            {
                CodZNetworking.Instance.Client_CustomData_Send((int)CustomDataType.MYSTERY_BOX_ROLLED);
            }
        }

        public void OnBuying()
        {
            InUse = true;
            WillTeleport = false;
            AudioManager.Instance.Play(RollSound, .25f);

            if (Networking.IsHost())
            {
                bool willTeleport = _mysteryBoxMover.CheckForTeleport();
                if (willTeleport)
                {
                    CodZNetworking.Instance.CustomData_Send((int)CustomDataType.MYSTERY_BOX_TELEPORT);
                }
            }
            
            StartCoroutine(DelayedSpawn());
        }

        private IEnumerator DelayedSpawn()
        {
            yield return new WaitForSeconds(5.5f);

            if (WillTeleport)
            {
                if (Networking.IsHost())
                {
                    _mysteryBoxMover.StartTeleporting();
                }
            }
            else
            {
                if (DidIRoll)
                {
                    DidIRoll = false;
                    bool isRare = Random.Range(0f, 1f) <= RareChance;

                    WeaponData rolledWeapon = null;
                    if (isRare)
                    {
                        int random = Random.Range(0, RareLoot.Count);
                        rolledWeapon = RareLoot[random];
                    }
                    else
                    {
                        int random = Random.Range(0, Loot.Count);
                        rolledWeapon = Loot[random];
                    }
                    
                    WeaponSpawner.ObjectId = rolledWeapon.DefaultSpawners[0];
                    AmmoSpawner.ObjectId = rolledWeapon.DefaultSpawners[1];

                    GameObject weapon = WeaponSpawner.Spawn();
                    AmmoSpawner.Spawn();
                
                    weapon.GetComponent<WeaponWrapper>().SetOwner(GameManager.ID);

                    if (WeaponSpawnedEvent != null)
                        WeaponSpawnedEvent.Invoke(rolledWeapon);
                }

                InUse = false;
            }

            if (Networking.IsHost())
            {
                _mysteryBoxMover.CurrentRoll++;
            }
        }
    }
}
#endif