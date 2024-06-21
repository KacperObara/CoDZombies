#if H3VR_IMPORTED
using System;
using System.Collections;
using System.Collections.Generic;
using Atlas.MappingComponents.Sandbox;
using CustomScripts.Gamemode;
using CustomScripts.Gamemode.GMDebug;
using CustomScripts.Objects.Weapons;
using FistVR;
using H3MP;
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

        public CustomItemSpawner WeaponSpawner;
        public CustomItemSpawner AmmoSpawner;

        private void OnValidate()
        {
            UpdateUI();
        }

        private void Start()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            NameText.text = Weapon.DisplayName;
            CostText.text = Cost.ToString();
        }

        public void TryBuying()
        {
            if (GMgr.Instance.TryRemovePoints(Cost))
            {
                WeaponSpawner.ObjectId = Weapon.DefaultSpawners[0];
                GameObject weapon = WeaponSpawner.Spawn();
                
                AmmoSpawner.ObjectId = Weapon.DefaultSpawners[1];
                AmmoSpawner.Spawn();

                weapon.GetComponent<WeaponWrapper>().SetOwner(GameManager.ID);
                
                if (!_alreadyBought)
                    _alreadyBought = true;
                AudioManager.Instance.Play(AudioManager.Instance.BuySound, .5f);
            }
        }
    }
}
#endif