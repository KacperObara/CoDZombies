using System;
using System.Linq;
using CustomScripts.Gamemode;
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
        private int sendIFF_ID = -1;
        private int mysteryBoxMoved_ID = -1;
        
        private int blockadeCleared_ID = -1;
        private int blockadeCleared_Client_ID = -1;
        
        // Send int value
        private int customData_ID = -1;
        private int customData_Client_ID = -1;
        
        // Send playerID + int value
        private int customData_playerID_ID = -1;
        private int customData_playerID_Client_ID = -1;

        private int powerUpSpawned_ID = -1;
        private int powerUpCollected_ID = -1;
        private int powerUpCollected_Client_ID = -1;
        
        void Start()
        {
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
            RegisterPacket("CodZ_GameStarted", StartGame_Handler, StartGame_Received, ref gameStarted_ID);
            RegisterPacket("CodZ_SendIFF", StartGame_Handler, SendIFF_Received, ref sendIFF_ID);
            RegisterPacket("CodZ_MysteryBoxMoved", MysteryBoxMoved_Handler, MysteryBoxMoved_Received, ref mysteryBoxMoved_ID);
            
            RegisterPacket("CodZ_BlockadeCleared", BlockadeCleared_Handler, BlockadeCleared_Received, ref blockadeCleared_ID);
            RegisterPacket("CodZ_Client_BlockadeCleared", Client_BlockadeCleared_Handler, Client_BlockadeCleared_Received, ref blockadeCleared_Client_ID);
            
            RegisterPacket("CodZ_CustomData", CustomData_Handler, CustomData_Received, ref customData_ID);
            RegisterPacket("CodZ_Client_CustomData", Client_CustomData_Handler, Client_CustomData_Received, ref customData_Client_ID);
            
            RegisterPacket("CodZ_CustomData_PlayerID", CustomData_PlayerID_Handler, CustomData_PlayerID_Received, ref customData_playerID_ID);
            RegisterPacket("CodZ_Client_CustomData_PlayerID", Client_CustomData_PlayerID_Handler, Client_CustomData_PlayerID_Received, ref customData_playerID_Client_ID);

            RegisterPacket("CodZ_PowerUpSpawned", PowerUpSpawned_Handler, PowerUpSpawned_Received, ref powerUpSpawned_ID);
            
            RegisterPacket("CodZ_PowerUpCollected", PowerUpCollected_Handler, PowerUpCollected_Received, ref powerUpCollected_ID);
            RegisterPacket("CodZ_Client_PowerUpCollected", Client_PowerUpCollected_Handler, Client_PowerUpCollected_Received, ref powerUpCollected_Client_ID);
        }
        
        private void RegisterPacket(string packetName, Mod.CustomPacketHandler hostHandler, Mod.CustomPacketHandlerReceivedDelegate clientHandler, ref int packetID)
        {
            if (Networking.IsHost())
            {
                if (Mod.registeredCustomPacketIDs.ContainsKey(packetName))
                {
                    packetID = Mod.registeredCustomPacketIDs[packetName];
                }
                else
                {
                    packetID = Server.RegisterCustomPacketType(packetName);
                }
                Mod.customPacketHandlers[packetID] = hostHandler;
            }
            else
            {
                if (Mod.registeredCustomPacketIDs.ContainsKey(packetName))
                {
                    packetID = Mod.registeredCustomPacketIDs[packetName];
                    Mod.customPacketHandlers[packetID] = hostHandler;
                }
                else
                {
                    ClientSend.RegisterCustomPacketType(packetName);
                    Mod.CustomPacketHandlerReceived += clientHandler;
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
            
            // Give all players a unique IFF so we know who award points to for sosig kills
            GM.CurrentPlayerBody.SetPlayerIFF(5);
            int iff = 6;
            foreach (var player in GameManager.players)
            {
                SendIFF_Send(player.Key, iff);
                iff++;
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
        
        #region Send IFF

        public void SendIFF_Send(int playerID, int iff)
        {
            if (!Networking.ServerRunning() || Networking.IsClient())
                return;
            
            Packet packet = new Packet(sendIFF_ID);
            packet.Write(playerID);
            packet.Write(iff);
            
            ServerSend.SendTCPDataToAll(packet, true);
        }
        
        private void SendIFF_Handler(int clientID, Packet packet)
        {
            int playerID = packet.ReadInt();
            int iff = packet.ReadInt();
            
            if (playerID == GameManager.ID)
                GM.CurrentPlayerBody.SetPlayerIFF(iff);
        }
        
        private void SendIFF_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_SendIFF")
            {
                sendIFF_ID = index;
                Mod.customPacketHandlers[index] = SendIFF_Handler;
                Mod.CustomPacketHandlerReceived -= SendIFF_Received;
            }
        }

        #endregion
        
        #region Mystery Box Moved

        public void MysteryBoxMoved_Send(int newWayPointID, bool immediate)
        {
            if (!Networking.ServerRunning() || Networking.IsClient())
                return;
            
            Packet packet = new Packet(mysteryBoxMoved_ID);
            packet.Write(newWayPointID);
            packet.Write(immediate);
            
            ServerSend.SendTCPDataToAll(packet, true);
        }
        
        private void MysteryBoxMoved_Handler(int clientID, Packet packet)
        {
            int newWaypointID = packet.ReadInt();
            bool immediate = packet.ReadBool();
            
            Refs.MysteryBoxMover.SetNextWaypoint(newWaypointID);

            if (immediate)
                Refs.MysteryBoxMover.StartTeleportAnim();
            else
                Refs.MysteryBoxMover.Teleport();
        }
        
        private void MysteryBoxMoved_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_MysteryBoxMoved")
            {
                mysteryBoxMoved_ID = index;
                Mod.customPacketHandlers[index] = MysteryBoxMoved_Handler;
                Mod.CustomPacketHandlerReceived -= MysteryBoxMoved_Received;
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
        
        #region PowerUpSpawned

        // Host
        public void PowerUpSpawned_Send(int powerUpId, Vector3 pos)
        {
            if (!Networking.ServerRunning() || Networking.IsClient())
                return;
    
            Packet packet = new Packet(powerUpSpawned_ID);
            packet.Write(powerUpId);
            packet.Write(pos);
    
            ServerSend.SendTCPDataToAll(packet, true);
        }

        void PowerUpSpawned_Handler(int clientID, Packet packet)
        {
            int powerUpId = packet.ReadInt();
            Vector3 pos = packet.ReadVector3();
            PowerUpManager.Instance.SpawnPowerUp(PowerUpManager.Instance.PowerUps[powerUpId], pos);
        }

        void PowerUpSpawned_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_PowerUpSpawned")
            {
                powerUpSpawned_ID = index;
                Mod.customPacketHandlers[index] = PowerUpSpawned_Handler;
                Mod.CustomPacketHandlerReceived -= PowerUpSpawned_Received;
            }
        }
        
        #endregion

        
        #region PowerUpCollected

        // Host
        public void PowerUpCollected_Send(int powerUpId)
        {
            if (!Networking.ServerRunning() || Networking.IsClient())
                return;
    
            Packet packet = new Packet(powerUpCollected_ID);
            packet.Write(powerUpId);
    
            ServerSend.SendTCPDataToAll(packet, true);
        }

        void PowerUpCollected_Handler(int clientID, Packet packet)
        {
            int powerUpId = packet.ReadInt();
            PowerUp powerUp = PowerUpManager.Instance.PowerUps[powerUpId];
            powerUp.ApplyModifier();
        }

        void PowerUpCollected_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_PowerUpCollected")
            {
                powerUpCollected_ID = index;
                Mod.customPacketHandlers[index] = PowerUpCollected_Handler;
                Mod.CustomPacketHandlerReceived -= PowerUpCollected_Received;
            }
        }

        // Client
        public void Client_PowerUpCollected_Send(int powerUpId)
        {
            if (!Networking.ServerRunning() || Networking.IsHost())
                return;

            Packet packet = new Packet(powerUpCollected_Client_ID);
            packet.Write(powerUpId);
    
            ClientSend.SendTCPData(packet, true);
        }

        void Client_PowerUpCollected_Handler(int clientID, Packet packet)
        {
            int powerUpId = packet.ReadInt();
            PowerUp powerUp = PowerUpManager.Instance.PowerUps[powerUpId];
            powerUp.ApplyModifier();
    
            PowerUpCollected_Send(powerUpId);
        }

        void Client_PowerUpCollected_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_Client_PowerUpCollected")
            {
                powerUpCollected_Client_ID = index;
                Mod.customPacketHandlers[index] = Client_PowerUpCollected_Handler;
                Mod.CustomPacketHandlerReceived -= Client_PowerUpCollected_Received;
            }
        }

        #endregion
        
        #region Custom Data (int value)

        // Host
        public void CustomData_Send(int customDataID)
        {
            if (!Networking.ServerRunning() || Networking.IsClient())
                return;
            
            HandleCustomData(customDataID);
            
            Packet packet = new Packet(customData_ID);
            packet.Write(customDataID);
            
            Debug.Log("Host sending custom data: " + (CustomDataType)customDataID);
            
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

            Debug.Log("Client sending custom data: " + (CustomDataType)customDataId);
            
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
                if (Networking.IsHostOrSolo())
                    Refs.MysteryBox.SpawnWeapon();
            }
            else if (customDataId == (int)CustomDataType.POWER_ENABLED)
            {
                GMgr.Instance.TurnOnPower();
            }
            else if (customDataId == (int)CustomDataType.EVERY_PLAYER_DEAD)
            {
                PlayerSpawner.Instance.MoveToEndGameArea();
            }
        }
        
        private void HandlePlayerCustomData(int playerID, int customDataId)
        {
            if (customDataId == (int)CustomPlayerDataType.PLAYER_DOWNED)
            {
                if (playerID == GameManager.ID)
                    return;
                
                Vector3 deathPos = GameManager.players[playerID].transform.position;
                ReviveButton.Instance.Spawn(playerID, deathPos);
            }
            else if (customDataId == (int)CustomPlayerDataType.PLAYER_REVIVED)
            {
                if (playerID == GameManager.ID)
                { 
                    PlayerSpawner.Instance.Revive();
                }
                else
                {
                    ReviveButton.Instance.Despawn();
                }
            }
        }
        #endregion
        
        #region Custom Data (PlayerID + int value)

        // Host
        public void CustomData_PlayerID_Send(int playerID, int customDataID)
        {
            if (!Networking.ServerRunning() || Networking.IsClient())
                return;
            
            HandlePlayerCustomData(playerID, customDataID);
            
            Packet packet = new Packet(customData_playerID_ID);
            packet.Write(playerID);
            packet.Write(customDataID);
            
            Debug.Log("Host sending custom data with player ID: " + playerID + " " + (CustomPlayerDataType)customData_ID);
            
            ServerSend.SendTCPDataToAll(packet, true);
        }
        
        void CustomData_PlayerID_Handler(int clientID, Packet packet)
        {
            int playerID = packet.ReadInt();
            int customDataId = packet.ReadInt();
            HandlePlayerCustomData(playerID, customDataId);
        }

        void CustomData_PlayerID_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_CustomData_PlayerID")
            {
                customData_playerID_ID = index;
                Mod.customPacketHandlers[index] = CustomData_PlayerID_Handler;
                Mod.CustomPacketHandlerReceived -= CustomData_PlayerID_Received;
            }
        }
        
        // Client
        public void Client_CustomData_PlayerID_Send(int playerID, int customDataID)
        {
            if (!Networking.ServerRunning() || Networking.IsHost())
                return;

            Packet packet = new Packet(customData_playerID_Client_ID);
            packet.Write(playerID);
            packet.Write(customDataID);
            
            Debug.Log("Client sending custom data with player ID: " + playerID + " " + (CustomPlayerDataType)customData_ID);
            ClientSend.SendTCPData(packet, true);
        }
        
        void Client_CustomData_PlayerID_Handler(int clientID, Packet packet)
        {
            int playerID = packet.ReadInt();
            int customDataId = packet.ReadInt();
            HandlePlayerCustomData(playerID, customDataId);
            CustomData_PlayerID_Send(playerID, customDataId);
        }

        void Client_CustomData_PlayerID_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_Client_CustomData_PlayerID")
            {
                customData_playerID_Client_ID = index;
                Mod.customPacketHandlers[index] = Client_CustomData_PlayerID_Handler;
                Mod.CustomPacketHandlerReceived -= Client_CustomData_PlayerID_Received;
            }
        }
        
        #endregion
    }
    
    public enum CustomDataType
    {
        MYSTERY_BOX_BOUGHT = 0,
        PACK_A_PUNCH_BUSY = 1,
        POWER_ENABLED = 2,
        EVERY_PLAYER_DEAD = 3,
    }
    
    public enum CustomPlayerDataType
    {
        PLAYER_DOWNED = 0,
        PLAYER_REVIVED = 1,
    }
}

