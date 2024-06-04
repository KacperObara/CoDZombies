using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts.Player;
using CustomScripts.Powerups.Perks;
using FistVR;
using UnityEngine;

public class ReviveButton : MonoBehaviour
{
	private FVRViveHand _handReviving;

	private float _reviveTime = PlayerData.Instance.QuickRevivePerkActivated ? 1.5f : 3f;
	private float _timer = 0f;

	private Color _defaultColor = Color.yellow;
	private Color _reviveColor = Color.white;
	
	private void OnTriggerStay(Collider other)
	{
		if (other.GetComponent<FVRViveHand>())
		{
			if (_handReviving == null)
				_handReviving = other.GetComponent<FVRViveHand>();
			
			if (_handReviving.Input.GripPressed)
			{
				_timer += Time.fixedDeltaTime;
				if (_timer >= _reviveTime)
				{
					//PlayerData.Instance.Revive();
					//Destroy(gameObject);
				}
			}
			else
			{
				_timer = 0f;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<FVRViveHand>())
		{
			//_handsInside.Remove(other.GetComponent<FVRViveHand>());
		}
	}
}
