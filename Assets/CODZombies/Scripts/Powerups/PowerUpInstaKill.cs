#if H3VR_IMPORTED
using System.Collections;
using CustomScripts.Multiplayer;
using CustomScripts.Player;
using UnityEngine;
namespace CustomScripts.Powerups
{
    public class PowerUpInstaKill : PowerUp
    {
        public AudioClip EndAudio;
        public MeshRenderer Renderer;
        private Animator _animator;

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
            PlayerData.Instance.InstaKill = true;
            PlayerData.Instance.InstaKillPowerUpIndicator.Activate(30f);
            StartCoroutine(DisablePowerUpDelay(30f));
            AudioManager.Instance.Play(ApplyAudio, .2f);
            
            Despawn();
        }

        public override void ApplyModifier()
        {
            if (Networking.IsHostOrSolo())
                OnCollect();
            SyncData();
        }

        private IEnumerator DisablePowerUpDelay(float time)
        {
            yield return new WaitForSeconds(time);
            AudioManager.Instance.Play(EndAudio, .5f);
            PlayerData.Instance.InstaKill = false;
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