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
        public Window TargetWindow;

        private const float agentUpdateInterval = .5f;
        private int _hitsGivingMoney = 6;

        private float _agentUpdateTimer;
        private float _cachedSpeed;
        //private bool _isAttackingWindow;
        private bool _isDead;

        public Sosig Sosig;

        private Coroutine _tearingPlanksCoroutine;

        // Every 5 seconds, change target if other is closer
        private float _zombieTargetTime = 5f;
        private float _zombieTargetTimer;

        public override void Initialize()
        {
            if (Networking.IsHostOrSolo())
            {
                Target = PlayersMgr.Instance.GetClosestAlivePlayer(transform.position).GetHead();
            }
            
            Sosig = GetComponent<Sosig>();

            Sosig.CoreRB.gameObject.AddComponent<ZosigTrigger>().Initialize(this);

            Sosig.Speed_Run = ZombieManager.Instance.ZosigPerRoundSpeed.Evaluate(RoundManager.Instance.RoundNumber);
            if (GameSettings.HardMode)
            {
                Sosig.Speed_Run += 1.12f;
            }

            Sosig.Mustard = ZombieManager.Instance.ZosigHPCurve.Evaluate(RoundManager.Instance.RoundNumber);
            foreach (SosigLink link in Sosig.Links)
            {
                link.SetIntegrity(
                    ZombieManager.Instance.ZosigLinkIntegrityCurve.Evaluate(RoundManager.Instance.RoundNumber));
            }

            if (GameSettings.WeakerEnemiesEnabled)
            {
                Sosig.Mustard *= .6f;
                foreach (SosigLink link in Sosig.Links)
                {
                    link.SetIntegrity(ZombieManager.Instance.ZosigLinkIntegrityCurve.Evaluate(RoundManager.Instance.RoundNumber) * .6f);
                }
            }

            if (RoundManager.Instance.IsRoundSpecial)
            {
                Sosig.Mustard *= .65f;
                foreach (SosigLink link in Sosig.Links)
                {
                    if (GameSettings.WeakerEnemiesEnabled)
                        link.SetIntegrity(ZombieManager.Instance.ZosigLinkIntegrityCurve.Evaluate(RoundManager.Instance.RoundNumber) * .45f);
                    else
                        link.SetIntegrity(ZombieManager.Instance.ZosigLinkIntegrityCurve.Evaluate(RoundManager.Instance.RoundNumber) * .65f);
                }
            }
            Sosig.Speed_Walk = Sosig.Speed_Run;
            Sosig.Speed_Turning = Sosig.Speed_Run;
            Sosig.Speed_Sneak = Sosig.Speed_Run;
            Sosig.Speed_Crawl = Sosig.Speed_Run;

            // Setting weapon IFF for disabling Friendly damage
            for (int i = 0; i < Sosig.Hands.Count; i++)
            {
                if (Sosig.Hands[i].HeldObject != null)
                {
                    Sosig.Hands[i].HeldObject.SourceIFF = Sosig.E.IFFCode;
                    Sosig.Hands[i].HeldObject.E.IFFCode = Sosig.E.IFFCode;
                }
            }
            _cachedSpeed = Sosig.Speed_Run;

            if (Networking.IsHostOrSolo())
            {
                Sosig.Hand_Primary.HeldObject.SourceIFF = Sosig.E.IFFCode;
                Sosig.Hand_Primary.HeldObject.E.IFFCode = Sosig.E.IFFCode;
            }
            
            CheckPerks();
        }

        public override void InitializeSpecialType()
        {
            StartCoroutine(SpawnSpecialEnemy());
        }

        private IEnumerator SpawnSpecialEnemy()
        {
            _cachedSpeed = 5f;
            Sosig.Speed_Run = 5f;
            Sosig.Speed_Walk = 5f;
            Sosig.Speed_Turning = 5f;
            Sosig.Speed_Sneak = 5f;
            Sosig.Speed_Crawl = 5f;

            CanInteractWithWindows = false;

            yield return new WaitForSeconds(2);
        }

        private void Update()
        {
            if (!Networking.IsHostOrSolo())
                return;
            
            if (Sosig == null)
                return;


            Sosig.Speed_Run = _cachedSpeed;
            Sosig.Speed_Walk = _cachedSpeed;
            Sosig.Speed_Turning = _cachedSpeed;
            Sosig.Speed_Crawl = _cachedSpeed;
            Sosig.Speed_Sneak = _cachedSpeed;

            if (TargetWindow && TargetWindow.IsBroken())
            {
                Sosig.Agent.isStopped = false;
                TargetWindow = null;
                Target = PlayersMgr.Instance.GetClosestAlivePlayer(transform.position).GetHead();
                Sosig.MaxHearingRange = 300;
                Sosig.MaxSightRange = 300;
                Sosig.CommandAssaultPoint(Target.position);
            }
            
            if (!TargetWindow)
            {
                _zombieTargetTimer += Time.deltaTime;
                if (_zombieTargetTimer >= _zombieTargetTime)
                {
                    _zombieTargetTimer -= _zombieTargetTime;
                    Target = PlayersMgr.Instance.GetClosestAlivePlayer(transform.position).GetHead();
                }
            }
        }

        public void AttackWindow(Window window)
        {
            TargetWindow = window;
            Target = TargetWindow.ZombieWaypoint;

            Sosig.MaxHearingRange = 0;
            Sosig.MaxSightRange = 0;
            Sosig.SetCurrentOrder(Sosig.SosigOrder.Idle);
            Sosig.Agent.isStopped = true;
        }

        private void LateUpdate()
        {
            if (!Networking.IsHostOrSolo())
                return;
            
            if (Sosig == null)
                return;

            _agentUpdateTimer += Time.deltaTime;
            if (_agentUpdateTimer >= agentUpdateInterval)
            {
                _agentUpdateTimer -= agentUpdateInterval;
                Sosig.CommandAssaultPoint(Target.position);
            }
        }

        public void Stun(float time)
        {
            if (_isDead)
                return;

            Sosig.Stun(time);
            Sosig.Shudder(.75f);
        }

        public void CheckPerks()
        {
            if (PlayerData.Instance.DeadShotPerkActivated)
            {
                Sosig.Links[0].DamMult = 1.35f;
            }

            if (PlayerData.Instance.DoubleTapPerkActivated)
            {
                Sosig.DamMult_Projectile = 1.25f;
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
                int killerID = Sosig.GetDiedFromIFF();
                if (Networking.IsMineIFF(killerID))
                {
                    GMgr.Instance.AddPoints(ZombieManager.Instance.PointsOnKill);
                }
                GMgr.Instance.Kills++;
            }
            
            ZombieManager.Instance.OnZombieDied(this);
            
            Debug.Log("After Zombie Killed, existing zombies: " + ZombieManager.Instance.ExistingZombies.Count + " Zombies remaining: " + ZombieManager.Instance.ZombiesRemaining);

            StartCoroutine(DelayedDespawn());
        }

        public void OnHit(Sosig sosig, Damage damage)
        {
            if (damage.Dam_TotalKinetic < 20)
                return;

            if (PlayerData.Instance.InstaKill)
            {
                Sosig.KillSosig();
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
            Sosig.Links[0].LinkExplodes(Damage.DamageClass.Projectile);
            Sosig.KillSosig();
            Debug.Log("Sosig was nuked");
        }
        
        public void OnTriggerEntered(Collider other)
        {
            if (_isDead)
                return;

            if (other.GetComponent<ITrap>() != null)
            {
                other.GetComponent<ITrap>().OnEnemyEntered(this);
            }

            // if (_isAttackingWindow)
            //     return;
            //
            // if (CanInteractWithWindows && other.GetComponent<WindowTrigger>())
            // {
            //     Window window = other.GetComponent<WindowTrigger>().Window;
            //     if (window.IsOpen)
            //     {
            //         ChangeTarget(GameRefs.Instance.Player);
            //         return;
            //     }
            //
            //     _isAttackingWindow = true;
            //
            //     _cachedSpeed = _sosig.Speed_Run;
            //     _sosig.Speed_Run = 0;
            //     _sosig.Speed_Walk = 0;
            //     _sosig.Speed_Turning = 0;
            //     _sosig.Speed_Crawl = 0;
            //     _sosig.Speed_Sneak = 0;
            //
            //     LastInteractedWindow = window;
            //     OnTouchingWindow();
            // }
        }
        
        public void OnTriggerExited(Collider other)
        {
        }

        // public void OnTouchingWindow()
        // {
        //     if (_tearingPlanksCoroutine == null)
        //         _tearingPlanksCoroutine = StartCoroutine(TearPlankDelayed());
        // }

        // public void OnHitWindow()
        // {
        //     LastInteractedWindow.OnPlankRipped();
        //
        //     if (LastInteractedWindow.IsOpen)
        //     {
        //         ChangeTarget(GameRefs.Instance.Player);
        //     }
        // }
        //
        // private IEnumerator TearPlankDelayed()
        // {
        //     while (!LastInteractedWindow.IsOpen && !_isDead)
        //     {
        //         yield return new WaitForSeconds(2.5f);
        //
        //         if (!_isDead && _sosig.BodyState == Sosig.SosigBodyState.InControl)
        //             OnHitWindow();
        //     }
        //
        //     _isAttackingWindow = false;
        //     _sosig.Speed_Run = _cachedSpeed;
        //     _sosig.Speed_Walk = _cachedSpeed;
        //     _sosig.Speed_Turning = _cachedSpeed;
        //     _sosig.Speed_Crawl = _cachedSpeed;
        //     _sosig.Speed_Sneak = _cachedSpeed;
        //
        //     _tearingPlanksCoroutine = null;
        // }

        private IEnumerator DelayedDespawn()
        {
            if (Sosig == null)
            {
                Debug.Log("Sosig is null");
                yield break;
            }
            
            if (!RoundManager.Instance.IsRoundSpecial)
            {
                yield return new WaitForSeconds(5);
            }
            
            Sosig.DeSpawnSosig();
        }
    }
}
#endif