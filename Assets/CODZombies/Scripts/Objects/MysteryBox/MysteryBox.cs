#if H3VR_IMPORTED
using System;
using System.Collections;
using System.Collections.Generic;
using Atlas.MappingComponents.Sandbox;
using CustomScripts.Multiplayer;
using CustomScripts.Objects.Weapons;
using FistVR;
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
        
        
        public ObjectSpawnPoint WeaponSpawner;
        public ObjectSpawnPoint AmmoSpawner;
        
        public AudioClip RollSound;

        [HideInInspector] public bool InUse = false;

        private MysteryBoxMover _mysteryBoxMover;

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

            if (Networking.IsHost())
            {
                //SpawnWeapon();
                CodZNetworking.Instance.CustomData_Send((int)CustomDataType.MYSTERY_BOX_BOUGHT);
            }
            else
            {
                CodZNetworking.Instance.Client_CustomData_Send((int)CustomDataType.MYSTERY_BOX_BOUGHT);
            }
        }

        public void SpawnWeapon()
        {
            InUse = true;
            AudioManager.Instance.Play(RollSound, .25f);
            
            StartCoroutine(DelayedSpawn());
        }

        private IEnumerator DelayedSpawn()
        {
            yield return new WaitForSeconds(5.5f);

            if (!Networking.IsHost())
            {
                InUse = false;
                yield break;
            }
            
            if (!_mysteryBoxMover.TryTeleport())
            {
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

                WeaponSpawner.Spawn();
                AmmoSpawner.Spawn();

                if (WeaponSpawnedEvent != null)
                    WeaponSpawnedEvent.Invoke(rolledWeapon);

                InUse = false;

                _mysteryBoxMover.CurrentRoll++;
            }
        }
    }
}
#endif