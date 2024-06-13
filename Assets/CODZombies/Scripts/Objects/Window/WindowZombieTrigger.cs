#if H3VR_IMPORTED

using CustomScripts.Zombie;
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
            if (Window.IsBroken)
                return;
            
            if (other.GetComponent<ZosigTrigger>())
            {
                ZosigZombieController zombie = other.GetComponent<ZosigTrigger>().ZosigController;
                if (zombie.TargetWindow == null)
                    zombie.AttackWindow(Window);
                
                _timer += Time.deltaTime;
                if (_timer >= _timeToTrigger)
                {
                    Window.TearPlank();
                    _timer = 0;
                }
            }
        }
    }
}
#endif