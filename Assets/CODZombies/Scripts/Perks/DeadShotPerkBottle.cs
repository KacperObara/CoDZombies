using CustomScripts.Managers;
using CustomScripts.Player;
using CustomScripts.Zombie;
using FistVR;
using UnityEngine;

namespace CustomScripts
{
    public class DeadShotPerkBottle : MonoBehaviour, IModifier
    {
        private Vector3 _originalPosition;
        
        private void Start()
        {
            _originalPosition = transform.parent.position;
        }
        
        public void ApplyModifier()
        {
            PlayerData.Instance.DeadShotPerkActivated = true;

            for (int i = 0; i < ZombieManager.Instance.ExistingZombies.Count; i++)
            {
                (ZombieManager.Instance.ExistingZombies[i] as ZosigZombieController).CheckPerks();
            }
            
            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            
            GetComponent<FVRPhysicalObject>().ForceBreakInteraction();
            transform.parent.position = _originalPosition;
        }
    }
}