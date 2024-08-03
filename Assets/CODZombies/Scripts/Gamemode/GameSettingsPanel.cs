#if H3VR_IMPORTED
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace CustomScripts
{
    public class GameSettingsPanel : MonoBehaviour
    {
        public Text DifficultyNormalText;
        public Text DifficultyHardText;
        public Text SpecialRoundEnabledText;
        public Text SpecialRoundDisabledText;
        public Text ItemSpawnerEnabledText;
        public Text ItemSpawnerDisabledText;
        public Text WeakerZombiesEnabledText;
        public Text WeakerZombiesDisabledText;
        public Text WonderWeaponsEnabledText;
        public Text WonderWeaponsDisabledText;

        public Color EnabledColor;
        public Color DisabledColor;

        private void Awake()
        {
            GameSettings.OnSettingsChanged += UpdateText;
        }

        private void Start()
        {
            UpdateText();
        }

        private void OnDestroy()
        {
            GameSettings.OnSettingsChanged -= UpdateText;
        }

        private void UpdateText()
        {
            DifficultyNormalText.color = GameSettings.HardMode ? DisabledColor : EnabledColor;
            DifficultyHardText.color = GameSettings.HardMode ? EnabledColor : DisabledColor;

            SpecialRoundEnabledText.color = GameSettings.SpecialRoundDisabled ? DisabledColor : EnabledColor;
            SpecialRoundDisabledText.color = GameSettings.SpecialRoundDisabled ? EnabledColor : DisabledColor;

            ItemSpawnerEnabledText.color = GameSettings.ItemSpawnerEnabled ? EnabledColor : DisabledColor;
            ItemSpawnerDisabledText.color = GameSettings.ItemSpawnerEnabled ? DisabledColor : EnabledColor;

            WeakerZombiesEnabledText.color = GameSettings.WeakerEnemiesEnabled ? EnabledColor : DisabledColor;
            WeakerZombiesDisabledText.color = GameSettings.WeakerEnemiesEnabled ? DisabledColor : EnabledColor;
            
            WonderWeaponsEnabledText.color = GameSettings.WonderWeaponEnabled ? EnabledColor : DisabledColor;
            WonderWeaponsDisabledText.color = GameSettings.WonderWeaponEnabled ? DisabledColor : EnabledColor;
        }
    }
}
#endif