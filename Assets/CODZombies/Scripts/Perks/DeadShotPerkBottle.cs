using CustomScripts.Managers;
using CustomScripts.Player;
using CustomScripts.Zombie;
using UnityEngine;

namespace CustomScripts
{
    public class DeadShotPerkBottle : MonoBehaviour, IModifier
    {
        public void ApplyModifier()
        {
            PlayerData.Instance.DeadShotPerkActivated = true;

            for (int i = 0; i < ZombieManager.Instance.ExistingZombies.Count; i++)
            {
                (ZombieManager.Instance.ExistingZombies[i] as ZosigZombieController).CheckPerks();
            }
            
            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            Destroy(transform.parent.gameObject);
        }
    }
}