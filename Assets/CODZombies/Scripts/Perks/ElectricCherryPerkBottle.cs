using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using CustomScripts.Player;
using CustomScripts.Powerups.Perks;
using FistVR;
using UnityEngine;

public class ElectricCherryPerkBottle : PerkBottle
{
	public override void ApplyModifier()
	{
		base.ApplyModifier();
		PlayerData.Instance.ElectricCherryPerkActivated = true;
		AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
		GetComponentInParent<FVRPhysicalObject>().ForceBreakInteraction();
		transform.parent.position = OriginPos;
	}
}
