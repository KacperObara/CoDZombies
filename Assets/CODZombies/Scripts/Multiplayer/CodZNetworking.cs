using H3MP;
using H3MP.Networking;
using UnityEngine;

namespace CustomScripts.Multiplayer
{
    public class CodZNetworking : MonoBehaviourSingleton<CodZNetworking> 
    {
        public bool isClient = false;
        
        //Packet IDs
        private int gameStarted_ID = -1;
        private int serverRunning_ID = -1;
        private int requestSync_ID = -1;
        
        void Start()
        {
            if(GMgr.H3mpEnabled)
                StartNetworking();
        }

        
        private void StartNetworking()
        {
            if (Networking.ServerRunning())
            {
                if (Networking.IsHost())
                {
                    //Only Sends data if they join the server IN the map.
                    //ServerClient.OnSendPostWelcomeData += OnPlayerJoined;
                }

                // if (Networking.IsHost() || Client.isFullyConnected)
                // {
                //     SetupPacketTypes();
                // }
            }
        }

        private void SetupPacketTypes()
        {
            if (Networking.IsHost())
            {
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_GameStarted"))
                    gameStarted_ID = Mod.registeredCustomPacketIDs["CodZ_GameStarted"];
                else
                    gameStarted_ID = Server.RegisterCustomPacketType("CodZ_GameStarted");
                Mod.customPacketHandlers[gameStarted_ID] = StartGame_Handler;
                
                
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_ServerRunning"))
                    serverRunning_ID = Mod.registeredCustomPacketIDs["CodZ_ServerRunning"];
                else
                    serverRunning_ID = Server.RegisterCustomPacketType("CodZ_ServerRunning");
                Mod.customPacketHandlers[serverRunning_ID] = ServerRunning_Handler;
                
                
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_RequestSync"))
                    requestSync_ID = Mod.registeredCustomPacketIDs["CodZ_RequestSync"];
                else
                    requestSync_ID = Server.RegisterCustomPacketType("CodZ_RequestSync");
                Mod.customPacketHandlers[requestSync_ID] = RequestSync_Handler;
            }
            else
            {
                //Game Started
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_GameStarted"))
                {
                    gameStarted_ID = Mod.registeredCustomPacketIDs["CodZ_GameStarted"];
                    Mod.customPacketHandlers[gameStarted_ID] = StartGame_Handler;
                }
                else
                {
                    ClientSend.RegisterCustomPacketType("CodZ_GameStarted");
                    Mod.CustomPacketHandlerReceived += StartGame_Received;
                }
                
                //Server Running
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_ServerRunning"))
                {
                    serverRunning_ID = Mod.registeredCustomPacketIDs["CodZ_ServerRunning"];
                    Mod.customPacketHandlers[serverRunning_ID] = ServerRunning_Handler;
                }
                else
                {
                    ClientSend.RegisterCustomPacketType("CodZ_ServerRunning");
                    Mod.CustomPacketHandlerReceived += ServerRunning_Received;
                }
                
                //Request Sync
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_RequestSync"))
                {
                    requestSync_ID = Mod.registeredCustomPacketIDs["CodZ_RequestSync"];
                    Mod.customPacketHandlers[requestSync_ID] = RequestSync_Handler;
                }
                else
                {
                    ClientSend.RegisterCustomPacketType("CodZ_RequestSync");
                    Mod.CustomPacketHandlerReceived += RequestSync_Received;
                }
            }
        }

        public void StartGame_Send()
        {
            if (!Networking.ServerRunning() || Networking.IsClient())
                return;
            
            Packet packet = new Packet(gameStarted_ID);
            packet.Write(GameSettings.HardMode);
            packet.Write(GameSettings.WeakerEnemiesEnabled);
            packet.Write(GameSettings.SpecialRoundDisabled);
            packet.Write(GameSettings.ItemSpawnerEnabled);

            ServerSend.SendTCPDataToAll(packet, true);
        }
        
        private void StartGame_Handler(int clientID, Packet packet)
        {
            GameSettings.HardMode = packet.ReadBool();
            GameSettings.WeakerEnemiesEnabled = packet.ReadBool();
            GameSettings.SpecialRoundDisabled = packet.ReadBool();
            GameSettings.ItemSpawnerEnabled = packet.ReadBool();
            
            GameSettings.OnSettingsChanged.Invoke();
            
            RoundManager.Instance.StartGame();
        }
        
        private void StartGame_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_GameStarted")
            {
                gameStarted_ID = index;
                Mod.customPacketHandlers[index] = StartGame_Handler;
                Mod.CustomPacketHandlerReceived -= StartGame_Received;
            }
        }
        
        public void ServerRunning_Send()
        {
            if (!Networking.ServerRunning() || Networking.IsClient())
                return;

            Packet packet = new Packet(serverRunning_ID);
            packet.Write(GMgr.Instance.GameServerRunning);
            
            ServerSend.SendTCPDataToAll(packet, true);
        }

        //Server Running Received
        private void ServerRunning_Handler(int clientID, Packet packet)
        {
            bool status = packet.ReadBool();
            GMgr.Instance.GameServerRunning = status;
            //SR_Menu.instance.lauchGameButton.SetActive(status);
        }
        
        private void ServerRunning_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_ServerRunning")
            {
                serverRunning_ID = index;
                Mod.customPacketHandlers[index] = ServerRunning_Handler;
                Mod.CustomPacketHandlerReceived -= ServerRunning_Received;
            }
        }
        
        //RequestSync Send ----------------------------------------------
        public void RequestSync_Send()
        {
            if (!Networking.ServerRunning() || Networking.IsHost())
                return;

            Debug.Log("Client Sending Request Sync");

            Packet packet = new Packet(requestSync_ID);
            ClientSend.SendTCPData(packet, true);
        }

        //RequestSync Received TODO make corouteen
        void RequestSync_Handler(int clientID, Packet packet)
        {
            Debug.Log("Received Client Request Sync");

            // GameOptions_Send();
            // LevelUpdate_Send(SR_Manager.instance.gameCompleted);
            // UpdateStats_Send();
            ServerRunning_Send();
        }

        void RequestSync_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_RequestSync")
            {
                requestSync_ID = index;
                Mod.customPacketHandlers[index] = RequestSync_Handler;
                Mod.CustomPacketHandlerReceived -= RequestSync_Received;
                    
                //Clients request data once handler is setup
                RequestSync_Send();
            }
        }
    }
}

