#if H3VR_IMPORTED

using System.Collections;
using System.Collections.Generic;
using CustomScripts.Zombie;
using UnityEngine;

namespace CustomScripts
{
    public class PlayerCollider : MonoBehaviour
    {
        private Transform _transform;
        
        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            _transform.position = GameRefs.Instance.PlayerHead.position;

            float yRot = GameRefs.Instance.PlayerHead.rotation.eulerAngles.y;
            Vector3 newRot = _transform.rotation.eulerAngles;
            newRot.y = yRot;
            _transform.rotation = Quaternion.Euler(newRot);
        }
    }
}
#endif