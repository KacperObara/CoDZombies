using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using H3MP;
using H3MP.Networking;
using H3MP.Scripts;
using UnityEngine;

public class Networking : MonoBehaviourSingleton<Networking> 
{
	public static bool ServerRunning()
	{
		if (GMgr.H3mpEnabled)
			return isServerRunning();

		return false;
	}
	
	static bool isServerRunning()
	{
		if (Mod.managerObject == null)
			return false;

		return true;
	}
	
	public static bool IsClient()
	{
		if (GMgr.H3mpEnabled)
			return isClient();

		return false;
	}
	
	static bool isClient()
	{
		if (Mod.managerObject == null)
			return false;

		if (ThreadManager.host == false)
			return true;
		return false;
	}
	
	public static bool IsHost()
	{
		if (GMgr.H3mpEnabled)
			return isHosting();

		return false;
	}
	
	static bool isHosting()
	{
		if (Mod.managerObject == null)
			return false;

		if (ThreadManager.host == true)
			return true;
		return false;
	}
	
	public static int GetPlayerCount()
	{
		if (GMgr.H3mpEnabled)
			return GetNetworkPlayerCount();
		return 1;
	}
	
	static int GetNetworkPlayerCount()
	{
		return GameManager.players.Count;
	}
	
	/// <summary>
	/// Returns array of all players (Not including local player) IDs
	/// </summary>
	/// <returns></returns>
	public static int[] GetPlayerIDs()
	{
		int[] playerArray = new int[GameManager.players.Count];

		int i = 0;
		foreach (KeyValuePair<int, PlayerManager> entry in GameManager.players)
		{
			playerArray[i] = entry.Key;
			i++;
		}

		return playerArray;
	}

	/// <summary>
	/// Returns the local players id.
	/// </summary>
	/// <returns></returns>
	public static int GetLocalID()
	{
		return GameManager.ID;
	}
	
	/// <summary>
	/// Returns the Custom Packet ID
	/// </summary>
	/// <param name="identifier"></param>
	/// <returns></returns>
	public static int RegisterHostCustomPacket(string identifier)
	{
		int id;
		if (Mod.registeredCustomPacketIDs.ContainsKey(identifier))
			id = Mod.registeredCustomPacketIDs[identifier];
		else
			id = Server.RegisterCustomPacketType(identifier);

		return id;
	}
	
	/// <summary>
	/// Returns the Gamemanager player at index i, does not include the local player.
	/// </summary>
	/// <param name="i"></param>
	/// <returns></returns>
	public static PlayerH3MPData GetPlayer(int i)
	{
		//Do error Checks
		return PlayerH3MPData.GetPlayer(i);
	}
}

public class PlayerH3MPData
{
	public Transform head;
	public string username;
	public Transform handLeft;
	public Transform handRight;
	public int ID;
	public float health;
	public int iff;

	public static PlayerH3MPData GetPlayer(int i)
	{
		return new PlayerH3MPData
		{
			head = GameManager.players[i].head,
			username = GameManager.players[i].username,
			handLeft = GameManager.players[i].leftHand,
			handRight = GameManager.players[i].rightHand,
		};
	}
}
