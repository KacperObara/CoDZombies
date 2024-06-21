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

        private AudioSource _audioSource;
        
        public List<AudioClip> TearingPlankSounds;
        public List<AudioClip> RepairingPlankSounds;
        public AudioClip PlankFloatSound;
        public AudioClip WindowRepairSound;
        
        private void Start()
        {
            _planks = GetComponentsInChildren<Plank>().ToList();
            _audioSource = GetComponent<AudioSource>();

            foreach (var plank in _planks)
            {
                plank.Initialize(this);
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
            Debug.Log("Plank Tearing: " + plankId + " " + _planks.Count + " " + _audioSource);
            _planks[plankId].Tear();
            _audioSource.PlayOneShot(TearingPlankSounds[UnityEngine.Random.Range(0, TearingPlankSounds.Count)]);
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
            _planks[plankId].Repair(false);
            
            _audioSource.PlayOneShot(PlankFloatSound);
            _audioSource.PlayOneShot(WindowRepairSound);
            
            if (BarricadedEvent != null)
                BarricadedEvent.Invoke();
        }

        public void OnPlankInPlace()
        {
            _audioSource.PlayOneShot(RepairingPlankSounds[UnityEngine.Random.Range(0, RepairingPlankSounds.Count)]);
        }

        public void RepairAll()
        {
            foreach (var plank in _planks)
            {
                plank.Repair(true);
            }
            _audioSource.PlayOneShot(WindowRepairSound);
        }
    }
    
    public enum WindowAction
    {
        Tear,
        Repair
    }
}
#endif