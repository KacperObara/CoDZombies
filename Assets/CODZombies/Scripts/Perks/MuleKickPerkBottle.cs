#if H3VR_IMPORTED
using Atlas.MappingComponents.Sandbox;
using FistVR;
using UnityEngine;

namespace CustomScripts.Powerups.Perks
{
    public class MuleKickPerkBottle : MonoBehaviour, IModifier
    {
        public ObjectSpawnPoint Spawner;
        public string ObjectID;

        public GameObject Model;

        private Vector3 _originalPosition;
        
        private void Start()
        {
            _originalPosition = transform.parent.position;
        }
        
        public void ApplyModifier()
        {
            Spawner.ObjectId = ObjectID;
            Spawner.Spawn();

            GetComponent<FVRPhysicalObject>().IsPickUpLocked = true;
            GetComponent<FVRPhysicalObject>().ForceBreakInteraction();
            GetComponent<Collider>().enabled = false;
            Model.SetActive(false);

            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            GetComponent<FVRPhysicalObject>().ForceBreakInteraction();
            transform.parent.position = _originalPosition;
        }
    }
}
#endif