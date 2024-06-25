using System;
using CustomScripts.Powerups.Perks;
using FistVR;
using UnityEngine;

namespace CustomScripts
{
    public class JuggerNogPerkBottle: PerkBottle
    {
        public static Action ConsumedEvent;

        public float NewHealth = 10000;
        
        public override void ApplyModifier()
        {
            base.ApplyModifier();
            GM.CurrentPlayerBody.SetHealthThreshold(NewHealth);
            GM.CurrentPlayerBody.ResetHealth();

            if (ConsumedEvent != null)
                ConsumedEvent.Invoke();

            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            GetComponentInParent<FVRPhysicalObject>().ForceBreakInteraction();
            transform.parent.position = OriginPos;
        }
    }
}