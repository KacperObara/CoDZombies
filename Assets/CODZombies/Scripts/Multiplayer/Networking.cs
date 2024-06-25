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
			return GameManager.singleton != null;
		}
	
		public static bool IsClient()
		{
			if (!ThreadManager.host)
				return true;
			return false;
		}
	
		public static bool IsHost()
		{
			if (ThreadManager.host)
				return true;
			return false;
		}

		public static bool IsSolo()
		{
			if (ServerRunning())
			{
				if (IsHost() && PlayersMgr.Instance.Players.Count == 1)
					return true;

				return false;
			}

			return true;
		}

		public static int CustomIndex = 0;
		// Stolen from Server.cs, original only supports 10 custom packet handlers
		public static int RegisterCustomPacketType(string handlerID, int clientID = 0)
		{
			int index = -1;

			if (Mod.registeredCustomPacketIDs.TryGetValue(handlerID, out index))
			{
				Mod.LogWarning("Client " + clientID + " requested for " + handlerID + " custom packet handler to be registered but this ID already exists.");
			}
			else // We don't yet have this handlerID, add it
			{
				index = CustomIndex;
				CustomIndex++;

				// Store for potential later use
				Mod.registeredCustomPacketIDs.Add(handlerID, index);

				// Send event so a mod can add their handler at the index
				Mod.CustomPacketHandlerReceivedInvoke(handlerID, index);
			}

			// Send back/relay to others
			ServerSend.RegisterCustomPacketType(handlerID, index);

			return index;
		}

		public static bool IsMineIFF(int iff)
		{
			if (GM.CurrentPlayerBody.GetPlayerIFF() == iff)
				return true;

			return false;
		}
	}
}