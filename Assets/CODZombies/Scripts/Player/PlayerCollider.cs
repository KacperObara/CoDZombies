#if H3VR_IMPORTED

using System.Collections;
using System.Collections.Generic;
using CustomScripts.Zombie;
using UnityEngine;

namespace CustomScripts
{
    public class PlayerCollider : MonoBehaviour
    {
        public LayerMask EnemyLayer;

        private Transform _transform;
        
        private Coroutine _dealDamageCoroutine;

        private const float DamageInterval = 1.5f;
        private float _damageTimer;

        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            _transform.position = GameReferences.Instance.PlayerHead.position;

            float yRot = GameReferences.Instance.PlayerHead.rotation.eulerAngles.y;
            Vector3 newRot = _transform.rotation.eulerAngles;
            newRot.y = yRot;
            _transform.rotation = Quaternion.Euler(newRot);
        }
    }
}
#endif