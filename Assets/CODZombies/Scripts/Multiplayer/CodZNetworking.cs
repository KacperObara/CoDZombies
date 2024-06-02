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
        private int powerEnabled_ID = -1;
        private int blockadeCleared_ID = -1;
        private int mysteryBoxMoved_ID = -1;
        private int powerUpSpawned_ID = -1;
        private int powerUpCollected_ID = -1;
        
        void Start()
        {
            if(GMgr.H3mpEnabled)
                StartNetworking();
        }

        
        private void StartNetworking()
        {
            if (Networking.ServerRunning())
            {
                SetupPacketTypes();
                
                if (Networking.IsClient())
                    isClient = true;

                for (int i = 0; i < GameManager.players.Count; i++)
                {
                    GameManager.players[i].SetIFF(i + 5);
                }
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
                
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_PowerEnabled"))
                    powerEnabled_ID = Mod.registeredCustomPacketIDs["CodZ_PowerEnabled"];
                else
                    powerEnabled_ID = Server.RegisterCustomPacketType("CodZ_PowerEnabled");
                Mod.customPacketHandlers[powerEnabled_ID] = PowerEnabled_Handler;       
                
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_BlockadeCleared"))
                    blockadeCleared_ID = Mod.registeredCustomPacketIDs["CodZ_BlockadeCleared"];
                else
                    blockadeCleared_ID = Server.RegisterCustomPacketType("CodZ_BlockadeCleared");
                Mod.customPacketHandlers[blockadeCleared_ID] = BlockadeCleared_Handler;
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
                
                //Power Enabled
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_PowerEnabled"))
                {
                    powerEnabled_ID = Mod.registeredCustomPacketIDs["CodZ_PowerEnabled"];
                    Mod.customPacketHandlers[powerEnabled_ID] = PowerEnabled_Handler;
                }
                else
                {
                    ClientSend.RegisterCustomPacketType("CodZ_PowerEnabled");
                    Mod.CustomPacketHandlerReceived += PowerEnabled_Received;
                }   
                
                //Blockade Cleared
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_BlockadeCleared"))
                {
                    blockadeCleared_ID = Mod.registeredCustomPacketIDs["CodZ_BlockadeCleared"];
                    Mod.customPacketHandlers[blockadeCleared_ID] = PowerEnabled_Handler;
                }
                else
                {
                    ClientSend.RegisterCustomPacketType("CodZ_BlockadeCleared");
                    Mod.CustomPacketHandlerReceived += BlockadeCleared_Received;
                }
            }
        }

        //Game Started -------------------------------------
        public void StartGame_Send()
        {
            if (!Networking.ServerRunning() || Networking.IsClient())
                return;
            
            Packet packet = new Packet(gameStarted_ID);
            packet.Write(GameSettings.HardMode);
            packet.Write(GameSettings.WeakerEnemiesEnabled);
            packet.Write(GameSettings.SpecialRoundDisabled);
            packet.Write(GameSettings.ItemSpawnerEnabled);

            Debug.Log("Start game packet sent");

            ServerSend.SendTCPDataToAll(packet, true);
        }
        
        private void StartGame_Handler(int clientID, Packet packet)
        {
            GameSettings.HardMode = packet.ReadBool();
            GameSettings.WeakerEnemiesEnabled = packet.ReadBool();
            GameSettings.SpecialRoundDisabled = packet.ReadBool();
            GameSettings.ItemSpawnerEnabled = packet.ReadBool();
            
            GameSettings.OnSettingsChanged.Invoke();

            Debug.Log("Start game packet received");
            
            RoundManager.Instance.StartGame();
        }
        
        private void StartGame_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_GameStarted")
            {
                Debug.Log("Start Game Received");
                gameStarted_ID = index;
                Mod.customPacketHandlers[index] = StartGame_Handler;
                Mod.CustomPacketHandlerReceived -= StartGame_Received;
            }
        }
        
        
        //Power Enabled -------------------------------------
        public void PowerEnabled_Send()
        {
            if (!Networking.ServerRunning() || Networking.IsHost())
                return;

            Debug.Log("Client Sending PowerEnabled");

            Packet packet = new Packet(powerEnabled_ID);
            ClientSend.SendTCPData(packet, true);
        }
        
        void PowerEnabled_Handler(int clientID, Packet packet)
        {
            Debug.Log("Received Client PowerEnabled");
            GMgr.Instance.TurnOnPower();
        }

        void PowerEnabled_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_PowerEnabled")
            {
                powerEnabled_ID = index;
                Mod.customPacketHandlers[index] = PowerEnabled_Handler;
                Mod.CustomPacketHandlerReceived -= PowerEnabled_Received;
            }
        }
        
        
        //Blockade Cleared -------------------------------------
        public void BlockadeCleared_Send(int blockadeId)
        {
            if (!Networking.ServerRunning() || Networking.IsHost())
                return;

            Debug.Log("Client Sending BlockadeCleared");

            Packet packet = new Packet(blockadeCleared_ID);
            packet.Write(blockadeId);
            
            ClientSend.SendTCPData(packet, true);
        }
        
        void BlockadeCleared_Handler(int clientID, Packet packet)
        {
            Debug.Log("Received Client BlockadeCleared");
            int blockadeId = packet.ReadInt();
            GMgr.Instance.Blockades[blockadeId].Unlock();
        }

        void BlockadeCleared_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_BlockadeCleared")
            {
                blockadeCleared_ID = index;
                Mod.customPacketHandlers[index] = PowerEnabled_Handler;
                Mod.CustomPacketHandlerReceived -= PowerEnabled_Received;
            }
        }
    }
}

