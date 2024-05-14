#if H3VR_IMPORTED
using System;
using System.Collections;
using System.Collections.Generic;
using Atlas.MappingComponents.Sandbox;
using CustomScripts.Objects.Weapons;
using FistVR;
using UnityEngine;
using UnityEngine.UI;


namespace CustomScripts.Objects
{
    public class WallShop : MonoBehaviour, IPurchasable
    {
        public WeaponData Weapon;
        public int Cost;
        public int PurchaseCost { get { return Cost; } }
        
        
        [SerializeField] private bool _isOneTimeOnly;
        public bool IsOneTimeOnly { get { return _isOneTimeOnly; } }

        private bool _alreadyBought;
        public bool AlreadyBought { get { return _alreadyBought; } }

        public Text NameText;
        public Text CostText;

        public List<ObjectSpawnPoint> ItemSpawners;
        
        private void Start()
        {
            NameText.text = Weapon.DisplayName;
            CostText.text = Cost.ToString();
        }

        public void TryBuying()
        {
            if (GMgr.Instance.TryRemovePoints(Cost))
            {
                // spawning weapons in unlimited
                for (int i = 0; i < Weapon.DefaultSpawners.Count; i++)
                {
                    ItemSpawners[i].ObjectId = Weapon.DefaultSpawners[i];
                    ItemSpawners[i].Spawn();
                }

                if (!_alreadyBought)
                    _alreadyBought = true;
                AudioManager.Instance.Play(AudioManager.Instance.BuySound, .5f);
            }
        }
    }
}
#endif