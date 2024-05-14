#if H3VR_IMPORTED
using System;
using System.Collections.Generic;
using System.Linq;
using FistVR;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CustomScripts
{
    public class GameSettings : MonoBehaviourSingleton<GameSettings>
    {
        public static Action OnSettingsChanged;

        public static bool HardMode;
        public static bool SpecialRoundDisabled;
        public static bool ItemSpawnerEnabled;
        public static bool WeakerEnemiesEnabled;

        public Text OptionDescriptionText;

        private void Start()
        {
            OptionDescriptionText.text = "Call of Duty\nZOMBIES";
            if (Random.Range(0, 500) == 0)
            {
                int random = Random.Range(0, 7);
                switch (random)
                {
                    case 0: OptionDescriptionText.text = "You are my sunshine,\nMy only sunshine"; break;
                    case 1: OptionDescriptionText.text = "I missed you"; break;
                    case 2: OptionDescriptionText.text = "Play with me"; break;
                    case 3: OptionDescriptionText.text = "It's just a game, mostly"; break;
                    case 4: OptionDescriptionText.text = "I have granted kids to hell"; break;
                    case 5: OptionDescriptionText.text = "It's only partially your fault"; break;
                }
            }
        }

        public void RestoreDefaultSettings()
        {
            HardMode = false;
            ItemSpawnerEnabled = false;
            SpecialRoundDisabled = false;
            WeakerEnemiesEnabled = false;

            OptionDescriptionText.text = "Default settings restored.";
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();
        }

        public void DifficultyNormalClicked()
        {
            HardMode = false;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Normal mode: Normal Enemy speed, HP and numbers.";
        }

        public void DifficultyHardClicked()
        {
            HardMode = true;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Hard mode: Increased Enemy speed, HP and numbers.";
        }

        public void EnableSpecialRoundClicked()
        {
            SpecialRoundDisabled = false;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Special round inspired by Hellhounds, occurs every 8 rounds.";
        }

        public void DisableSpecialRoundClicked()
        {
            SpecialRoundDisabled = true;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Special round disabled.";
        }

        public void EnableItemSpawnerClicked()
        {
            ItemSpawnerEnabled = true;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Item Spawner will appear at the starting location. Scoring will be disabled for that game.";
        }

        public void DisableItemSpawnerClicked()
        {
            ItemSpawnerEnabled = false;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "No Item Spawner. Scoring enabled";
        }

        public void EnableWeakerEnemiesClicked()
        {
            WeakerEnemiesEnabled = true;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Enemies have reduced HP";
        }

        public void DisableWeakerEnemiesClicked()
        {
            WeakerEnemiesEnabled = false;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Enemies have normal HP";
        }

        public void ToggleBackgroundMusic()
        {
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();
        }
    }
}
#endif