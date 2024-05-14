using System;
using FistVR;
using UnityEngine;

namespace CustomScripts
{
    public class JuggerNogPerkBottle: MonoBehaviour, IModifier
    {
        public static Action ConsumedEvent;

        public float NewHealth = 10000;

        public void ApplyModifier()
        {
            GM.CurrentPlayerBody.SetHealthThreshold(NewHealth);
            GM.CurrentPlayerBody.ResetHealth();

            if (ConsumedEvent != null)
                ConsumedEvent.Invoke();

            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            Destroy(gameObject);
        }
    }
}