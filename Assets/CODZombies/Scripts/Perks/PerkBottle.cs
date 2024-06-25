using System;
using CustomScripts;
using UnityEngine;

namespace CustomScripts.Powerups.Perks
{
    public class PerkBottle : MonoBehaviour, IModifier
    {
        [HideInInspector] public Vector3 OriginPos;
        public GameObject PlayerPerkIcon;

        private void Start()
        {
            OriginPos = transform.parent.position;
        }

        public virtual void ApplyModifier()
        {
            PlayerPerkIcon.SetActive(true);
        }
    }
}