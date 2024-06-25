using CustomScripts.Managers;
using CustomScripts.Player;
using CustomScripts.Powerups.Perks;
using CustomScripts.Zombie;
using FistVR;
using UnityEngine;

namespace CustomScripts
{
    public class DeadShotPerkBottle : PerkBottle
    {
        public override void ApplyModifier()
        {
            base.ApplyModifier();
            PlayerData.Instance.DeadShotPerkActivated = true;

            for (int i = 0; i < ZombieManager.Instance.ExistingZombies.Count; i++)
            {
                (ZombieManager.Instance.ExistingZombies[i] as ZosigZombieController).CheckPerks();
            }
            
            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            
            GetComponentInParent<FVRPhysicalObject>().ForceBreakInteraction();
            transform.parent.position = OriginPos;
        }
    }
}