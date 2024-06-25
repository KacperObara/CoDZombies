#if H3VR_IMPORTED

using System;
using UnityEngine;
using UnityEngine.UI;
namespace CustomScripts
{
    public class EndPanel : MonoBehaviourSingleton<EndPanel>
    {
        public Text KillsText;
        public Text RoundText;
        
        public void UpdatePanel()
        {
            KillsText.text = "Kills: " + GMgr.Instance.Kills;
            RoundText.text = "Rounds: " + RoundManager.Instance.RoundNumber;
        }
    }
}

#endif