using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using CustomScripts.Player;
using FistVR;
using UnityEngine;

public class ElectricCherryPerkBottle : MonoBehaviour, IModifier
{
	private Vector3 _originalPosition;
        
	private void Start()
	{
		_originalPosition = transform.parent.position;
	}
	
	public void ApplyModifier()
	{
		PlayerData.Instance.ElectricCherryPerkActivated = true;
		AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
		GetComponent<FVRPhysicalObject>().ForceBreakInteraction();
		transform.parent.position = _originalPosition;
	}
}
