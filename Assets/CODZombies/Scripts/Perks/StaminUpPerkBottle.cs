using System;
using CustomScripts.Player;
using FistVR;
using UnityEngine;

namespace CustomScripts.Powerups.Perks
{
    public class StaminUpPerkBottle : PerkBottle
    {
        public static Action ConsumedEvent;
        
        public override void ApplyModifier()
        {
            base.ApplyModifier();
            PlayerData.Instance.StaminUpPerkActivated = true;
            GM.CurrentSceneSettings.MaxSpeedClamp = 4f;

            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            
            GetComponentInParent<FVRPhysicalObject>().ForceBreakInteraction();
            transform.parent.position = OriginPos;
        }
    }
}