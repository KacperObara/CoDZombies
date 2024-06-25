using System;
using CustomScripts.Player;
using FistVR;
using UnityEngine;

namespace CustomScripts.Powerups.Perks
{
    public class StaminUpPerkBottle : MonoBehaviour, IModifier
    {
        public static Action ConsumedEvent;

        private Vector3 _originalPosition;
        
        private void Start()
        {
            _originalPosition = transform.parent.position;
        }

        public void ApplyModifier()
        {
            PlayerData.Instance.StaminUpPerkActivated = true;
            GM.CurrentSceneSettings.MaxSpeedClamp = 4f;

            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            
            GetComponent<FVRPhysicalObject>().ForceBreakInteraction();
            transform.parent.position = _originalPosition;
        }
    }
}