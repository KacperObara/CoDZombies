using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CustomScripts.Gamemode
{
	public class RoundView : MonoBehaviour 
	{
		public Text RoundText;

		private void Awake()
		{
			RoundManager.OnRoundChanged -= UpdateRoundText;
			RoundManager.OnRoundChanged += UpdateRoundText;
		}

		private void Start()
		{
			UpdateRoundText();
		}

		private void OnDestroy()
		{
			RoundManager.OnRoundChanged -= UpdateRoundText;
		}

		private void UpdateRoundText()
		{
			RoundText.text = "Round: " + RoundManager.Instance.RoundNumber;
		}
	}
}

