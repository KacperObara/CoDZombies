using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomScripts.Multiplayer
{
	public class HideIfNotHost : MonoBehaviour {
		private void Start()
		{
			if (Networking.IsClient())
				gameObject.SetActive(false);
			else
			{
				Debug.Log("Host is running, object is active.");
			}
		}
	}
}

