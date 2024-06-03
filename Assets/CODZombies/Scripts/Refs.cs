using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using UnityEngine;

public class Refs : MonoBehaviourSingleton<Refs> 
{
	private MysteryBox _mysteryBox;
	public static MysteryBox MysteryBox { get { return Instance._mysteryBox; } }

	private void Start()
	{
		_mysteryBox = FindObjectOfType<MysteryBox>();
	}
}
