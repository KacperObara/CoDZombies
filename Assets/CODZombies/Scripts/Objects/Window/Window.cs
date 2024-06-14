#if H3VR_IMPORTED
using System;
using System.Collections.Generic;
using System.Linq;
using CustomScripts.Multiplayer;
using UnityEngine;

namespace CustomScripts
{
    public class Window : MonoBehaviour
    {
        public static Action BarricadedEvent;
        public int ID { get; set; }
        
        public Transform ZombieWaypoint;
        
        private List<Plank> _planks;

        private AudioSource _tearPlankAudio;
        
        private void Start()
        {
            _planks = GetComponentsInChildren<Plank>().ToList();
            _tearPlankAudio = GetComponent<AudioSource>();

            foreach (var plank in _planks)
            {
                plank.Window = this;
            }

            ID = GameRefs.Windows.FindIndex(window => window == this);
        }
        
        public bool IsBroken()
        {
            return _planks.All(plank => plank.IsBroken);
        }
        
        public bool IsFullyRepaired()
        {
            return _planks.All(plank => !plank.IsBroken);
        }
        
        //Data sender
        public void TearPlank()
        {
            if (Networking.IsHostOrSolo())
            {
                int plankId = _planks.FindIndex(plank => !plank.IsBroken);
                OnPlankTeared(plankId);
                CodZNetworking.Instance.WindowStateChanged_Send(ID, plankId, WindowAction.Tear);
            }
        }

        // Data receiver
        public void OnPlankTeared(int plankId)
        {
            Debug.Log("Plank Tearing: " + plankId + " " + _planks.Count + " " + _tearPlankAudio);
            _planks[plankId].Tear();
            _tearPlankAudio.Play();
        }

        public void RepairWindow()
        {
            int plankId = _planks.FindIndex(plank => plank.IsBroken);
            if (Networking.IsHostOrSolo())
            {
                OnWindowRepaired(plankId);
                CodZNetworking.Instance.WindowStateChanged_Send(ID, plankId, WindowAction.Repair);
            }
            else
            {
                CodZNetworking.Instance.Client_WindowStateChanged_Send(ID, plankId);
            }
        }

        public void OnWindowRepaired(int plankId)
        {
            _planks[plankId].Repair();
            AudioManager.Instance.Play(AudioManager.Instance.BarricadeRepairSound, .5f);
            
            if (BarricadedEvent != null)
                BarricadedEvent.Invoke();
        }

        public void RepairAll()
        {
            foreach (var plank in _planks)
            {
                plank.Repair();
            }
        }
    }
    
    public enum WindowAction
    {
        Tear,
        Repair
    }
}
#endif