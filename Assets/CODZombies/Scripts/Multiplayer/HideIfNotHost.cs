using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomScripts.Multiplayer
{
	public class HideIfNotHost : MonoBehaviour {
		private void Start()
		{
			if (!Networking.IsHostOrSolo())
				gameObject.SetActive(false);
		}
	}
}

