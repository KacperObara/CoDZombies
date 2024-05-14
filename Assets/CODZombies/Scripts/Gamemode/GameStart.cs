#if H3VR_IMPORTED
using UnityEngine;
namespace CustomScripts
{
    public class GameStart : MonoBehaviourSingleton<GameStart>
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
}
#endif