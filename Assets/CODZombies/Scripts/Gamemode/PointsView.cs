#if H3VR_IMPORTED
using UnityEngine;
using UnityEngine.UI;
namespace CustomScripts.Gamemode
{
    public class PointsView : MonoBehaviour
    {
        public Text PointsText;

        private void Awake()
        {
            GMgr.OnPointsChanged += OnPointsChanged;
        }
        
        private void Start()
        {
            OnPointsChanged();
        }

        private void OnDestroy()
        {
            GMgr.OnPointsChanged -= OnPointsChanged;
        }

        private void OnPointsChanged()
        {
            PointsText.text = "Points:\n" + GMgr.Instance.Points;
        }
    }
}
#endif