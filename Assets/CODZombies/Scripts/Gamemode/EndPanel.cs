#if H3VR_IMPORTED

using System;
using UnityEngine;
using UnityEngine.UI;
namespace CustomScripts
{
    public class EndPanel : MonoBehaviourSingleton<EndPanel>
    {
        public Text KillsText;
        
        public void UpdatePanel()
        {
            KillsText.text = "Kills: " + GMgr.Instance.Kills;
        }
    }
}

#endif