#if H3VR_IMPORTED
using System.Collections;
using CustomScripts.Gamemode;
using CustomScripts.Gamemode.GMDebug;
using CustomScripts.Multiplayer;
using CustomScripts.Player;
using FistVR;
using H3MP;
using UnityEngine;
namespace CustomScripts.Powerups
{
    public class PowerUpDeathMachine : PowerUp
    {
        public AudioClip EndAudio;

        public MeshRenderer Renderer;

        public CustomItemSpawner MinigunSpawner;
        public CustomItemSpawner MagazineSpawner;

        private Animator _animator;
        private FVRPhysicalObject _magazineObject;
        
        public FVRPhysicalObject MinigunObject;

        private void Awake()
        {
            _animator = transform.GetComponent<Animator>();
        }

        public override void Spawn(Vector3 pos)
        {
            transform.position = pos;
            Renderer.enabled = true;
            _animator.Play("Rotating");
            StartCoroutine(DespawnDelay());
        }
        
        public override void OnCollect()
        {
            AudioManager.Instance.Play(ApplyAudio, .5f);

            Despawn();
        }

        public override void ApplyModifier()
        {
            MinigunSpawner.Spawn();
            MagazineSpawner.Spawn();

            MinigunObject = MinigunSpawner.SpawnedObject.GetComponent<FVRPhysicalObject>();
            MinigunObject.SpawnLockable = false;
            MinigunObject.UsesGravity = false;

            MinigunObject.RootRigidbody.isKinematic = true;
            MinigunObject.GetComponent<WeaponWrapper>().SetOwner(GameManager.ID);

            _magazineObject = MagazineSpawner.SpawnedObject.GetComponent<FVRPhysicalObject>();
            _magazineObject.SpawnLockable = false;
            _magazineObject.UsesGravity = false;

            _magazineObject.RootRigidbody.isKinematic = true;

            (_magazineObject as FVRFireArmMagazine).Load(MinigunObject as FVRFireArm);

            PlayerData.Instance.DeathMachinePowerUpIndicator.Activate(30f);

            StartCoroutine(DisablePowerUpDelay(30f));
            
            if (Networking.IsHostOrSolo())
                OnCollect();
            SyncData();
        }

        private IEnumerator DisablePowerUpDelay(float time)
        {
            yield return new WaitForSeconds(time);
            AudioManager.Instance.Play(EndAudio, .5f);

            MinigunObject.ForceBreakInteraction();
            MinigunObject.IsPickUpLocked = true;
            Destroy(MinigunObject.gameObject);

            _magazineObject.ForceBreakInteraction();
            _magazineObject.IsPickUpLocked = true;
            Destroy(_magazineObject.gameObject);
        }

        private void Despawn()
        {
            transform.position += new Vector3(0, -1000f, 0);
        }

        private IEnumerator DespawnDelay()
        {
            yield return new WaitForSeconds(15f);

            for (int i = 0; i < 5; i++)
            {
                Renderer.enabled = false;
                yield return new WaitForSeconds(.3f);
                Renderer.enabled = true;
                yield return new WaitForSeconds(.7f);
            }

            Despawn();
        }
    }
}
#endif