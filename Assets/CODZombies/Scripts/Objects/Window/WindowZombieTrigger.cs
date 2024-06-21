#if H3VR_IMPORTED

using CustomScripts.Multiplayer;
using CustomScripts.Zombie;
using FistVR;
using UnityEngine;
namespace CustomScripts
{
    public class WindowZombieTrigger : MonoBehaviour
    {
        public Window Window;

        private float _timer;
        private float _timeToTrigger = 2.5f;
        private void OnTriggerStay(Collider other)
        {
            if (!Networking.IsHostOrSolo())
                return;
            
            if (Window.IsBroken())
                return;
            
            if (other.GetComponent<ZosigTrigger>())
            {
                ZosigZombieController zombie = other.GetComponent<ZosigTrigger>().ZosigController;
                if (zombie.Sosig.BodyState == Sosig.SosigBodyState.Dead || zombie.Sosig.BodyPose == Sosig.SosigBodyPose.Prone || zombie.Sosig.IsConfused)
                    return;
                
                if (Vector3.Distance(zombie.Sosig.transform.position, transform.position) < 2f)
                {
                    if (zombie.TargetWindow == null)
                        zombie.AttackWindow(Window);
                
                    _timer += Time.deltaTime;
                    if (_timer >= _timeToTrigger)
                    {
                        //Debug.Log("Zombie destroyed window: " + zombie.Sosig.BodyPose + "   " + zombie.Sosig.IsConfused);
                        Window.TearPlank();
                        _timer = 0;
                    }
                }
            }
        }
    }
}
#endif