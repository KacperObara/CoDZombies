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
        public List<Transform> SpawnPoints;

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
            if (Networking.IsHost())
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
            
            CurrentPos = newPos;

            _parent.transform.position = newPos.position;
            _parent.transform.rotation = newPos.rotation;

            CurrentRoll = 0;
            _mysteryBox.InUse = false;
        }

        public bool TryTeleport()
        {
            if (CurrentRoll <= SafeRollsProvided)
                return false;
            
            if (Random.Range(0, 100) <= TeleportChance)
            {
                int waypointID = GetRandomMovePoint();
                SetNextWaypoint(waypointID);
                CodZNetworking.Instance.MysteryBoxMoved_Send(waypointID, false);
                StartTeleportAnim();
                return true;
            }
            
            return false;
        }

        public void StartTeleportAnim()
        {
            int secretTeddyChance = Random.Range(0, 5801);
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
            return 0;
        }
        
        public void SetNextWaypoint(int waypointID)
        {
            _nextTeleportWaypoint = waypointID;
        }
    }
}
#endif