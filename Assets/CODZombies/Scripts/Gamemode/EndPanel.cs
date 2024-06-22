#if H3VR_IMPORTED

using System;
using UnityEngine;
using UnityEngine.UI;
namespace CustomScripts
{
    public class EndPanel : MonoBehaviourSingleton<EndPanel>
    {
        public Text TotalPointsText;
        public Text KillsText;

        public Text DifficultyText;
        public Text SpecialRoundsText;
        public Text LessZombieHPText;


        public void UpdatePanel()
        {
            //TotalPointsText.text = "Total Points:\n" + GMgr.Instance.TotalPoints;

            KillsText.text = "Kills:\n" + GMgr.Instance.Kills;

            DifficultyText.text = GameSettings.HardMode ? "Hard" : "Normal";
            SpecialRoundsText.text = GameSettings.SpecialRoundDisabled ? "No" : "Yes";
            LessZombieHPText.text = GameSettings.WeakerEnemiesEnabled ? "Yes" : "No";
        }
    }
}

#endif