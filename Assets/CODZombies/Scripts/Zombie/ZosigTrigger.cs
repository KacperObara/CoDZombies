#if H3VR_IMPORTED
using UnityEngine;
namespace CustomScripts.Zombie
{
    public class ZosigTrigger : MonoBehaviour
    {
        public ZosigZombieController ZosigController;

        private void OnTriggerEnter(Collider other)
        {
            ZosigController.OnTriggerEntered(other);
        }

        public void Initialize(ZosigZombieController controller)
        {
            ZosigController = controller;
        }

        private void OnTriggerExit(Collider other)
        {
            ZosigController.OnTriggerExited(other);
        }
    }
}
#endif