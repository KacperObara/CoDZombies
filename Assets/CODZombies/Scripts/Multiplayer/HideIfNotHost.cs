using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomScripts.Multiplayer
{
	public class HideIfNotHost : MonoBehaviour {
		private void Start()
		{
			//if (!Networking.ServerRunning())
			//	return;
			
			//if (Networking.IsClient())
			//	gameObject.SetActive(false);
		}
	}
}

