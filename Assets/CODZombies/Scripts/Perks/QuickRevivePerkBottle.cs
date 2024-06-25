using CustomScripts.Player;
using FistVR;
using UnityEngine;

namespace CustomScripts.Powerups.Perks
{
    public class QuickRevivePerkBottle : PerkBottle
    {
        public override void ApplyModifier()
        {
            base.ApplyModifier();
            PlayerData.Instance.QuickRevivePerkActivated = true;
            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            GetComponentInParent<FVRPhysicalObject>().ForceBreakInteraction();
            transform.parent.position = OriginPos;
        }
    }
}