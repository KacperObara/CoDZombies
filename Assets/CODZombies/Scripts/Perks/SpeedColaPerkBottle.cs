using CustomScripts.Gamemode;
using CustomScripts.Player;
using FistVR;
using UnityEngine;

namespace CustomScripts.Powerups.Perks
{
    public class SpeedColaPerkBottle : MonoBehaviour, IModifier
    {
        private Vector3 _originalPosition;
        
        private void Start()
        {
            _originalPosition = transform.parent.position;
        }
        
        public void ApplyModifier()
        {
            PlayerData.Instance.SpeedColaPerkActivated = true;

            FVRFireArm heldWeapon = PlayerData.Instance.LeftHand.CurrentInteractable as FVRFireArm;
            if (heldWeapon != null)
                heldWeapon.GetComponent<WeaponWrapper>().OnWeaponGrabbed();

            heldWeapon = PlayerData.Instance.RightHand.CurrentInteractable as FVRFireArm;
            if (heldWeapon != null)
                heldWeapon.GetComponent<WeaponWrapper>().OnWeaponGrabbed();

            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            GetComponent<FVRPhysicalObject>().ForceBreakInteraction();
            transform.parent.position = _originalPosition;
        }
    }
}