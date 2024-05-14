﻿using System;
using System.Collections;
using System.Collections.Generic;
using FistVR;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class MagazineBox : FVRPhysicalObject
{
	public Sprite EmptySprite;

	[SerializeField] private Text _magazineNameText;
	[SerializeField] private Text _magazineCountText;
	[SerializeField] private Image _magazineImage;

	
	private Stack<FVRFireArmMagazine> _magazines = new Stack<FVRFireArmMagazine>();
	private bool IsEmpty { get { return _magazines.Count == 0; } }
	private FVRFireArmMagazine CurrentMagazine { get { return _magazines.Peek(); } }


	public override void BeginInteraction(FVRViveHand hand)
	{
		if (hand.Input.TriggerFloat > 0.5)
		{
			RetrieveMagazine(hand);
		}
		else
		{
			base.BeginInteraction(hand);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<FVRViveHand>())
		{
			FVRViveHand hand = other.GetComponent<FVRViveHand>();
			if (hand.IsThisTheRightHand)
			{
				hand.Trigger_Button.RemoveOnStateUpListener(TryPlacing, SteamVR_Input_Sources.RightHand);
				hand.Trigger_Button.AddOnStateUpListener(TryPlacing, SteamVR_Input_Sources.RightHand);
			}
			else
			{
				hand.Trigger_Button.RemoveOnStateUpListener(TryPlacing, SteamVR_Input_Sources.LeftHand);
				hand.Trigger_Button.AddOnStateUpListener(TryPlacing, SteamVR_Input_Sources.LeftHand);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<FVRViveHand>())
		{
			FVRViveHand hand = other.GetComponent<FVRViveHand>();
			if (hand.IsThisTheRightHand)
			{
				hand.Trigger_Button.RemoveOnStateUpListener(TryPlacing, SteamVR_Input_Sources.RightHand);
			}
			else
			{
				hand.Trigger_Button.RemoveOnStateUpListener(TryPlacing, SteamVR_Input_Sources.LeftHand);
			}
		}
	}

	private void TryPlacing(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource)
	{
		FVRViveHand hand;
		if (fromsource != SteamVR_Input_Sources.RightHand)
		{
			hand = GM.CurrentMovementManager.Hands[0];
		}
		else
		{
			hand = GM.CurrentMovementManager.Hands[1];
		}

		if (hand.CurrentInteractable == null)
			return;

		FVRFireArmMagazine magazine = hand.CurrentInteractable.GetComponent<FVRFireArmMagazine>();

		if (magazine == null)
			return;
		
		if (IsEmpty || !IsEmpty && IsMagazineCompatible(magazine))
		{
			AddMagazine(magazine);
		}
	}

	private void RetrieveMagazine(FVRViveHand hand)
	{
		if (IsEmpty)
			return;

		FVRFireArmMagazine magazine = _magazines.Pop();
		magazine.gameObject.SetActive(true);

		hand.RetrieveObject(magazine);

		UpdateUI();
	}

	public void AddMagazine(FVRFireArmMagazine magazine)
	{
		_magazines.Push(magazine);
		magazine.gameObject.SetActive(false);
		UpdateUI();
	}

	private bool IsMagazineCompatible(FVRFireArmMagazine magazine)
	{
		return CurrentMagazine.ObjectWrapper.SpawnedFromId == magazine.ObjectWrapper.SpawnedFromId;
	}

	private void UpdateUI()
	{
		if (IsEmpty)
		{
			_magazineNameText.text = "None";
			_magazineCountText.text = "0";
			_magazineImage.sprite = EmptySprite;
		}
		else
		{
			_magazineNameText.text = IM.GetSpawnerID(CurrentMagazine.ObjectWrapper.SpawnedFromId).DisplayName;
			_magazineCountText.text = _magazines.Count.ToString();
			_magazineImage.sprite = IM.GetSpawnerID(CurrentMagazine.ObjectWrapper.SpawnedFromId).Sprite;
		}
	}
}
