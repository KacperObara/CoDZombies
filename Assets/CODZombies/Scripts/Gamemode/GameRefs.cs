#if H3VR_IMPORTED
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Atlas;
using Atlas.MappingComponents.Sandbox;
using CustomScripts.Objects;
using CustomScripts.Powerups;
using UnityEngine;

namespace CustomScripts
{
    public class GameRefs : MonoBehaviourSingleton<GameRefs>
    {
        public Color CanBuyColor;
        public Color CannotBuyColor;
        
        [HideInInspector] public Transform Player;
        [SerializeField] private Transform DebugPlayer;

        [HideInInspector] public Transform PlayerHead;
        [SerializeField] private Transform DebugPlayerHead;
        
        private List<Window> _windows;
        public static List<Window> Windows { get { return Instance._windows; } }
        
        private MysteryBox _mysteryBox;
        public static MysteryBox MysteryBox { get { return Instance._mysteryBox; } }
	
        private MysteryBoxMover _mysteryBoxMover;
        public static MysteryBoxMover MysteryBoxMover { get { return Instance._mysteryBoxMover; } }

        private Radio _radio;
        public static Radio Radio { get { return Instance._radio; } }
	
        private PackAPunch _packAPunch;
        public static PackAPunch PackAPunch { get { return Instance._packAPunch; } }

        public override void Awake()
        {
            base.Awake();

            Player = DebugPlayer;
            PlayerHead = DebugPlayerHead;

            _mysteryBox = FindObjectOfType<MysteryBox>();
            _mysteryBoxMover = FindObjectOfType<MysteryBoxMover>();
            _radio = FindObjectOfType<Radio>();
            _packAPunch = FindObjectOfType<PackAPunch>();
            _windows = FindObjectsOfType<Window>().ToList();
        }

        private IEnumerator Start()
        {
            while (FistVR.GM.CurrentPlayerBody == null)
                yield return null;

            Player = FistVR.GM.CurrentPlayerBody.transform;
            PlayerHead = FistVR.GM.CurrentPlayerBody.Head.transform;
        }

        public bool IsPlayerClose(Transform pos, float dist)
        {
            return Vector3.Distance(pos.position, Player.position) <= dist;
        }
    }
}

#endif