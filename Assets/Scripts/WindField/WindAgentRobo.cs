using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class WindAgentRobo : MonoBehaviour {
    public WindBlock currentBlock;
    public Animator animator;
    public float windAmount = 5f;

    private WindFieldModule m_WindModule;
    
    private void Start() {
        m_WindModule = GameManager.GetModule<WindFieldModule>();
    }

    private void Update() {
        AddWindByMovement();

        // if (Game.Input.GetKey("Attack")) {
        //     AddWindByAttack();
        // }
    }

    private void AddWindByAttack() {
        if (Game.Blackboard.GetData<bool>("IsReceivingAnyInput")) {
            var direction = Game.Blackboard.GetData<LookDirection>("Robo.LookDirection");
            switch (direction) {
                case LookDirection.Forward:
                    m_WindModule.AddWind(currentBlock.x, currentBlock.y - 1, Vector3.forward * windAmount);
                    m_WindModule.AddWind(currentBlock.x, currentBlock.y - 2, Vector3.forward * windAmount);
                    m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y - 2, (Vector3.forward + Vector3.left).normalized * windAmount);
                    m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y - 2, (Vector3.forward + Vector3.right).normalized * windAmount);
                    break;
                case LookDirection.Backward:
                    m_WindModule.AddWind(currentBlock.x, currentBlock.y + 1, Vector3.back * windAmount);
                    m_WindModule.AddWind(currentBlock.x, currentBlock.y + 2, Vector3.back * windAmount);
                    m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y + 2, (Vector3.back + Vector3.left).normalized * windAmount);
                    m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y + 2, (Vector3.back + Vector3.right).normalized * windAmount);
                    break;
                case LookDirection.Left:
                    m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y, Vector3.left * windAmount);
                    m_WindModule.AddWind(currentBlock.x - 2, currentBlock.y, Vector3.left * windAmount);
                    m_WindModule.AddWind(currentBlock.x - 2, currentBlock.y - 1, (Vector3.left + Vector3.back).normalized * windAmount);
                    m_WindModule.AddWind(currentBlock.x - 2, currentBlock.y + 1, (Vector3.left + Vector3.forward).normalized * windAmount);
                    break;
                case LookDirection.Right:
                    m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y, Vector3.right * windAmount);
                    m_WindModule.AddWind(currentBlock.x + 2, currentBlock.y, Vector3.right * windAmount);
                    m_WindModule.AddWind(currentBlock.x + 2, currentBlock.y - 1, (Vector3.right + Vector3.back).normalized * windAmount);
                    m_WindModule.AddWind(currentBlock.x + 2, currentBlock.y + 1, (Vector3.right + Vector3.forward).normalized * windAmount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    void AddWindByMovement() {
        var block = m_WindModule.GetBlockByPosition(transform.position);
        if (currentBlock != block) {
            if (currentBlock == null) {
                OnBlockChanged(0, 0, block.x, block.y);
            }
            else {
                OnBlockChanged(currentBlock.x, currentBlock.y, block.x, block.y);
            }

            currentBlock = block;
        }
    }

    private void OnBlockChanged(int xi, int yi, int xf, int yf) {
        //i:initial f:final
        var delta = new Vector3(xf - xi, 0f, yf - yi) * windAmount;
        m_WindModule.AddWind(xi, yi, delta);
        Game.Event.Invoke("OnWindAdded", this, null);
        Debug.Log("Add wind at " + xi + " " + yi + " " + delta.ToString());
    }
}