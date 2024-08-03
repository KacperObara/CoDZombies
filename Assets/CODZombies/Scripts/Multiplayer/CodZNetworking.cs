using System;
using System.Collections;
using System.Linq;
using CustomScripts.Gamemode;
using CustomScripts.Managers;
using CustomScripts.Powerups;
using FistVR;
using H3MP;
using H3MP.Networking;
using UnityEngine;

namespace CustomScripts.Multiplayer
{
    // Welcome traveller, You are here to learn about the networking system in CODZombies.
    // I'm sorry but I have no idea what I'm doing, and why this works.
    public class CodZNetworking : MonoBehaviourSingleton<CodZNetworking> 
    {
        //Packet IDs
        private int gameStarted_ID = -1;
        private int mysteryBoxMoved_ID = -1;
        
        private int blockadeCleared_ID = -1;
        private int blockadeCleared_Client_ID = -1;

        private int powerUpSpawned_ID = -1;
        private int powerUpCollected_ID = -1;
        private int powerUpCollected_Client_ID = -1;
        
        private int papPurchased_ID = -1;
        private int papPurchased_Client_ID = -1;       
        
        private int windowStateChanged_ID = -1;
        private int windowStateChanged_Client_ID = -1;
        
        // Send int value
        private int customData_ID = -1;
        private int customData_Client_ID = -1;
        
        // Send playerID + int value
        private int customData_playerID_ID = -1;
        private int customData_playerID_Client_ID = -1;
        
        private IEnumerator Start()
        {
            // Wait one frame so that everything is all setup
            yield return null;
            StartNetworking();
        }
        
        private void StartNetworking()
        {
            Mod.customPacketHandlers = new Mod.CustomPacketHandler[20];
            
            if (Networking.ServerRunning())
            {
                SetupPacketTypes();
                
                // Different IFF for every player to track kills and award points
                GM.CurrentPlayerBody.SetPlayerIFF(GameManager.ID + 5);
            }
        }

        private void SetupPacketTypes()
        {
            RegisterPacket("CodZ_GameStarted", StartGame_Handler, StartGame_Received, ref gameStarted_ID);
            RegisterPacket("CodZ_MysteryBoxMoved", MysteryBoxMoved_Handler, MysteryBoxMoved_Received, ref mysteryBoxMoved_ID);
            
            RegisterPacket("CodZ_BlockadeCleared", BlockadeCleared_Handler, BlockadeCleared_Received, ref blockadeCleared_ID);
            RegisterPacket("CodZ_Client_BlockadeCleared", Client_BlockadeCleared_Handler, Client_BlockadeCleared_Received, ref blockadeCleared_Client_ID);

            RegisterPacket("CodZ_PowerUpSpawned", PowerUpSpawned_Handler, PowerUpSpawned_Received, ref powerUpSpawned_ID);
            
            RegisterPacket("CodZ_PowerUpCollected", PowerUpCollected_Handler, PowerUpCollected_Received, ref powerUpCollected_ID);
            RegisterPacket("CodZ_Client_PowerUpCollected", Client_PowerUpCollected_Handler, Client_PowerUpCollected_Received, ref powerUpCollected_Client_ID);
            
            RegisterPacket("CodZ_PaPPurchased", PaPPurchased_Handler, PaPPurchased_Received, ref papPurchased_ID);
            RegisterPacket("CodZ_Client_PaPPurchased", Client_PaPPurchased_Handler, Client_PaPPurchased_Received, ref papPurchased_Client_ID);
            
            RegisterPacket("CodZ_WindowStateChanged", WindowStateChanged_Handler, WindowStateChanged_Received, ref windowStateChanged_ID);
            RegisterPacket("CodZ_Client_WindowStateChanged", Client_WindowStateChanged_Handler, Client_WindowStateChanged_Received, ref windowStateChanged_Client_ID);
            
            RegisterPacket("CodZ_CustomData", CustomData_Handler, CustomData_Received, ref customData_ID);
            RegisterPacket("CodZ_Client_CustomData", Client_CustomData_Handler, Client_CustomData_Received, ref customData_Client_ID);
            
            RegisterPacket("CodZ_CustomData_PlayerID", CustomData_PlayerID_Handler, CustomData_PlayerID_Received, ref customData_playerID_ID);
            RegisterPacket("CodZ_Client_CustomData_PlayerID", Client_CustomData_PlayerID_Handler, Client_CustomData_PlayerID_Received, ref customData_playerID_Client_ID);
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
                    packetID = Networking.RegisterCustomPacketType(packetName);
                    //packetID = Server.RegisterCustomPacketType(packetName);
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
            packet.Write(GameSettings.WonderWeaponEnabled);

            ServerSend.SendTCPDataToAll(packet, true);
        }
        
        private void StartGame_Handler(int clientID, Packet packet)
        {
            GameSettings.HardMode = packet.ReadBool();
            GameSettings.WeakerEnemiesEnabled = packet.ReadBool();
            GameSettings.SpecialRoundDisabled = packet.ReadBool();
            GameSettings.ItemSpawnerEnabled = packet.ReadBool();
            GameSettings.WonderWeaponEnabled = packet.ReadBool();
            
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
            
            GameRefs.MysteryBoxMover.SetNextWaypoint(newWaypointID);

            if (immediate)
                GameRefs.MysteryBoxMover.Teleport();
            else
                GameRefs.MysteryBoxMover.StartTeleportAnim();
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
            powerUp.OnCollect();
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
            powerUp.OnCollect();
    
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
        
        #region PaPPurchased

        // Host
        public void PaPPurchased_Send()
        {
            if (!Networking.ServerRunning() || Networking.IsClient())
                return;

            Packet packet = new Packet(papPurchased_ID);
            ServerSend.SendTCPDataToAll(packet, true);
        }

        void PaPPurchased_Handler(int clientID, Packet packet)
        {
            GameRefs.PackAPunch.OnBuying();
        }

        void PaPPurchased_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_PaPPurchased")
            {
                papPurchased_ID = index;
                Mod.customPacketHandlers[index] = PaPPurchased_Handler;
                Mod.CustomPacketHandlerReceived -= PaPPurchased_Received;
            }
        }

        // Client
        public void Client_PaPPurchased_Send()
        {
            if (!Networking.ServerRunning() || Networking.IsHost())
                return;

            Packet packet = new Packet(papPurchased_Client_ID);
            ClientSend.SendTCPData(packet, true);
        }

        void Client_PaPPurchased_Handler(int clientID, Packet packet)
        {
            GameRefs.PackAPunch.OnBuying();
            PaPPurchased_Send();
        }

        void Client_PaPPurchased_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_Client_PaPPurchased")
            {
                papPurchased_Client_ID = index;
                Mod.customPacketHandlers[index] = Client_PaPPurchased_Handler;
                Mod.CustomPacketHandlerReceived -= Client_PaPPurchased_Received;
            }
        }

        #endregion
        
        #region WindowStateChanged

        // Host
        public void WindowStateChanged_Send(int windowId, int plankId, WindowAction windowAction) // true = repairing, false = tearing
        {
            if (!Networking.ServerRunning() || Networking.IsClient())
                return;
            
            Packet packet = new Packet(windowStateChanged_ID);
            packet.Write(windowId);
            packet.Write(plankId);
            packet.Write((int)windowAction);

            ServerSend.SendTCPDataToAll(packet, true);
        }

        void WindowStateChanged_Handler(int clientID, Packet packet)
        {
            int windowId = packet.ReadInt();
            int plankId = packet.ReadInt();
            WindowAction windowAction = (WindowAction)packet.ReadInt();
            
            if (windowAction == WindowAction.Repair)
            {
                GameRefs.Windows[windowId].OnWindowRepaired(plankId);
            }
            else if (windowAction == WindowAction.Tear)
            {
                GameRefs.Windows[windowId].OnPlankTeared(plankId);
            }
        }

        void WindowStateChanged_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_WindowStateChanged")
            {
                windowStateChanged_ID = index;
                Mod.customPacketHandlers[index] = WindowStateChanged_Handler;
                Mod.CustomPacketHandlerReceived -= WindowStateChanged_Received;
            }
        }

        // Client
        public void Client_WindowStateChanged_Send(int windowId, int plankId)
        {
            if (!Networking.ServerRunning() || Networking.IsHost())
                return;

            Packet packet = new Packet(windowStateChanged_Client_ID);
            packet.Write(windowId);
            packet.Write(plankId);
            
            ClientSend.SendTCPData(packet, true);
        }

        void Client_WindowStateChanged_Handler(int clientID, Packet packet)
        {
            int windowId = packet.ReadInt();
            int plankId = packet.ReadInt();

            GameRefs.Windows[windowId].OnWindowRepaired(plankId);
            WindowStateChanged_Send(windowId, plankId, WindowAction.Repair); // client doesn't send plank tearing data
        }

        void Client_WindowStateChanged_Received(string handlerID, int index)
        {
            if (handlerID == "CodZ_Client_WindowStateChanged")
            {
                windowStateChanged_Client_ID = index;
                Mod.customPacketHandlers[index] = Client_WindowStateChanged_Handler;
                Mod.CustomPacketHandlerReceived -= Client_WindowStateChanged_Received;
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
            
            //HandleCustomData(customDataId);
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
            
            ClientSend.SendTCPData(packet, true);
        }
        
        void Client_CustomData_PlayerID_Handler(int clientID, Packet packet)
        {
            int playerID = packet.ReadInt();
            int customDataId = packet.ReadInt();
            //HandlePlayerCustomData(playerID, customDataId);
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
        
        
        private void HandleCustomData(int customDataId)
        {
            if (customDataId == (int)CustomDataType.MYSTERY_BOX_ROLLED)
            {
                GameRefs.MysteryBox.OnBuying();
            }
            else if (customDataId == (int)CustomDataType.MYSTERY_BOX_TELEPORT)
            {
                GameRefs.MysteryBox.WillTeleport = true;
            }
            else if (customDataId == (int)CustomDataType.POWER_ENABLED)
            {
                GMgr.Instance.TurnOnPower();
            }
            else if (customDataId == (int)CustomDataType.EVERY_PLAYER_DEAD)
            {
                PlayerSpawner.Instance.MoveToEndGameArea();
            }
            else if (customDataId == (int)CustomDataType.RADIO_TOGGLE)
            {
                GameRefs.Radio.ToggleMusic();
            }
        }
        
        private void HandlePlayerCustomData(int playerID, int customDataId)
        {
            if (customDataId == (int)CustomPlayerDataType.PLAYER_DOWNED)
            {
                if (Networking.IsHost() && PlayersMgr.Instance.AllPlayersDowned())
                {
                    CustomData_Send((int)CustomDataType.EVERY_PLAYER_DEAD);
                }
                
                if (playerID == GameManager.ID)
                    return;
                
                Vector3 deathPos = GameManager.players[playerID].transform.position;

                PlayersMgr.SpawnReviveButton(playerID, deathPos);
                //ReviveButton.Instance.Spawn(playerID, deathPos);
                PlayersMgr.GetPlayerExcludingMe(playerID).IsDowned = true;
            }
            else if (customDataId == (int)CustomPlayerDataType.PLAYER_DEAD)
            {
                if (playerID == GameManager.ID)
                    return;
                
                if (Networking.IsHost() && PlayersMgr.Instance.AllPlayersDowned())
                {
                    CustomData_Send((int)CustomDataType.EVERY_PLAYER_DEAD);
                }
                
                PlayersMgr.GetPlayerExcludingMe(playerID).IsDowned = false;
                PlayersMgr.GetPlayerExcludingMe(playerID).IsDead = true;

                PlayersMgr.DespawnReviveButton(playerID);
            }
            else if (customDataId == (int)CustomPlayerDataType.PLAYER_REVIVED)
            {
                if (playerID == GameManager.ID)
                { 
                    PlayerSpawner.Instance.Revive();
                }
                else
                {
                    PlayersMgr.GetPlayerExcludingMe(playerID).IsDowned = false;
                    PlayersMgr.DespawnReviveButton(playerID);
                }
            }
            else if (customDataId == (int)CustomPlayerDataType.ZOMBIE_HIT)
            {
                // Hits and kills could be tracked locally, but somehow it stopped working suddenly.
                // For client, sosig died IFF is set to host or -1 FOR SOME GODDAMN REASON
                if (playerID == GM.CurrentPlayerBody.GetPlayerIFF())
                { 
                    GMgr.Instance.AddPoints(ZombieManager.Instance.PointsOnHit);
                }
            }
            else if (customDataId == (int)CustomPlayerDataType.ZOMBIE_KILLED)
            {
                if (playerID == GM.CurrentPlayerBody.GetPlayerIFF())
                {
                    GMgr.Instance.AddPoints(ZombieManager.Instance.PointsOnKill);
                    GMgr.Instance.Kills++;
                }
            }
            else if (customDataId == (int)CustomPlayerDataType.PLAYER_SPAWNED)
            {
                if (playerID == GameManager.ID)
                    return;
                
                PlayersMgr.GetPlayerExcludingMe(playerID).IsDowned = false;
                PlayersMgr.GetPlayerExcludingMe(playerID).IsDead = false;
            }
        }
    }
    
    public enum CustomDataType
    {
        MYSTERY_BOX_ROLLED = 0,
        MYSTERY_BOX_TELEPORT = 1,
        //MYSTERY_BOX_GUN_SPAWNED = 2,
        POWER_ENABLED = 3,
        EVERY_PLAYER_DEAD = 4,
        RADIO_TOGGLE = 5,
    }
    
    public enum CustomPlayerDataType
    {
        PLAYER_DOWNED = 0,
        PLAYER_DEAD = 1,
        PLAYER_REVIVED = 2,
        ZOMBIE_HIT = 3,
        ZOMBIE_KILLED = 4,
        PLAYER_SPAWNED = 5,
    }
}
