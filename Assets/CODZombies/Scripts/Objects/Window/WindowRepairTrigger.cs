﻿using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using UnityEngine;

public class WindowRepairTrigger : MonoBehaviour 
{
    public Window Window;

    private float _timer;
    private float _timeToTrigger = 1.5f;
    private void OnTriggerStay(Collider other)
    {
        if (Window.IsFullyRepaired())
            return;
            
        if (other.GetComponent<PlayerCollider>())
        {
            _timer += Time.deltaTime;
            if (_timer >= _timeToTrigger)
            {
                Window.RepairWindow();
                _timer = 0;
            }
        }
    }
}
