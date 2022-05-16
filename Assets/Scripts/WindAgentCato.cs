using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class WindAgentCato : MonoBehaviour {
    public WindBlock currentBlock;
    public float windAmount = 5f;

    private WindFieldModule m_WindModule;

    private void Start() {
        m_WindModule = GameManager.GetModule<WindFieldModule>();
    }

    private void Update() {
        AddWindByBlockChange();
    }

    void AddWindByBlockChange() {
        var block = m_WindModule.GetBlockByPosition(transform.position);
        if (currentBlock != block) {
            if (currentBlock == null) {
                //OnBlockChanged(0, 0, block.x, block.y);
            }
            else {
                OnBlockChanged(currentBlock.x, currentBlock.y, block.x, block.y);
            }

            currentBlock = block;
        }
    }

    private void OnBlockChanged(int xi, int yi, int xf, int yf) {
        //i:initial f:final
        var delta = new Vector3(xf - xi, 0f, yf - yi).normalized * windAmount;
        m_WindModule.AddWind(xi, yi, delta, false);
        // Debug.Log("Add wind at " + xi + " " + yi + " " + delta.ToString());
    }
}