#if H3VR_IMPORTED
using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts.Multiplayer;
using CustomScripts.Powerups;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomScripts
{
    public class PowerUpManager : MonoBehaviourSingleton<PowerUpManager>
    {
        public float ChanceForAmmo = 2.5f;
        public float ChanceForPowerUp = 6f;

        public float AmmoCooldownTime;
        public float PowerUpCooldownTime;

        public PowerUpMaxAmmo MaxAmmo;
        public List<PowerUp> PowerUps;

        private readonly List<int> _randomIndexes = new List<int>();

        private bool _isPowerUpCooldown;
        private bool _isMaxAmmoCooldown;
        
        public int GetIndexOf(PowerUp powerUp) { return PowerUps.IndexOf(powerUp); }
        
        private void Start()
        {
            if (Networking.IsHostOrSolo())
            {
                RoundManager.OnZombieKilled -= RollForPowerUp;
                RoundManager.OnZombieKilled += RollForPowerUp;

                ShuffleIndexes();
            }
        }

        private void OnDestroy()
        {
            RoundManager.OnZombieKilled -= RollForPowerUp;
        }

        public void RollForPowerUp(GameObject spawnPos)
        {
            if (Networking.ServerRunning() && Networking.IsClient())
                return;
            
            // // Chance for Max Ammo
            //float chance = Random.Range(0f, 100f);
            // if (GameSettings.LimitedAmmo && !_isMaxAmmoCooldown)
            // {
            //     if (chance < ChanceForAmmo)
            //     {
            //         StartCoroutine(PowerUpMaxAmmoCooldown());
            //         SpawnPowerUp(MaxAmmo, spawnPos.transform.position);
            //         return;
            //     }
            // }

            if (_isPowerUpCooldown)
                return;

            // Chance for other power ups
            float chance = Random.Range(0f, 100f);
            if (chance < ChanceForPowerUp)
            {
                if (_randomIndexes.Count == 0)
                    ShuffleIndexes();
                
                SpawnPowerUp(PowerUps[_randomIndexes[0]], spawnPos.transform.position);
                CodZNetworking.Instance.PowerUpSpawned_Send(_randomIndexes[0], spawnPos.transform.position);

                _randomIndexes.RemoveAt(0);
            }
        }

        /// <summary>
        /// Power ups have randomized order. If one spawns,
        /// it cannot spawn again before all others have spawned too
        /// </summary>
        private void ShuffleIndexes()
        {
            _randomIndexes.Clear();

            for (int i = 0; i < PowerUps.Count; i++)
            {
                _randomIndexes.Add(i);
            }

            _randomIndexes.Shuffle();
        }

        public void SpawnPowerUp(PowerUp powerUp, Vector3 pos)
        {
            if (powerUp == null)
            {
                Debug.LogWarning("PowerUp spawn failed! PowerUp == null");
                return;
            }

            StartCoroutine(PowerUpCooldown());
            powerUp.Spawn(pos + Vector3.up);
        }
        
        // public void Collect(int powerUpID)
        // {
        //     if (Networking.IsHostOrSolo())
        //     {
        //         PowerUps[powerUpID].ApplyModifier();
        //         CodZNetworking.Instance.PowerUpCollected_Send(powerUpID);
        //     }
        //     else
        //     {
        //         CodZNetworking.Instance.Client_PowerUpCollected_Send(powerUpID);
        //     }
        // }

        private IEnumerator PowerUpCooldown()
        {
            _isPowerUpCooldown = true;
            yield return new WaitForSeconds(PowerUpCooldownTime);
            _isPowerUpCooldown = false;
        }
        
        // private IEnumerator PowerUpMaxAmmoCooldown()
        // {
        //     _isMaxAmmoCooldown = true;
        //     yield return new WaitForSeconds(AmmoCooldownTime);
        //     _isMaxAmmoCooldown = false;
        // }
    }
}
#endif