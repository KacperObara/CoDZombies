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
	//public static PlayerH3MPData HostData { get { return Instance.Players[0]; } }
	
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
	}

	public PlayerH3MPData GetRandomPlayer()
	{
		int randomPlayer = Random.Range(0, Players.Count);
		return Players[randomPlayer];
	}
	
	public PlayerH3MPData GetRandomAlivePlayer()
	{
		List<PlayerH3MPData> alivePlayers = Players.FindAll(player => !player.IsDowned && !player.IsDead);
		int randomPlayer = Random.Range(0, alivePlayers.Count);
		return alivePlayers[randomPlayer];
	}

	public PlayerH3MPData GetClosestAlivePlayer(Vector3 origin)
	{
		List<PlayerH3MPData> alivePlayers = Players.FindAll(player => !player.IsDowned && !player.IsDead);
		return alivePlayers.OrderBy(player => Vector3.Distance(player.GetHead().position, origin)).First();
	}

	public void AddMe()
	{
		Players.Add(new PlayerH3MPData
		{
			IsMe = true,
			PlayerManager = null
		});
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
}

// Making my own list of players since GameManager.players doesn't include host
// Host is always the first player in the list, list exists only for the host
public class PlayerH3MPData
{
	public bool IsMe;
	public PlayerManager PlayerManager;
	public bool IsDowned;
	public bool IsDead;

	public Transform GetHead()
	{
		if (IsMe)
			return GM.CurrentPlayerBody.Head;
		return PlayerManager.head;
	}
}
