using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomScripts;
using CustomScripts.Multiplayer;
using FistVR;
using H3MP;
using H3MP.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayersMgr : MonoBehaviourSingleton<PlayersMgr> 
{
	public List<PlayerH3MPData> Players = new List<PlayerH3MPData>();
	public static PlayerH3MPData Me;
	
	public ReviveButton ReviveButtonPrefab;
	public List<ReviveButton> ReviveButtons = new List<ReviveButton>();
	
	private void Start()
	{
		AddMe();
		RoundManager.OnGameStarted += AddClients;
		RoundManager.RoundStarted += SetAllToAlive;
	}

	private void AddClients()
	{
		foreach (var player in GameManager.players)
		{
			AddClient(player.Value);
		}

		Debug.Log("Connected Players");
		foreach (var player in Players)
		{
			if (player.IsMe)
				Debug.Log("Me " + GameManager.ID);
			else
				Debug.Log(player.PlayerManager.username + " " + player.PlayerManager.ID);
		}

		foreach (var player in Players)
		{
			if (!player.IsMe)
				Debug.Log("IFF: " + (player.PlayerManager.ID + 5));
		}
	}

	public PlayerH3MPData GetRandomPlayer()
	{
		int randomPlayer = Random.Range(0, Players.Count);
		return Players[randomPlayer];
	}
	
	public PlayerH3MPData GetRandomAlivePlayer()
	{
		List<PlayerH3MPData> alivePlayers = Players.FindAll(player => player.IsAlive);
		int randomPlayer = Random.Range(0, alivePlayers.Count);
		return alivePlayers[randomPlayer];
	}

	public PlayerH3MPData GetClosestAlivePlayer(Vector3 origin)
	{
		PlayerH3MPData closestPlayer = null;
		float closestDistanceSqr = float.MaxValue;

		foreach (var player in Players)
		{
			if (player.IsAlive)
			{
				float distanceSqr = (player.GetHead().position - origin).sqrMagnitude;
				if (distanceSqr < closestDistanceSqr)
				{
					closestDistanceSqr = distanceSqr;
					closestPlayer = player;
				}
			}
		}

		return closestPlayer;
	}
	
	public PlayerH3MPData GetClosestPlayer(Vector3 origin)
	{
		PlayerH3MPData closestPlayer = null;
		float closestDistanceSqr = float.MaxValue;

		foreach (var player in Players)
		{
			float distanceSqr = (player.GetHead().position - origin).sqrMagnitude;
			if (distanceSqr < closestDistanceSqr)
			{
				closestDistanceSqr = distanceSqr;
				closestPlayer = player;
			}
		}

		return closestPlayer;
	}
	
	public bool AllPlayersDowned()
	{
		return Players.All(player => !player.IsAlive);
	}

	public void AddMe()
	{
		Players.Add(new PlayerH3MPData
		{
			IsMe = true,
			PlayerManager = null
		});
		Me = Players[Players.Count - 1];
	}

	public void AddClient(PlayerManager playerManager)
	{
		Players.Add(new PlayerH3MPData
		{
			IsMe = false,
			PlayerManager = playerManager
		});
	}

	private void OnDestroy()
	{
		RoundManager.OnGameStarted -= AddClients;
		RoundManager.RoundStarted -= SetAllToAlive;
	}

	public static PlayerH3MPData GetPlayerExcludingMe(int playerID)
	{
		return Instance.Players.Find(player => !player.IsMe && player.PlayerManager.ID == playerID);
	}
	
	public static void SpawnReviveButton(int playerID, Vector3 pos)
	{
		ReviveButton button = Instantiate(Instance.ReviveButtonPrefab, pos, Quaternion.identity);
		button.Spawn(playerID, pos);
		//Debug.Log("Revive button spawned: " + playerID);
	}
	
	public static void DespawnReviveButton(int playerID)
	{
		//Debug.Log("Revive button despawned " + playerID);
		ReviveButton reviveButton = Instance.ReviveButtons.Find(button => button.AffectedPlayerID == playerID);
		reviveButton.Despawn();
	}

	public void SetAllToAlive()
	{
		foreach (var player in Players)
		{
			player.IsDowned = false;
			player.IsDead = false;
		}
	}
}

// Making my own list of players since GameManager.players doesn't include host
// Host is always the first player in the list, list exists only for the host
public class PlayerH3MPData
{
	public bool IsMe;
	public PlayerManager PlayerManager;
	public bool IsDowned;
	public bool IsDead;
	public bool Exists { get { return IsMe || PlayerManager != null; } } // Handle disconnects
	public bool IsAlive { get { return !IsDowned && !IsDead && Exists; } }
	
	public Transform GetHead()
	{
		if (IsMe)
			return GM.CurrentPlayerBody.Head;
		return PlayerManager.head;
	}
}
