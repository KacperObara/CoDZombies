using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfNotHost : MonoBehaviour {
	private void Start()
	{
		if (Networking.IsClient())
			gameObject.SetActive(false);
	}
}
