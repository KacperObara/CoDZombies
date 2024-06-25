using System;
using FistVR;
using UnityEngine;

namespace CustomScripts
{
    public class JuggerNogPerkBottle: MonoBehaviour, IModifier
    {
        public static Action ConsumedEvent;

        public float NewHealth = 10000;

        private Vector3 _originalPosition;
        
        private void Start()
        {
            _originalPosition = transform.parent.position;
        }
        
        public void ApplyModifier()
        {
            GM.CurrentPlayerBody.SetHealthThreshold(NewHealth);
            GM.CurrentPlayerBody.ResetHealth();

            if (ConsumedEvent != null)
                ConsumedEvent.Invoke();

            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            GetComponent<FVRPhysicalObject>().ForceBreakInteraction();
            transform.parent.position = _originalPosition;
        }
    }
}