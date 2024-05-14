#if H3VR_IMPORTED
using System;
using System.Collections;
using System.Collections.Generic;
using Atlas.MappingComponents.Sandbox;
using CustomScripts.Objects.Weapons;
using FistVR;
using UnityEngine;
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

        [HideInInspector] public List<WeaponData> LootId;
        [HideInInspector] public List<WeaponData> LimitedAmmoLootId;

        public ObjectSpawnPoint WeaponSpawner;
        public ObjectSpawnPoint AmmoSpawner;

        public AudioClip RollSound;

        [HideInInspector] public bool InUse = false;

        private MysteryBoxMover _mysteryBoxMover;

        private void Awake()
        {
            _mysteryBoxMover = GetComponent<MysteryBoxMover>();
        }

        public void SpawnWeapon()
        {
            if (InUse)
                return;

            if (!GMgr.Instance.TryRemovePoints(Cost))
                return;

            InUse = true;
            AudioManager.Instance.Play(RollSound, .25f);

            StartCoroutine(DelayedSpawn());
        }

        private IEnumerator DelayedSpawn()
        {
            yield return new WaitForSeconds(5.5f);

            if (_mysteryBoxMover.TryTeleport())
            {
                _mysteryBoxMover.StartTeleportAnim();
                GMgr.Instance.AddPoints(Cost);
            }
            else
            {
                int random = Random.Range(0, LootId.Count);
                WeaponData rolledWeapon = LootId[random];

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