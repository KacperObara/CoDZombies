﻿using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using CustomScripts.Multiplayer;
using CustomScripts.Player;
using CustomScripts.Powerups.Perks;
using FistVR;
using UnityEngine;
using UnityEngine.UI;

public class ReviveButton : MonoBehaviourSingleton<ReviveButton>
{
	public Image ReviveIcon;
	private int _affectedPlayerID;
	
	private FVRViveHand _handReviving;

	private float _reviveTime = PlayerData.Instance.QuickRevivePerkActivated ? 1.5f : 3f;
	private float _timer = 0f;

	private Color _defaultColor = Color.yellow;
	private Color _reviveColor = Color.white;

	public void Spawn(int playerID, Vector3 pos)
	{
		gameObject.SetActive(true);
		_affectedPlayerID = playerID;
		transform.position = pos;
	}

	public void Despawn()
	{
		gameObject.SetActive(false);
	}
	
	private void OnTriggerStay(Collider other)
	{
		if (other.GetComponent<FVRViveHand>())
		{
			if (_handReviving == null)
				_handReviving = other.GetComponent<FVRViveHand>();
			
			if (_handReviving.Input.GripPressed)
			{
				ReviveIcon.color = _reviveColor;
				_timer += Time.fixedDeltaTime;
				if (_timer >= _reviveTime)
				{
					if (Networking.IsHost())
					{
						CodZNetworking.Instance.CustomData_PlayerID_Send(_affectedPlayerID, (int)CustomPlayerDataType.PLAYER_REVIVED);
					}
					else
					{
						CodZNetworking.Instance.Client_CustomData_PlayerID_Send(_affectedPlayerID, (int)CustomPlayerDataType.PLAYER_REVIVED);
					}

					Despawn();
					//PlayerData.Instance.Revive();
					//Destroy(gameObject);
				}
			}
			else
			{
				_timer = 0f;
				ReviveIcon.color = _defaultColor;
			}
		}
	}

	private void Update()
	{
		transform.LookAt(transform.position + GM.CurrentPlayerBody.Head.transform.rotation * Vector3.forward, GM.CurrentPlayerBody.Head.transform.rotation * Vector3.up);
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<FVRViveHand>())
		{
			//_handsInside.Remove(other.GetComponent<FVRViveHand>());
		}
	}
}
