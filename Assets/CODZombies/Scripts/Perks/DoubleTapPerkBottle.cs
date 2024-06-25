using CustomScripts.Gamemode;
using CustomScripts.Managers;
using CustomScripts.Player;
using CustomScripts.Powerups.Perks;
using CustomScripts.Zombie;
using FistVR;
using UnityEngine;

namespace CustomScripts
{
    public class DoubleTapPerkBottle : PerkBottle
    {
        public float DamageMultiplier = 1.5f;
        
        public override void ApplyModifier()
        {
            base.ApplyModifier();
            PlayerData.Instance.DoubleTapPerkActivated = true;
            
            for (int i = 0; i < ZombieManager.Instance.ExistingZombies.Count; i++)
            {
                (ZombieManager.Instance.ExistingZombies[i] as ZosigZombieController).CheckPerks();
            }

            FVRFireArm heldWeapon = PlayerData.Instance.LeftHand.CurrentInteractable as FVRFireArm;
            if (heldWeapon != null)
                heldWeapon.GetComponent<WeaponWrapper>().OnWeaponGrabbed();

            heldWeapon = PlayerData.Instance.RightHand.CurrentInteractable as FVRFireArm;
            if (heldWeapon != null)
                heldWeapon.GetComponent<WeaponWrapper>().OnWeaponGrabbed();


            PlayerData.Instance.DamageModifier *= DamageMultiplier;
            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            GetComponentInParent<FVRPhysicalObject>().ForceBreakInteraction();
            transform.parent.position = OriginPos;
        }
    }
}