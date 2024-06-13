#if H3VR_IMPORTED

using System.Collections;
using FistVR;
using UnityEngine;
namespace CustomScripts
{
    public class Plank : MonoBehaviour
    {
        public bool IsBroken = false;
        
        public void Tear()
        {
            IsBroken = true;
            transform.localPosition = new Vector3(0, -10f, -10f);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        
        public void Repair()
        {
            IsBroken = false;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        // public IEnumerator MoveTo(Transform destination, float time)
        // {
        //     Vector3 startingPos = transform.position;
        //     Vector3 finalPos = destination.position;
        //     Quaternion startingRot = transform.rotation;
        //     Quaternion finalRot = destination.rotation;
        //
        //     float elapsedTime = 0;
        //
        //     while (elapsedTime < time)
        //     {
        //         transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
        //         transform.rotation = Quaternion.Lerp(startingRot, finalRot, (elapsedTime / time));
        //         elapsedTime += Time.deltaTime;
        //         yield return null;
        //     }
        //
        //     transform.position = finalPos;
        //     transform.rotation = finalRot;
        // }
    }
}
#endif