﻿using System;
using System.Collections;
using CustomScripts.Gamemode.GMDebug;
using CustomScripts.Managers;
using FistVR;
using Sodalite.Api;
using UnityEngine;

namespace CustomScripts
{
    public class CustomSosigSpawnPoint : MonoBehaviour
    {
        public SosigEnemyTemplate SosigEnemyTemplate;

        [Tooltip("Spawn when game starts?")]
        public bool SpawnOnStart = false;

        public int IFF = 1;
        public Sosig.SosigOrder SpawnState;

        public float SpawnDelay = 0;
        public ParticleSystem SpawnPS;

        private bool _spawnInterrupted = false;

        public IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            if (SpawnOnStart) Spawn();
        }

        public void Spawn()
        {
            ZombieManager.LocationChangedEvent += InterruptSpawn;
            StartCoroutine(DelayedSpawn());
        }

        private IEnumerator DelayedSpawn()
        {
            if (SpawnPS)
                SpawnPS.Play(true);
            if (RoundManager.Instance.IsRoundSpecial)
                AudioManager.Instance.Play(AudioManager.Instance.HellHoundSpawnSound, volume:.4f, delay:.25f);

            yield return new WaitForSeconds(SpawnDelay);

            ZombieManager.LocationChangedEvent -= InterruptSpawn;
            if (_spawnInterrupted)
            {
                _spawnInterrupted = false;
                yield break;
            }

            try
            {
                SosigAPI.SpawnOptions options = new SosigAPI.SpawnOptions
                {
                    SpawnActivated = true,
                    SpawnState = SpawnState,
                    IFF = IFF,
                    SpawnWithFullAmmo = true,
                    EquipmentMode = SosigAPI.SpawnOptions.EquipmentSlots.All,
                    SosigTargetPosition = transform.position,
                    SosigTargetRotation = transform.eulerAngles
                };

                Sosig spawnedSosig = SosigAPI.Spawn(SosigEnemyTemplate, options, transform.position, transform.rotation);
                ZombieManager.Instance.OnZosigSpawned(spawnedSosig);
            }
            catch (Exception e)
            {
                ErrorShower.Instance.Show("Error, zosigs failed to spawn\nPlease send logs to Kodeman");
                Debug.LogError(e);
                throw;
            }
        }

        private void InterruptSpawn()
        {
            _spawnInterrupted = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.8f, 0.2f, 0.2f, 0.5f);
            Gizmos.DrawSphere(transform.position, 0.1f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.25f);
        }
    }
}