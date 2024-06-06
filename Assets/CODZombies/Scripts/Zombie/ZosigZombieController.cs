#if H3VR_IMPORTED
using System;
using System.Collections;
using System.Linq;
using CustomScripts.Managers;
using CustomScripts.Multiplayer;
using CustomScripts.Player;
using FistVR;
using H3MP;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace CustomScripts.Zombie
{
    public class ZosigZombieController : ZombieController
    {
        public bool CanInteractWithWindows = true;

        private const float agentUpdateInterval = .5f;
        private int _hitsGivingMoney = 6;

        private float _agentUpdateTimer;
        private float _cachedSpeed;
        private bool _isAttackingWindow;
        private bool _isDead;

        private Sosig _sosig;

        private Coroutine _tearingPlanksCoroutine;

        // Every 5 seconds, change target if other is closer
        private float _zombieTargetTime = 5f;
        private float _zombieTargetTimer;

        public override void Initialize()
        {
            //// If solo, target you, otherwise target random player (It's more complicated to include host in random player selection)
            if (Networking.IsHostOrSolo())
            {
                Target = PlayersMgr.Instance.GetClosestAlivePlayer(transform.position).GetHead();
            }
            
            _sosig = GetComponent<Sosig>();

            _sosig.CoreRB.gameObject.AddComponent<ZosigTrigger>().Initialize(this);

            _sosig.Speed_Run = ZombieManager.Instance.ZosigPerRoundSpeed.Evaluate(RoundManager.Instance.RoundNumber);
            if (GameSettings.HardMode)
            {
                _sosig.Speed_Run += 1.12f;
            }

            _sosig.Mustard = ZombieManager.Instance.ZosigHPCurve.Evaluate(RoundManager.Instance.RoundNumber);
            foreach (SosigLink link in _sosig.Links)
            {
                link.SetIntegrity(
                    ZombieManager.Instance.ZosigLinkIntegrityCurve.Evaluate(RoundManager.Instance.RoundNumber));
            }

            if (GameSettings.WeakerEnemiesEnabled)
            {
                _sosig.Mustard *= .6f;
                foreach (SosigLink link in _sosig.Links)
                {
                    link.SetIntegrity(ZombieManager.Instance.ZosigLinkIntegrityCurve.Evaluate(RoundManager.Instance.RoundNumber) * .6f);
                }
            }

            if (RoundManager.Instance.IsRoundSpecial)
            {
                _sosig.Mustard *= .65f;
                foreach (SosigLink link in _sosig.Links)
                {
                    if (GameSettings.WeakerEnemiesEnabled)
                        link.SetIntegrity(ZombieManager.Instance.ZosigLinkIntegrityCurve.Evaluate(RoundManager.Instance.RoundNumber) * .45f);
                    else
                        link.SetIntegrity(ZombieManager.Instance.ZosigLinkIntegrityCurve.Evaluate(RoundManager.Instance.RoundNumber) * .65f);
                }
            }
            
            _sosig.Speed_Walk = _sosig.Speed_Run;
            _sosig.Speed_Turning = _sosig.Speed_Run;
            _sosig.Speed_Sneak = _sosig.Speed_Run;
            _sosig.Speed_Crawl = _sosig.Speed_Run;

            // Setting weapon IFF for disabling Friendly damage
            for (int i = 0; i < _sosig.Hands.Count; i++)
            {
                if (_sosig.Hands[i].HeldObject != null)
                {
                    _sosig.Hands[i].HeldObject.SourceIFF = _sosig.E.IFFCode;
                    _sosig.Hands[i].HeldObject.E.IFFCode = _sosig.E.IFFCode;
                }
            }

            _sosig.Hand_Primary.HeldObject.SourceIFF = _sosig.E.IFFCode;
            _sosig.Hand_Primary.HeldObject.E.IFFCode = _sosig.E.IFFCode;

            _cachedSpeed = _sosig.Speed_Run;

            CheckPerks();
        }

        public override void InitializeSpecialType()
        {
            StartCoroutine(SpawnSpecialEnemy());
        }

        private IEnumerator SpawnSpecialEnemy()
        {
            _cachedSpeed = 5f;
            _sosig.Speed_Run = 5f;
            _sosig.Speed_Walk = 5f;
            _sosig.Speed_Turning = 5f;
            _sosig.Speed_Sneak = 5f;
            _sosig.Speed_Crawl = 5f;

            //_sosig.Agent.agentTypeID = 1; // Changing zombies agent type to crawlers(Hellhounds)

            CanInteractWithWindows = false;

            yield return new WaitForSeconds(2);
        }

        private void Update()
        {
            if (!Networking.IsHostOrSolo())
                return;
            
            if (_sosig == null)
                return;

            if (_isAttackingWindow)
            {
                _sosig.Speed_Run = 0;
                _sosig.Speed_Walk = 0;
                _sosig.Speed_Turning = 0;
                _sosig.Speed_Crawl = 0;
                _sosig.Speed_Sneak = 0;
            }
            else
            {
                _sosig.Speed_Run = _cachedSpeed;
                _sosig.Speed_Walk = _cachedSpeed;
                _sosig.Speed_Turning = _cachedSpeed;
                _sosig.Speed_Crawl = _cachedSpeed;
                _sosig.Speed_Sneak = _cachedSpeed;
            }
            
            _zombieTargetTimer += Time.deltaTime;
            if (_zombieTargetTimer >= _zombieTargetTime)
            {
                _zombieTargetTimer -= _zombieTargetTime;
                Target = PlayersMgr.Instance.GetClosestAlivePlayer(transform.position).GetHead();
            }
        }

        private void LateUpdate()
        {
            if (!Networking.IsHostOrSolo())
                return;
            
            if (_sosig == null)
                return;

            _agentUpdateTimer += Time.deltaTime;
            if (_agentUpdateTimer >= agentUpdateInterval)
            {
                _agentUpdateTimer -= agentUpdateInterval;

                _sosig.CommandAssaultPoint(Target.position);
            }
        }

        public void Stun(float time)
        {
            if (_isDead)
                return;

            _sosig.Stun(time);
            _sosig.Shudder(.75f);
        }

        public void CheckPerks()
        {
            if (PlayerData.Instance.DeadShotPerkActivated)
            {
                _sosig.Links[0].DamMult = 1.35f;
            }

            if (PlayerData.Instance.DoubleTapPerkActivated)
            {
                _sosig.DamMult_Projectile = 1.25f;
            }
        }

        public override void OnKill(bool awardPoints = true)
        {
            if (!ZombieManager.Instance.ExistingZombies.Contains(this))
                return;

            _isDead = true;

            if (RoundManager.Instance.IsRoundSpecial)
            {
                AudioManager.Instance.Play(AudioManager.Instance.HellHoundDeathSound, .13f);
                var explosionPS = Instantiate(ZombieManager.Instance.HellhoundExplosionPS, transform.position + new Vector3(0, .75f, 0), transform.rotation);
                Destroy(explosionPS.gameObject, 4f);
            }
            
            if (awardPoints)
            {
                int killerID = _sosig.GetDiedFromIFF();
                if (Networking.IsMineIFF(killerID))
                {
                    GMgr.Instance.AddPoints(ZombieManager.Instance.PointsOnKill);
                }

                Debug.Log("Sosig Died killer: " + killerID + " and me " + GM.CurrentPlayerBody.GetPlayerIFF());
                if (Networking.IsMineIFF(killerID))
                {
                    Debug.Log("That means I killed it!");
                }

                ZombieManager.Instance.OnZombieDied(this);
            }
            else
            {
                ZombieManager.Instance.ExistingZombies.Remove(this);
            }

            StartCoroutine(DelayedDespawn());
        }

        public void OnHit(Sosig sosig, Damage damage)
        {
            if (damage.Dam_TotalKinetic < 20)
                return;

            if (PlayerData.Instance.InstaKill)
            {
                _sosig.KillSosig();
            }

            if (_hitsGivingMoney <= 0)
                return;

            _hitsGivingMoney--;

            if (Networking.IsMineIFF(damage.Source_IFF))
                GMgr.Instance.AddPoints(ZombieManager.Instance.PointsOnHit);
        }

        public override void OnHit(float damage, bool headHit)
        {
            //nuke
            _sosig.Links[0].LinkExplodes(Damage.DamageClass.Projectile);
            _sosig.KillSosig();
        }

        public void ChangeTarget(Transform newTarget)
        {
            Target = newTarget;
        }

        public void OnTriggerEntered(Collider other)
        {
            if (_isDead)
                return;

            if (other.GetComponent<ITrap>() != null)
            {
                other.GetComponent<ITrap>().OnEnemyEntered(this);
            }

            if (_isAttackingWindow)
                return;

            if (CanInteractWithWindows && other.GetComponent<WindowTrigger>())
            {
                Window window = other.GetComponent<WindowTrigger>().Window;
                if (window.IsOpen)
                {
                    ChangeTarget(GameReferences.Instance.Player);
                    return;
                }

                _isAttackingWindow = true;

                _cachedSpeed = _sosig.Speed_Run;
                _sosig.Speed_Run = 0;
                _sosig.Speed_Walk = 0;
                _sosig.Speed_Turning = 0;
                _sosig.Speed_Crawl = 0;
                _sosig.Speed_Sneak = 0;

                LastInteractedWindow = window;
                OnTouchingWindow();
            }
        }

        // TODO In next version, create two colliders, one for entering, second larger for exiting to avoid looping
        // Be mindful that sosig can sometimes get stuck on the edge and enter and exit constantly,
        // which means it will take longer to tear down planks
        // public void OnTriggerExited(Collider other)
        // {
        //     if (_isDead)
        //         return;
        //
        //     if (other.GetComponent<WindowTrigger>())
        //     {
        //         _isAttackingWindow = false;
        //         _sosig.Speed_Run = _cachedSpeed;
        //         _sosig.Speed_Walk = _cachedSpeed;
        //         _sosig.Speed_Turning = _cachedSpeed;
        //         _sosig.Speed_Crawl = _cachedSpeed;
        //         _sosig.Speed_Sneak = _cachedSpeed;
        //
        //         ChangeTarget(GameReferences.Instance.Player);
        //
        //         if (_tearingPlanksCoroutine != null)
        //             StopCoroutine(_tearingPlanksCoroutine);
        //     }
        // }

        public void OnTriggerExited(Collider other)
        {
        }

        public void OnTouchingWindow()
        {
            if (_tearingPlanksCoroutine == null)
                _tearingPlanksCoroutine = StartCoroutine(TearPlankDelayed());
        }

        public void OnHitWindow()
        {
            LastInteractedWindow.OnPlankRipped();

            if (LastInteractedWindow.IsOpen)
            {
                ChangeTarget(GameReferences.Instance.Player);
            }
        }

        private IEnumerator TearPlankDelayed()
        {
            while (!LastInteractedWindow.IsOpen && !_isDead)
            {
                yield return new WaitForSeconds(2.5f);

                if (!_isDead && _sosig.BodyState == Sosig.SosigBodyState.InControl)
                    OnHitWindow();
            }

            _isAttackingWindow = false;
            _sosig.Speed_Run = _cachedSpeed;
            _sosig.Speed_Walk = _cachedSpeed;
            _sosig.Speed_Turning = _cachedSpeed;
            _sosig.Speed_Crawl = _cachedSpeed;
            _sosig.Speed_Sneak = _cachedSpeed;

            _tearingPlanksCoroutine = null;
        }

        private IEnumerator DelayedDespawn()
        {
            if (_sosig == null)
            {
                Debug.Log("Sosig is null");
                yield break;
            }
            
            if (!RoundManager.Instance.IsRoundSpecial)
            {
                yield return new WaitForSeconds(5);
            }
            
            _sosig.DeSpawnSosig();
        }
    }
}
#endif