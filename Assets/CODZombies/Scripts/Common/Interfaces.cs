#if H3VR_IMPORTED

using CustomScripts.Multiplayer;
using CustomScripts.Zombie;
using H3MP;
using UnityEngine;
using UnityEngine.UI;

namespace CustomScripts
{
    // TODO Solve this mess that was created by multiplayer changes
    
    // Juggernog, DoubleTap etc.
    public interface IModifier
    {
        void ApplyModifier();
    }

    // Insta kill, double points etc.
    public interface IPowerUp : IModifier
    {
        void Spawn(Vector3 pos);
    }

    // Needed, because I can't serialize interfaces
    public abstract class PowerUp : MonoBehaviour, IPowerUp
    {
        // Sync Data and Activate
        public abstract void ApplyModifier();
        // Activate
        public abstract void OnCollect();
        public abstract void Spawn(Vector3 pos);

        public void SyncData()
        {
            int powerUpID = PowerUpManager.Instance.GetIndexOf(this);
            if (Networking.IsHostOrSolo())
            {
                CodZNetworking.Instance.PowerUpCollected_Send(powerUpID);
            }
            else
            {
                CodZNetworking.Instance.Client_PowerUpCollected_Send(powerUpID);
            }
        }

        public AudioClip ApplyAudio;
    }

    // Shops, doors, traps, teleports etc.
    public interface IPurchasable
    {
        int PurchaseCost { get; }
        bool IsOneTimeOnly { get; }
        bool AlreadyBought { get; }
    }

    public interface IRequiresPower
    {
        bool IsPowered { get; }
    }

    public interface ITrap
    {
        void OnEnemyEntered(ZombieController controller);
    }
}

#endif