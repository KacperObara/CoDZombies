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
		public override void Awake()
		{
			base.Awake();
			CustomIndex = 0;
		}

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

		public static int CustomIndex = 0;
		// Stolen from Server.cs, original only supports 10 custom packet handlers
		public static int RegisterCustomPacketType(string handlerID, int clientID = 0)
		{
			int index = -1;

			// index = CustomIndex;
			// CustomIndex++;
			// return index;

			if (Mod.registeredCustomPacketIDs.TryGetValue(handlerID, out index))
			{
				Mod.LogWarning("Client " + clientID + " requested for " + handlerID + " custom packet handler to be registered but this ID already exists.");
			}
			else // We don't yet have this handlerID, add it
			{
				index = CustomIndex;
				CustomIndex++;

				// If couldn't find one, need to add more space to handlers array
				// if (index >= Mod.customPacketHandlers.Length)
				// {
				// 	//index = Mod.customPacketHandlers.Length;
				// 	Mod.CustomPacketHandler[] temp = Mod.customPacketHandlers;
				// 	Mod.customPacketHandlers = new Mod.CustomPacketHandler[index + 1];
				// 	for (int i = 0; i < temp.Length; ++i)
				// 	{
				// 		Mod.customPacketHandlers[i] = temp[i];
				// 	}
				// 	for (int i = temp.Length; i < Mod.customPacketHandlers.Length; ++i) 
				// 	{
				// 		Mod.availableCustomPacketIndices.Add(i);
				// 	}
				// }

				// Store for potential later use
				Mod.registeredCustomPacketIDs.Add(handlerID, index);

				// Send event so a mod can add their handler at the index
				Mod.CustomPacketHandlerReceivedInvoke(handlerID, index);
			}

			// Send back/relay to others
			ServerSend.RegisterCustomPacketType(handlerID, index);

			return index;
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

		// // Players don't include host, so -1 is host
		// public static int GetRandomPlayerId()
		// {
		// 	if (!ServerRunning())
		// 		return -1;
		// 	
		// 	if (GameManager.players.Count > 0)
		// 	{
		// 		int randomPlayer = Random.Range(0, GameManager.players.Count + 1);
		// 		if (randomPlayer == GameManager.players.Count)
		// 			return -1;
		// 		return randomPlayer;
		// 	}
		// 	else
		// 	{
		// 		return -1;
		// 	}
		// }

		public static bool IsMineIFF(int iff)
		{
			if (GM.CurrentPlayerBody.GetPlayerIFF() == iff)
				return true;

			return false;
		}
	}
}