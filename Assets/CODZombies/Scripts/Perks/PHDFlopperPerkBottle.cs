using CustomScripts.Player;
using FistVR;
using UnityEngine;

namespace CustomScripts.Powerups.Perks
{
    public class PHDFlopperPerkBottle : MonoBehaviour, IModifier
    {
        private Vector3 _originalPosition;
        
        private void Start()
        {
            _originalPosition = transform.parent.position;
        }
        
        public void ApplyModifier()
        {
            PlayerData.Instance.PHDFlopperPerkActivated = true;
            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            GetComponent<FVRPhysicalObject>().ForceBreakInteraction();
            transform.parent.position = _originalPosition;
        }
    }
}