using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using CustomScripts.Managers;
using CustomScripts.Zombie;
using FistVR;
using UnityEngine;
using UnityEngine.Events;

public class SosigTrigger : MonoBehaviour 
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Sosig>())
		{
			other.GetComponent<Sosig>().GetComponent<ZombieController>().OnHit(9999);
		}
	}
}
