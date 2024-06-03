using System;
using System.Linq;
using FistVR;
using H3MP;
using H3MP.Networking;
using UnityEngine;

namespace CustomScripts.Multiplayer
{
    public class CodZNetworking : MonoBehaviourSingleton<CodZNetworking> 
    {
        //Packet IDs
        private int gameStarted_ID = -1;
        
        private int powerEnabled_ID = -1;
        private int powerEnabled_Client_ID = -1;
        
        private int blockadeCleared_ID = -1;
        private int blockadeCleared_Client_ID = -1;
        
        private int customData_ID = -1;
        private int customData_Client_ID = -1;
        
        // private int mysteryBoxMoved_ID = -1;
        // private int powerUpSpawned_ID = -1;
        // private int powerUpCollected_ID = -1;
        
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
                
                // Power Enabled
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_PowerEnabled"))
                    powerEnabled_ID = Mod.registeredCustomPacketIDs["CodZ_PowerEnabled"];
                else
                    powerEnabled_ID = Server.RegisterCustomPacketType("CodZ_PowerEnabled");
                Mod.customPacketHandlers[powerEnabled_ID] = PowerEnabled_Handler;       
                
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_Client_PowerEnabled"))
                    powerEnabled_Client_ID = Mod.registeredCustomPacketIDs["CodZ_Client_PowerEnabled"];
                else
                    powerEnabled_Client_ID = Server.RegisterCustomPacketType("CodZ_Client_PowerEnabled");
                Mod.customPacketHandlers[powerEnabled_Client_ID] = Client_PowerEnabled_Handler;       
                
                
                // Blockade Cleared
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_BlockadeCleared"))
                    blockadeCleared_ID = Mod.registeredCustomPacketIDs["CodZ_BlockadeCleared"];
                else
                    blockadeCleared_ID = Server.RegisterCustomPacketType("CodZ_BlockadeCleared");
                Mod.customPacketHandlers[blockadeCleared_ID] = BlockadeCleared_Handler;
                
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_Client_BlockadeCleared"))
                    blockadeCleared_Client_ID = Mod.registeredCustomPacketIDs["CodZ_Client_BlockadeCleared"];
                else
                    blockadeCleared_Client_ID = Server.RegisterCustomPacketType("CodZ_Client_BlockadeCleared");
                Mod.customPacketHandlers[blockadeCleared_Client_ID] = Client_BlockadeCleared_Handler;
                
                
                // Custom Data
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_CustomData"))
                    customData_ID = Mod.registeredCustomPacketIDs["CodZ_CustomData"];
                else
                    customData_ID = Server.RegisterCustomPacketType("CodZ_CustomData");
                Mod.customPacketHandlers[customData_ID] = CustomData_Handler;
                
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_Client_CustomData"))
                    customData_Client_ID = Mod.registeredCustomPacketIDs["CodZ_Client_CustomData"];
                else
                    customData_Client_ID = Server.RegisterCustomPacketType("CodZ_Client_CustomData");
                Mod.customPacketHandlers[customData_Client_ID] = Client_CustomData_Handler;
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
                
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_Client_PowerEnabled"))
                {
                    powerEnabled_Client_ID = Mod.registeredCustomPacketIDs["CodZ_Client_PowerEnabled"];
                    Mod.customPacketHandlers[powerEnabled_Client_ID] = Client_PowerEnabled_Handler;
                }
                else
                {
                    ClientSend.RegisterCustomPacketType("CodZ_Client_PowerEnabled");
                    Mod.CustomPacketHandlerReceived += Client_PowerEnabled_Received;
                } 
                
                //Blockade Cleared
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_BlockadeCleared"))
                {
                    blockadeCleared_ID = Mod.registeredCustomPacketIDs["CodZ_BlockadeCleared"];
                    Mod.customPacketHandlers[blockadeCleared_ID] = BlockadeCleared_Handler;
                }
                else
                {
                    ClientSend.RegisterCustomPacketType("CodZ_BlockadeCleared");
                    Mod.CustomPacketHandlerReceived += BlockadeCleared_Received;
                }
                
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_Client_BlockadeCleared"))
                {
                    blockadeCleared_Client_ID = Mod.registeredCustomPacketIDs["CodZ_Client_BlockadeCleared"];
                    Mod.customPacketHandlers[blockadeCleared_Client_ID] = Client_BlockadeCleared_Handler;
                }
                else
                {
                    ClientSend.RegisterCustomPacketType("CodZ_Client_BlockadeCleared");
                    Mod.CustomPacketHandlerReceived += Client_BlockadeCleared_Received;
                }
                
                //Custom Data
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_CustomData"))
                {
                    customData_ID = Mod.registeredCustomPacketIDs["CodZ_CustomData"];
                    Mod.customPacketHandlers[customData_ID] = CustomData_Handler;
                }
                else
                {
                    ClientSend.RegisterCustomPacketType("CodZ_CustomData");
                    Mod.CustomPacketHandlerReceived += CustomData_Received;
                }
                
                if (Mod.registeredCustomPacketIDs.ContainsKey("CodZ_Client_CustomData"))
                {
                    customData_Client_ID = Mod.registeredCustomPacketIDs["CodZ_Client_CustomData"];
                    Mod.customPacketHandlers[customData_Client_ID] = Client_CustomData_Handler;
                }
                else
                {
                    ClientSend.RegisterCustomPacketType("CodZ_Client_CustomData");
                    Mod.CustomPacketHandlerReceived += Client_CustomData_Received;
                }
            }
        }

        #region Game Start

        public void StartGame_Send()
        {
            if (!Networking.ServerRunning() || Networking.IsClient())
                return;
            
            Packet packet = new Packet(gameStarted_ID);
            packet.Write(GameSettings.HardMode);
            packet.Write(GameSettings.WeakerEnemiesEnabled);
            packet.Write(GameSettings.SpecialRoundDisabled);
            packet.Write(GameSettings.ItemSpawnerEnabled);

            try
            {
                GM.CurrentPlayerBody.SetPlayerIFF(5);
                for (int i = 0; i < GameManager.players.Count; i++)
                {
                    //GameManager.players[i].SetIFF(i + 5);
                    ServerSend.PlayerIFF(GameManager.players.ElementAt(i).Key, i + 6);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error + " + e);
            }
            
            
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

        #endregion
        
        #region Power Enabled

        // Host
        public void PowerEnabled_Send()
        {
            if (!Networking.ServerRunning() || Networking.IsClient())
                return;
            
            Packet packet = new Packet(powerEnabled_ID);
            ServerSend.SendTCPDataToAll(packet, true);
        }
        
        void PowerEnabled_Handler(int clientID, Packet packet)
        {
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
        
        // Client
        public void Client_PowerEnabled_Send()
        {
            if (!Networking.ServerRunning() || Networking.IsHost())
                return;

            Packet packet = new Packet(powerEnabled_Client_ID);
            ClientSend.SendTCPData(packet, true);
        }
        
        void Client_PowerEnabled_Handler(int clientID, Packet packet)
        {
            GMgr.Instance.TurnOnPower();
            PowerEnabled_Send();
        }

        void Client_PowerEnabled_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_Client_PowerEnabled")
            {
                powerEnabled_Client_ID = index;
                Mod.customPacketHandlers[index] = Client_PowerEnabled_Handler;
                Mod.CustomPacketHandlerReceived -= Client_PowerEnabled_Received;
            }
        }
        
        #endregion
        
        #region Blockade Cleared

        // Host
        public void BlockadeCleared_Send(int blockadeId)
        {
            if (!Networking.ServerRunning() || Networking.IsClient())
                return;
            
            Packet packet = new Packet(blockadeCleared_ID);
            packet.Write(blockadeId);
            
            ServerSend.SendTCPDataToAll(packet, true);
        }
        
        void BlockadeCleared_Handler(int clientID, Packet packet)
        {
            int blockadeId = packet.ReadInt();
            GMgr.Instance.Blockades[blockadeId].Unlock();
        }

        void BlockadeCleared_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_BlockadeCleared")
            {
                blockadeCleared_ID = index;
                Mod.customPacketHandlers[index] = BlockadeCleared_Handler;
                Mod.CustomPacketHandlerReceived -= BlockadeCleared_Received;
            }
        }
        
        // Client
        public void Client_BlockadeCleared_Send(int blockadeId)
        {
            if (!Networking.ServerRunning() || Networking.IsHost())
                return;

            Packet packet = new Packet(blockadeCleared_Client_ID);
            packet.Write(blockadeId);
            
            ClientSend.SendTCPData(packet, true);
        }
        
        void Client_BlockadeCleared_Handler(int clientID, Packet packet)
        {
            int blockadeId = packet.ReadInt();
            GMgr.Instance.Blockades[blockadeId].Unlock();
            
            BlockadeCleared_Send(blockadeId);
        }

        void Client_BlockadeCleared_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_Client_BlockadeCleared")
            {
                blockadeCleared_Client_ID = index;
                Mod.customPacketHandlers[index] = Client_BlockadeCleared_Handler;
                Mod.CustomPacketHandlerReceived -= Client_BlockadeCleared_Received;
            }
        }
        
        #endregion
        
        
        #region Custom Data

        // Host
        public void CustomData_Send(int customDataID)
        {
            if (!Networking.ServerRunning() || Networking.IsClient())
                return;
            
            Packet packet = new Packet(customData_ID);
            packet.Write(customDataID);
            
            ServerSend.SendTCPDataToAll(packet, true);
        }
        
        void CustomData_Handler(int clientID, Packet packet)
        {
            int customDataId = packet.ReadInt();
            HandleCustomData(customDataId);
        }

        void CustomData_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_CustomData")
            {
                customData_ID = index;
                Mod.customPacketHandlers[index] = CustomData_Handler;
                Mod.CustomPacketHandlerReceived -= CustomData_Received;
            }
        }
        
        // Client
        public void Client_CustomData_Send(int customDataId)
        {
            if (!Networking.ServerRunning() || Networking.IsHost())
                return;

            Packet packet = new Packet(customData_Client_ID);
            packet.Write(customDataId);
            
            ClientSend.SendTCPData(packet, true);
        }
        
        void Client_CustomData_Handler(int clientID, Packet packet)
        {
            int customDataId = packet.ReadInt();
            HandleCustomData(customDataId);
            CustomData_Send(customDataId);
        }

        void Client_CustomData_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_Client_CustomData")
            {
                customData_Client_ID = index;
                Mod.customPacketHandlers[index] = Client_CustomData_Handler;
                Mod.CustomPacketHandlerReceived -= Client_CustomData_Received;
            }
        }

        private void HandleCustomData(int customDataId)
        {
            if (customDataId == (int)CustomDataType.MYSTERY_BOX_BOUGHT)
            {
                Refs.MysteryBox.SpawnWeapon();
            }
            else if (customDataId == (int)CustomDataType.MYSTERY_BOX_MOVED)
            {
                Refs.MysteryBox.AnimateBoxMove();
            }
        }
        
        #endregion
    }
    
    public enum CustomDataType
    {
        MYSTERY_BOX_BOUGHT = 0,
        MYSTERY_BOX_MOVED = 1,
    }
}

