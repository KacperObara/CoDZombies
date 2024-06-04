using System;
using System.Collections.Generic;
using CustomScripts;
using FistVR;
using H3MP;
using H3MP.Networking;
using H3MP.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomScripts.Multiplayer
{
	public class Networking : MonoBehaviourSingleton<Networking> 
	{
		public static bool IsHostOrSolo()
		{
			if (ServerRunning())
			{
				if (IsHost())
					return true;
				return false;
			}
			
			return true;
		}
		
		public static bool ServerRunning()
		{
			return true;
		}
	
		public static bool IsClient()
		{
			if (ThreadManager.host == false)
				return true;
			return false;
		}
	
		public static bool IsHost()
		{
			if (ThreadManager.host == true)
				return true;
			return false;
		}
	
		// public static int GetPlayerCount()
		// {
		// 	if (GMgr.H3mpEnabled)
		// 		return GetNetworkPlayerCount();
		// 	return 1;
		// }
		//
		// static int GetNetworkPlayerCount()
		// {
		// 	return GameManager.players.Count;
		// }
	
		/// <summary>
		/// Returns array of all players (Not including local player) IDs
		/// </summary>
		/// <returns></returns>
		// public static int[] GetPlayerIDs()
		// {
		// 	int[] playerArray = new int[GameManager.players.Count];
		//
		// 	int i = 0;
		// 	foreach (KeyValuePair<int, PlayerManager> entry in GameManager.players)
		// 	{
		// 		playerArray[i] = entry.Key;
		// 		i++;
		// 	}
		//
		// 	return playerArray;
		// }

		// /// <summary>
		// /// Returns the local players id.
		// /// </summary>
		// /// <returns></returns>
		// public static int GetLocalID()
		// {
		// 	return GameManager.ID;
		// }
		//
		//
		// /// <summary>
		// /// Returns the Custom Packet ID
		// /// </summary>
		// /// <param name="identifier"></param>
		// /// <returns></returns>
		// public static int RegisterHostCustomPacket(string identifier)
		// {
		// 	int id;
		// 	if (Mod.registeredCustomPacketIDs.ContainsKey(identifier))
		// 		id = Mod.registeredCustomPacketIDs[identifier];
		// 	else
		// 		id = Server.RegisterCustomPacketType(identifier);
		//
		// 	return id;
		// }

		/// <summary>
		/// Returns the Gamemanager player at index i, does not include the local player.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		// public static PlayerH3MPData GetPlayer(int i)
		// {
		// 	//Do error Checks
		// 	return PlayerH3MPData.GetPlayer(i);
		// }

		// Players don't include host, so -1 is host
		public static int GetRandomPlayerId()
		{
			if (!ServerRunning())
				return -1;
			
			if (GameManager.players.Count > 0)
			{
				int randomPlayer = Random.Range(0, GameManager.players.Count + 1);
				if (randomPlayer == GameManager.players.Count)
					return -1;
				return randomPlayer;
			}
			else
			{
				return -1;
			}
		}

		public static bool IsMineIFF(int iff)
		{
			if (GM.CurrentPlayerBody.GetPlayerIFF() == iff)
				return true;

			return false;
		}
		
		public List<PlayerH3MPData> Players = new List<PlayerH3MPData>();

		public PlayerH3MPData GetRandomPlayer()
		{
			int randomPlayer = Random.Range(0, Players.Count);
			return Players[randomPlayer];
		}

		private void Start()
		{
			//AddHost();
			//GameManager.OnSceneJoined
		}

		public void AddHost(int ID)
		{
			Players.Add(new PlayerH3MPData
			{
				ID = ID,
				IsHost = true,
				PlayerManager = null
			});
		}

		public void AddClient(PlayerManager playerManager)
		{
			Players.Add(new PlayerH3MPData
			{
				ID = playerManager.ID,
				IsHost = true,
				PlayerManager = playerManager
			});
		}
	}
	
	public class PlayerH3MPData
	{
		public int ID;
		public bool IsHost;
		public PlayerManager PlayerManager;
		
		public Transform GetHead()
		{
			if (IsHost)
				return GM.CurrentPlayerBody.Head;
			return PlayerManager.head;
		}
		
		//public string username;
		//public Transform head;
		//public Transform handLeft;
		//public Transform handRight;
		//public float health;
		//public int iff;

		// public static PlayerH3MPData GetPlayer(int i)
		// {
		// 	return new PlayerH3MPData
		// 	{
		// 		head = GameManager.players[i].head,
		// 		username = GameManager.players[i].username,
		// 		handLeft = GameManager.players[i].leftHand,
		// 		handRight = GameManager.players[i].rightHand,
		// 		iff = i + 5, // Every player has unique IFF so we know who killed zombies
		// 		
		// 	};
		// }
	}
}