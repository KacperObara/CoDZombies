using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using H3MP.Networking;
using UnityEngine;

public class ZombieNetworking : MonoBehaviourSingleton<ZombieNetworking> 
{
    public bool isClient = false;
    
    private void StartNetworking()
    {
        if (Networking.ServerRunning())
        {
            if (Networking.IsHost())
            {
                //Only Sends data if they join the server IN the map.
                //ServerClient.OnSendPostWelcomeData += OnPlayerJoined;
            }

            if (Networking.IsHost() || Client.isFullyConnected)
            {
                //SetupPacketTypes();
            }
        }
    }
}
