using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using CustomScripts.Gamemode;
using CustomScripts.Multiplayer;
using CustomScripts.Player;
using CustomScripts.Powerups.Perks;
using FistVR;
using UnityEngine;
using UnityEngine.UI;

public class ReviveButton : MonoBehaviourSingleton<ReviveButton>
{
	public Image ReviveIcon;
	public int AffectedPlayerID;
	
	private FVRViveHand _handReviving;

	private float ReviveTime
	{
		get { return PlayerData.Instance.QuickRevivePerkActivated ? 2.5f : 5f; }
	}
	private float _timer = 0f;

	private Color _defaultColor = Color.yellow;
	private Color _reviveColor = Color.white;
	
	public void Spawn(int playerID, Vector3 pos)
	{
		Debug.Log("Revive button spawned");
		gameObject.SetActive(true);
		AffectedPlayerID = playerID;
		transform.position = pos + Vector3.up;
		_timer = 0f;
		ReviveIcon.color = _defaultColor;
		PlayersMgr.Instance.ReviveButtons.Add(this);
	} 

	public void Despawn()
	{
		gameObject.SetActive(false);
		PlayersMgr.Instance.ReviveButtons.Remove(this);
		Destroy(gameObject);
	}
	
	private void OnTriggerStay(Collider other)
	{
		if (other.GetComponent<FVRViveHand>())
		{
			if (_handReviving == null)
				_handReviving = other.GetComponent<FVRViveHand>();

			//Debug.Log("Hand inside revive trigger");
			if (_handReviving.Input.GripPressed)
			{
				//Debug.Log("Grip pressed on revive button");
				
				ReviveIcon.color = _reviveColor;
				_timer += Time.fixedDeltaTime;
				if (_timer >= ReviveTime)
				{
					if (Networking.IsHost())
					{
						CodZNetworking.Instance.CustomData_PlayerID_Send(AffectedPlayerID, (int)CustomPlayerDataType.PLAYER_REVIVED);
					}
					else
					{
						CodZNetworking.Instance.Client_CustomData_PlayerID_Send(AffectedPlayerID, (int)CustomPlayerDataType.PLAYER_REVIVED);
					}

					//Despawn();
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
