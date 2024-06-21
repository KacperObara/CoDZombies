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
	
	private void Start()
	{
		AddMe();
		RoundManager.OnGameStarted += AddClients;
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
		List<PlayerH3MPData> alivePlayers = Players.FindAll(player => player.IsAlive);
		return alivePlayers.OrderBy(player => Vector3.Distance(player.GetHead().position, origin)).First();
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
	}

	public static PlayerH3MPData GetPlayerExcludingMe(int playerID)
	{
		return Instance.Players.Find(player => !player.IsMe && player.PlayerManager.ID == playerID);
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
