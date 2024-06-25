#if H3VR_IMPORTED
using Atlas.MappingComponents.Sandbox;
using FistVR;
using UnityEngine;

namespace CustomScripts.Powerups.Perks
{
    public class MuleKickPerkBottle : PerkBottle
    {
        public ObjectSpawnPoint Spawner;
        public string ObjectID;

        public GameObject Model;

        public override void ApplyModifier()
        {
            base.ApplyModifier();
            Spawner.ObjectId = ObjectID;
            Spawner.Spawn();

            //GetComponentInParent<FVRPhysicalObject>().IsPickUpLocked = true;
            //GetComponentInParent<FVRPhysicalObject>().ForceBreakInteraction();
            //GetComponent<Collider>().enabled = false;
            //Model.SetActive(false);

            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            transform.parent.position = OriginPos;
        }
    }
}
#endif