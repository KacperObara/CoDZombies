#if H3VR_IMPORTED

using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts.Multiplayer;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomScripts
{
    public class MysteryBoxMover : MonoBehaviour
    {
        public bool MoveRandomlyOnStart = true;
        
        public List<Transform> SpawnPoints;
        public int CurrentSpawnPoint;

        [Range(0, 100)] public float TeleportChance = 20f;
        public int SafeRollsProvided = 3;

        public AudioClip TeddyBearSound;
        public AudioClip SecretTeddyBearSound;

        public Transform TeddyBear;

        [HideInInspector] public Transform CurrentPos;
        [HideInInspector] public int CurrentRoll = 0;
        private Animator _animator;

        private MysteryBox _mysteryBox;
        private Transform _parent;

        private int _nextTeleportWaypoint = -1;

        private void Awake()
        {
            _parent = transform.parent;
            _animator = _parent.GetComponent<Animator>();
            _mysteryBox = GetComponent<MysteryBox>();
        }

        private void Start()
        {
            RoundManager.OnGameStarted += TeleportOnStart;
        }

        public void TeleportOnStart()
        {
            if (Networking.IsHost() && MoveRandomlyOnStart)
            {
                int waypointID = GetRandomMovePoint();
                _nextTeleportWaypoint = waypointID;
                CodZNetworking.Instance.MysteryBoxMoved_Send(waypointID, true);
                Teleport();
            }
        }

        public void Teleport()
        {
            Transform newPos = SpawnPoints[_nextTeleportWaypoint];
            CurrentSpawnPoint = _nextTeleportWaypoint;
            
            CurrentPos = newPos;

            _parent.transform.position = newPos.position;
            _parent.transform.rotation = newPos.rotation;

            CurrentRoll = 0;
            _mysteryBox.InUse = false;
        }

        public bool CheckForTeleport()
        {
            if (CurrentRoll <= SafeRollsProvided)
                return false;

            if (Random.Range(0, 100) <= TeleportChance)
                return true;

            return false;
        }
        
        public void StartTeleporting()
        {
            int waypointID = GetRandomMovePoint();
            SetNextWaypoint(waypointID);
            CodZNetworking.Instance.MysteryBoxMoved_Send(waypointID, false);
            StartTeleportAnim();
        }

        public void StartTeleportAnim()
        {
            int secretTeddyChance = Random.Range(0, 1000);
            GameObject teddy;

            if (secretTeddyChance == 0)
            {
                teddy = TeddyBear.GetChild(1).gameObject;

                AudioManager.Instance.Play(SecretTeddyBearSound);
            }
            else
            {
                teddy = TeddyBear.GetChild(0).gameObject;

                AudioManager.Instance.Play(TeddyBearSound);
            }
            TeddyBear.GetComponent<Animator>().Play("Activation");
            teddy.SetActive(true);

            StartCoroutine(DelayedAnimation(teddy));
        }

        private IEnumerator DelayedAnimation(GameObject teddy)
        {
            yield return new WaitForSeconds(3f);

            teddy.SetActive(false);

            yield return new WaitForSeconds(1.2f);

            _animator.Play("Teleport");
            StartCoroutine(DelayedTeleport());
        }

        private IEnumerator DelayedTeleport()
        {
            yield return new WaitForSeconds(4.2f);
            Teleport();
        }

        public int GetRandomMovePoint()
        {
            List<int> availablePoints = new List<int>();
            for (int i = 0; i < SpawnPoints.Count; i++)
            {
                if (i == CurrentSpawnPoint)
                    continue;

                availablePoints.Add(i);
            }
            return availablePoints[Random.Range(0, availablePoints.Count)];
        }
        
        public void SetNextWaypoint(int waypointID)
        {
            _nextTeleportWaypoint = waypointID;
        }

        private void OnDestroy()
        {
            RoundManager.OnGameStarted -= TeleportOnStart;
        }
    }
}
#endif