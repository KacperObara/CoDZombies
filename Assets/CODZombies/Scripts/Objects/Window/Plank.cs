#if H3VR_IMPORTED

using System;
using System.Collections;
using FistVR;
using UnityEngine;
namespace CustomScripts
{
    public class Plank : MonoBehaviour
    {
        private Window _window;
        private Transform _parentTransform;
        
        public bool IsBroken;

        private Vector3 _destroyedPos;

        public void Initialize(Window window)
        {
            _window = window;
            _parentTransform = transform.parent;
            _destroyedPos = _window.transform.position - (_window.transform.right * 2f);
            gameObject.SetActive(true);
        }

        public void Tear()
        {
            IsBroken = true;
            StartCoroutine(TearAnimation());
        }
        
        public void Repair(bool instant)
        {
            IsBroken = false;
            gameObject.SetActive(true);
            
            if (instant)
            {
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
            else
            {
                StartCoroutine(RepairAnimation());
            }
        }
        
        private IEnumerator RepairAnimation()
        { 
            // Rise and rotate
            Vector3 targetPosition = _destroyedPos;
            targetPosition.y = _parentTransform.position.y;
            
            Quaternion targetRotation = Quaternion.identity;
            Quaternion startRotation = transform.localRotation;
            
            float elapsedTime = 0;
            while (elapsedTime < 0.5f)
            {
                transform.position = Vector3.Lerp(_destroyedPos, targetPosition, elapsedTime / 0.5f);
                transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / 0.5f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
            transform.localRotation = targetRotation;
            
            // Pause
            yield return new WaitForSeconds(0.75f);
            
            // Move to window
            Vector3 localPos = transform.localPosition;
            elapsedTime = 0;
            while (elapsedTime < 0.25f)
            {
                transform.localPosition = Vector3.Lerp(localPos, Vector3.zero, elapsedTime / 0.25f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            _window.OnPlankInPlace();
        }

        public IEnumerator TearAnimation()
        {
            Vector3 startingPos = transform.position;
            Vector3 finalPos = _destroyedPos;
            Quaternion startingRot = transform.rotation;
            Quaternion finalRot = Quaternion.identity;
        
            float elapsedTime = 0;
        
            while (elapsedTime < 0.5f)
            {
                transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / 0.5f));
                transform.rotation = Quaternion.Lerp(startingRot, finalRot, (elapsedTime / 0.5f));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        
            transform.position = finalPos;
            transform.rotation = finalRot;
            gameObject.SetActive(false);
        }
    }
}
#endif