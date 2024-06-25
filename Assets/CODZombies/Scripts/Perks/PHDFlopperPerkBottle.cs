using CustomScripts.Player;
using FistVR;
using UnityEngine;

namespace CustomScripts.Powerups.Perks
{
    public class PHDFlopperPerkBottle : PerkBottle
    {
        
        public override void ApplyModifier()
        {
            base.ApplyModifier();
            PlayerData.Instance.PHDFlopperPerkActivated = true;
            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            GetComponentInParent<FVRPhysicalObject>().ForceBreakInteraction();
            transform.parent.position = OriginPos;
        }
    }
}