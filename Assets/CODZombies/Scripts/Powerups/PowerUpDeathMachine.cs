#if H3VR_IMPORTED
using System.Collections;
using CustomScripts.Gamemode.GMDebug;
using CustomScripts.Multiplayer;
using CustomScripts.Player;
using FistVR;
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

        private FVRPhysicalObject _minigunObject;

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
            if (Networking.IsHostOrSolo())
            {
                MinigunSpawner.Spawn();
                MagazineSpawner.Spawn();

                _minigunObject = MinigunSpawner.SpawnedObject.GetComponent<FVRPhysicalObject>();
                _minigunObject.SpawnLockable = false;
                _minigunObject.UsesGravity = false;

                _minigunObject.RootRigidbody.isKinematic = true;

                _magazineObject = MagazineSpawner.SpawnedObject.GetComponent<FVRPhysicalObject>();
                _magazineObject.SpawnLockable = false;
                _magazineObject.UsesGravity = false;

                _magazineObject.RootRigidbody.isKinematic = true;

                (_magazineObject as FVRFireArmMagazine).Load(_minigunObject as FVRFireArm);

                PlayerData.Instance.DeathMachinePowerUpIndicator.Activate(30f);

                StartCoroutine(DisablePowerUpDelay(30f));
            }
            
            AudioManager.Instance.Play(ApplyAudio, .5f);

            Despawn();
        }

        public override void ApplyModifier()
        {
            SyncData();
            if (Networking.IsHostOrSolo())
                ApplyModifier();
        }

        private IEnumerator DisablePowerUpDelay(float time)
        {
            yield return new WaitForSeconds(time);
            AudioManager.Instance.Play(EndAudio, .5f);

            _minigunObject.ForceBreakInteraction();
            _minigunObject.IsPickUpLocked = true;
            Destroy(_minigunObject.gameObject);

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